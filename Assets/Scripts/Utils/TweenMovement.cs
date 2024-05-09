using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenMovement : MonoBehaviour
{
    public Vector3 pointA, pointB;
    public float duration = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = pointA;
        MoveToPointB();
    }

    void MoveToPointA()
    {
        transform.DOMove(pointA, duration).OnComplete(MoveToPointB);
    }
    
    void MoveToPointB()
    {
        transform.DOMove(pointB, duration).OnComplete(MoveToPointA);
    }
}
