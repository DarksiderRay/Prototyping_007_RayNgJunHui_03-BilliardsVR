using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PoolBall_Cue : PoolBall
{
    [Header("Focus Indicator")]
    [SerializeField, NaughtyAttributes.ReadOnly] private bool isFocused = false;
    [SerializeField, NaughtyAttributes.ReadOnly] private Vector3 focusDir;
    [SerializeField] private LineRenderer primaryLineIndicator;
    [SerializeField] private LineRenderer secondaryLineIndicator;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private GameObject hitPointIndicatorObj;
    [SerializeField] private LayerMask boundsLayerMask;
    [SerializeField] private LayerMask ballLayerMask;

    protected override void OnTriggerEnter(Collider col)
    {
        
    }
    
    public void SetFocused(bool value)
    {
        isFocused = value;

        primaryLineIndicator.enabled = isFocused;
        hitPointIndicatorObj.SetActive(isFocused);
        secondaryLineIndicator.enabled = isFocused;
    }

    public void SetDirection(Vector3 dir)
    {
        focusDir = dir;
        
        List<Vector3> linePositions = new();
        linePositions.Add(transform.position);
        linePositions.Add(transform.position + focusDir);

        primaryLineIndicator.positionCount = linePositions.Count;
        primaryLineIndicator.SetPositions(linePositions.ToArray());

        FindHitPoint();
    }

    private void FindHitPoint()
    {
        if (Physics.SphereCast(transform.position, sphereCastRadius, focusDir, out var hit, 
                Mathf.Infinity, boundsLayerMask | ballLayerMask))
        {
            hitPointIndicatorObj.SetActive(true);
            hitPointIndicatorObj.transform.position = hit.point + hit.normal * sphereCastRadius;
            primaryLineIndicator.SetPosition(1, hit.point);

            secondaryLineIndicator.enabled = true;
            List<Vector3> secondaryLinePositions = new();
            secondaryLinePositions.Add(hit.point);
            if (LayerMaskExtensions.Contains(boundsLayerMask, hit.collider.gameObject.layer))
            {
                var dir = Vector3.Reflect(focusDir, hit.normal);
                float secondDist = 1;
                if (Physics.Raycast(hit.point, dir, out var secondHit,
                        1, boundsLayerMask | ballLayerMask))
                {
                    secondDist = Vector3.Distance(hit.point, secondHit.point);
                }
                secondaryLinePositions.Add(hit.point + dir.normalized * secondDist);   
            }
            else if (LayerMaskExtensions.Contains(ballLayerMask, hit.collider.gameObject.layer))
            {
                var dir = -hit.normal;
                float secondDist = 1;
                if (Physics.Raycast(hit.point, dir, out var secondHit,
                        1, boundsLayerMask | ballLayerMask))
                {
                    secondDist = Vector3.Distance(hit.point, secondHit.point);
                }
                
                secondaryLinePositions.Add(hit.point - hit.normal * secondDist);
            }
            

            secondaryLineIndicator.positionCount = secondaryLinePositions.Count;
            secondaryLineIndicator.SetPositions(secondaryLinePositions.ToArray());
        }
        else
        {
            hitPointIndicatorObj.SetActive(false);
            secondaryLineIndicator.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
        Gizmos.DrawLine(transform.position, transform.position + startDirection.normalized);
    }
}
