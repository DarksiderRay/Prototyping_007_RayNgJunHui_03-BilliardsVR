using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenRotation : MonoBehaviour
{
    public float rotSpeed = 10;

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles += new Vector3(0, rotSpeed, 0) * Time.deltaTime;
    }
}
