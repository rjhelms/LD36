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
}