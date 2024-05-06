using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class BallPlacementManager : MonoBehaviour
{
    [Header("Balls")]
    [SerializeField] private PoolBall cueBall;
    [SerializeField] private PoolBall ball_1;
    [SerializeField] private PoolBall ball_8;
    [SerializeField] private List<PoolBall> solidBalls = new();
    [SerializeField] private List<PoolBall> stripedBalls = new();
    
    [Header("Placements")]
    [SerializeField] private Transform cueBallPlacement;
    [SerializeField] private Transform ballPlacement_1;
    [SerializeField] private Transform ballPlacement_8;
    [SerializeField] private List<Transform> ballPlacements_Corners;
    [SerializeField] private List<Transform> ballPlacements_Remaining;


    [Button]
    public void ResetBalls()
    {
        cueBall.ResetPosition(cueBallPlacement.position);
        ball_1.ResetPosition(ballPlacement_1.position);
        ball_8.ResetPosition(ballPlacement_8.position);

        PoolBall cornerBall_Solid = solidBalls[Random.Range(0, solidBalls.Count)];
        PoolBall cornerBall_Striped = stripedBalls[Random.Range(0, stripedBalls.Count)];
        int randomCornerIndex = Random.Range(0,2);
        for (int i = 0; i < ballPlacements_Corners.Count; i++)
        {
            if (i == randomCornerIndex)
            {
                cornerBall_Solid.ResetPosition(ballPlacements_Corners[i].position);
            }
            else
            {
                cornerBall_Striped.ResetPosition(ballPlacements_Corners[i].position);
            }
        }

        List<PoolBall> remainingBalls = solidBalls.Union(stripedBalls).ToList();
        remainingBalls.Remove(cornerBall_Solid);
        remainingBalls.Remove(cornerBall_Striped);
        List<PoolBall> shuffledRemainingBalls = ShuffleBalls(remainingBalls);

        for (int i = 0; i < ballPlacements_Remaining.Count; i++)
        {
            if (i >= shuffledRemainingBalls.Count)
                break;
            
            shuffledRemainingBalls[i].ResetPosition(ballPlacements_Remaining[i].position);
        }
    }

    private List<PoolBall> ShuffleBalls(List<PoolBall> originalBalls)
    {
        List<PoolBall> shuffledBalls = new();
        int count = originalBalls.Count;

        for (int i = 0; i < count; i++)
        {
            PoolBall ball = originalBalls[Random.Range(0, originalBalls.Count)];
            shuffledBalls.Add(ball);
            originalBalls.Remove(ball);
        }

        return shuffledBalls;
    }

}
