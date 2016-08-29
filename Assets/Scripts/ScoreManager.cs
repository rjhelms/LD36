using UnityEngine;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;
    public int Lives = 3;
    public int HitPoints = 3;
    public int MaxHitPoints = 5;
    public int Level = 1;

    public GameController GameController;

    public int NextHitPoint = 0;
    public int NextOneUp = 0;
    public int HitPointThreshholdIncrease = 0;

    public void Reset()
    {
        this.Score = 0;
        this.Lives = 3;
        this.HitPoints = 3;
        this.MaxHitPoints = 5;
        this.Level = 1;
        this.NextHitPoint = 0;
        this.NextOneUp = 0;
        this.HitPointThreshholdIncrease = 0;
    }
}