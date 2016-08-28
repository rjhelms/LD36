using UnityEngine;

public class PlayerProjectile : Projectile
{
    void FixedUpdate()
    {
        {
            transform.position += new Vector3(0, -ScoreManager.Instance.GameController.ProjectileSpeed, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bottom")
        {
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
            coll.GetComponent<NPC>().Hit();
            DestroyObject(this.gameObject);
        }
    }
}
