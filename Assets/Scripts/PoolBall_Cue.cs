using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PoolBall_Cue : PoolBall
{
    [Header("Focus Indicator")]
    [SerializeField, ReadOnly] private bool isFocused = false;
    [SerializeField, ReadOnly] private Vector3 focusDir;
    [SerializeField] private LineRenderer lineIndicator;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private GameObject hitPointIndicatorObj;
    
    public void SetFocused(bool value)
    {
        isFocused = value;

        lineIndicator.enabled = isFocused;
        hitPointIndicatorObj.SetActive(isFocused);
    }

    public void SetDirection(Vector3 dir)
    {
        focusDir = dir;
        
        List<Vector3> linePositions = new();
        linePositions.Add(transform.position);
        linePositions.Add(transform.position + focusDir);

        lineIndicator.positionCount = linePositions.Count;
        lineIndicator.SetPositions(linePositions.ToArray());

        FindHitPoint();
    }

    private void FindHitPoint()
    {
        if (Physics.SphereCast(transform.position, sphereCastRadius, focusDir, out var hit))
        {
            hitPointIndicatorObj.SetActive(true);
            hitPointIndicatorObj.transform.position = hit.point + hit.normal * sphereCastRadius;
        }
        else
        {
            hitPointIndicatorObj.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
        Gizmos.DrawLine(transform.position, transform.position + startDirection.normalized);
    }
}
