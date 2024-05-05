using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CenterParentSet : MonoBehaviour
{
    [Button]
    private void SetCenterParent()
    {
        Transform newParent = new GameObject().GetComponent<Transform>();
        newParent.name = this.name;
        newParent.parent = transform.parent;

        Vector3 boundsCenter = GetComponent<Renderer>().bounds.center;
        newParent.transform.position = boundsCenter;

        transform.parent = newParent;
    }
}
