using UnityEngine;

public class PlayerProjectile : Projectile
{
    void FixedUpdate()
    {
        transform.position += new Vector3(0, -gameController.ProjectileSpeed, 0);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bottom")
        {
            Debug.Log("Collided with bottom bounds");

            /* 
             * Disable the collider, and destroy it a second later.
             * this allows projectiles to leave the screen before disappearing,
             * without hitting off-screen enemies 
             */

            GetComponent<CircleCollider2D>().enabled = false;
            DestroyObject(this.gameObject, 0.2f);
        }

        if (coll.gameObject.tag == "NPC")
        {
            Debug.Log("Collided with NPC");
            coll.GetComponent<NPC>().Hit();
            DestroyObject(this.gameObject);
        }
    }
}
