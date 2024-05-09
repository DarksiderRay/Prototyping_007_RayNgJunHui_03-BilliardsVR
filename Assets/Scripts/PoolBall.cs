using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PoolBall : MonoBehaviour
{
    public enum BallType
    {
        Cue,
        Solid,
        Striped,
        Ball8
    }

    [Header("Ball Properties")]
    [SerializeField] private BallType ballType;
    [SerializeField] private int number;

    [Header("Physics Components")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider collider;
    [SerializeField] private Rigidbody rigidbody;

    // [Header("Focus Indicator")]
    // [SerializeField, ReadOnly] private bool isFocused = false;
    // [SerializeField, ReadOnly] private Vector3 focusDir;
    // [SerializeField] private LineRenderer lineIndicator;
    // [SerializeField] private float sphereCastRadius;
    // [SerializeField] private GameObject hitPointIndicatorObj;

    [Header("Audio")] 
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip sfxClip_HitCueStick;
    [SerializeField] private AudioClip sfxClip_HitBall;
    [SerializeField] private AudioClip sfxClip_Pocketed;

    [Header("DEBUG")]
    [SerializeField] protected Vector3 startDirection = Vector3.forward;
    [SerializeField] private float startVelocity = 1f;
    
    public delegate void OnBallPocketed(int ballNo);
    public OnBallPocketed onBallPocketed;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<Collider>();
        
        sfxAudioSource = GetComponent<AudioSource>();
    }
    
    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent(out PoolTableHole hole))
        {
            onBallPocketed?.Invoke(number - 1);
            //gameObject.SetActive(false);
            meshRenderer.enabled = false;
            collider.enabled = false;

            hole.ShowText(number);
            
            sfxAudioSource.PlayOneShot(sfxClip_Pocketed);
        }
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.TryGetComponent(out CueTip cueTip))
        {
            rigidbody.AddForce(cueTip.currentVelocity * cueTip.impulseMultiplier, ForceMode.Impulse);
            sfxAudioSource.PlayOneShot(sfxClip_HitCueStick);
        }
        else if (col.gameObject.TryGetComponent(out PoolBall _))
        {
            sfxAudioSource.PlayOneShot(sfxClip_HitBall);
            //sfxAudioSource.volume = col.impulse.magnitude;
        }
        
        //StartCoroutine(cueTip.TempDisableCollider());
    }
    
    private void ResetRigidbody()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void ResetPosition(Vector3 pos)
    {
        StartCoroutine(TempDisableSFX());
        
        //gameObject.SetActive(true);
        meshRenderer.enabled = true;
        collider.enabled = true;
        transform.position = pos;
        ResetRigidbody();
    }

    [Button]
    private void SetStartVelocity()
    {
        rigidbody.AddForce(startDirection * startVelocity, ForceMode.Impulse);
    }

    private IEnumerator TempDisableSFX()
    {
        sfxAudioSource.volume = 0;
        yield return new WaitForSeconds(0.2f);
        sfxAudioSource.volume = 1;
    }
}
