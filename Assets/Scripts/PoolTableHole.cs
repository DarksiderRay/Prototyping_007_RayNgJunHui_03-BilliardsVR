using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PoolTableHole : MonoBehaviour
{
    private Camera mainCamera;
    
    [SerializeField] private BallPocketedText ballPocketedText;

    void Awake()
    {
        mainCamera = Camera.main;
    }
    
    public void ShowText(int ballNo)
    {
        var newText = Instantiate(ballPocketedText, transform.position,
            Quaternion.LookRotation(transform.position - mainCamera.transform.position, Vector3.up));

        var eulerAngles = newText.transform.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        newText.transform.eulerAngles = eulerAngles;
        
        newText.GetComponent<BallPocketedText>().SetText(ballNo);
    }

    [Button]
    private void DebugShowText()
    {
        ShowText(0);
    }
}
