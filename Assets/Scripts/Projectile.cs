using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    private GameController gameController;

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(0, -gameController.ProjectileSpeed, 0);
    }
}
