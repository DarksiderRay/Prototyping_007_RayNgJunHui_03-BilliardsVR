using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CueTip : MonoBehaviour
{
    [SerializeField] private Rigidbody cueStickRigidbody;
    [SerializeField] private Collider col;
    
    public float impulseMultiplier = 5;

    [Header("Aim Components")]
    [SerializeField] private float focusRange = 0.25f;
    [SerializeField] private LayerMask ballLayer;
    [SerializeField, ReadOnly] private PoolBall_Cue focusedCueBall;
    
    [Header("DEBUG")] 
    public Vector3 currentVelocity;
    private Vector3 previousPos;

    void Update()
    {
        currentVelocity = (transform.position - previousPos)/ Time.deltaTime;
        previousPos = transform.position;
        
        // check focused ball
        UpdateFocusedBall();

        if (focusedCueBall)
        {
            focusedCueBall.SetDirection(transform.up);
        }
    }
    
    public IEnumerator TempDisableCollider()
    {
        col.enabled = false;

        yield return new WaitForSeconds(1f);
        col.enabled = true;
    }

    public void ToggleCollider(bool value)
    {
        col.enabled = value;
    }

    private void UpdateFocusedBall()
    {
        if (focusedCueBall)
            focusedCueBall.SetFocused(false);
        
        if (Physics.Raycast(transform.position, transform.up, out var hit, focusRange, ballLayer,
                QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.TryGetComponent(out PoolBall_Cue ball))
            {
                focusedCueBall = ball;
                focusedCueBall.SetFocused(true);
                return;
            }
        }

        focusedCueBall = null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * focusRange);
    }
}
