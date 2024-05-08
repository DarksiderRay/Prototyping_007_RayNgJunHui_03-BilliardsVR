using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PocketedBallUI : MonoBehaviour
{
    [SerializeField] private BallPlacementManager ballPlacementManager;
    [SerializeField] private Transform ballParent;
    [SerializeField] private List<PoolBall> balls;
    
    [SerializeField] private Transform ballToggleParent;
    [SerializeField] private List<Toggle> ballToggles;


    void Awake()
    {
        ballPlacementManager.onResetBalls += ResetToggles;
        
        balls = ballParent.GetComponentsInChildren<PoolBall>().ToList();
        foreach (var ball in balls)
        {
            ball.onBallPocketed += PocketBall;
        }
        
        ballToggles = ballToggleParent.GetComponentsInChildren<Toggle>().ToList();
        ResetToggles();
    }

    private void ResetToggles()
    {
        foreach (var tog in ballToggles)
        {
            tog.isOn = false;
        }
    }

    public void PocketBall(int ballNo)
    {
        if (ballNo < 0 || ballNo >= ballToggles.Count)
            return;
        
        ballToggles[ballNo].isOn = true;
    }
}
