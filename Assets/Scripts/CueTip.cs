using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueTip : MonoBehaviour
{
    [SerializeField] private Rigidbody cueStickRigidbody;
    [SerializeField] private Collider col;
    
    public float impulseMultiplier = 5;

    [Header("DEBUG")] 
    public Vector3 currentVelocity;
    private Vector3 previousPos;

    void Update()
    {
        currentVelocity = (transform.position - previousPos)/ Time.deltaTime;
        previousPos = transform.position;
    }
    
    public IEnumerator TempDisableCollider()
    {
        col.enabled = false;

        yield return new WaitForSeconds(1f);
        col.enabled = true;
    }
}
