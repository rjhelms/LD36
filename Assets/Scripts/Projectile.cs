using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    protected GameController gameController;

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }


}
