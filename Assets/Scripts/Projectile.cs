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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bounds")
        {
            Debug.Log("Collided with bounds");

            /* 
             * Disable the collider, and destroy it a second later.
             * this allows projectiles to leave the screen before disappearing,
             * without hitting off-screen enemies 
             */

            GetComponent<CircleCollider2D>().enabled = false;
            DestroyObject(this.gameObject, 0.2f);
        }
    }
}
