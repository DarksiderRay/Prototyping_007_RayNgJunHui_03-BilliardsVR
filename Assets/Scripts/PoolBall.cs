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
    [SerializeField] private Rigidbody rigidbody;

    // [Header("Focus Indicator")]
    // [SerializeField, ReadOnly] private bool isFocused = false;
    // [SerializeField, ReadOnly] private Vector3 focusDir;
    // [SerializeField] private LineRenderer lineIndicator;
    // [SerializeField] private float sphereCastRadius;
    // [SerializeField] private GameObject hitPointIndicatorObj;

    [Header("DEBUG")]
    [SerializeField] protected Vector3 startDirection = Vector3.forward;
    [SerializeField] private float startVelocity = 1f;
    
    public delegate void OnBallPocketed(int ballNo);
    public OnBallPocketed onBallPocketed;

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent(out PoolTableHole hole))
        {
            onBallPocketed?.Invoke(number - 1);
            gameObject.SetActive(false);
        }
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.TryGetComponent(out CueTip cueTip))
            return;
        
        rigidbody.AddForce(cueTip.currentVelocity * cueTip.impulseMultiplier, ForceMode.Impulse);

        //StartCoroutine(cueTip.TempDisableCollider());
    }
    
    private void ResetRigidbody()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void ResetPosition(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        ResetRigidbody();
    }

    [Button]
    private void SetStartVelocity()
    {
        rigidbody.AddForce(startDirection * startVelocity, ForceMode.Impulse);
    }

    
}
