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

    [Header("Components")]
    [SerializeField] private Rigidbody rigidbody;

    [Header("DEBUG")]
    [SerializeField] private Vector3 startDirection = Vector3.forward;
    [SerializeField] private float startVelocity = 1f;

    private void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.TryGetComponent(out CueTip cueTip))
            return;
        
        Debug.Log(col.impulse);
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
        transform.position = pos;
        ResetRigidbody();
    }

    [Button]
    private void SetStartVelocity()
    {
        rigidbody.AddForce(startDirection * startVelocity, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, transform.position + startDirection.normalized);
    }
}
