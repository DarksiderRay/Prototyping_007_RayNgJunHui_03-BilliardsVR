using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerpendicularTest : MonoBehaviour
{
    public Transform cueStick;
    public Transform hand;

    [Space] public Vector3 alignedDir;
    
    private Vector3 planeNormal, dirFromHandToStick;

    // Update is called once per frame
    void Update()
    {
        planeNormal = Vector3.Cross(cueStick.up, cueStick.forward);
        dirFromHandToStick = hand.position - cueStick.position;

        alignedDir = Vector3.ProjectOnPlane(dirFromHandToStick, planeNormal);
        alignedDir = Vector3.ProjectOnPlane(alignedDir, cueStick.forward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + planeNormal);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + dirFromHandToStick);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + alignedDir);
    }
}
