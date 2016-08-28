using UnityEngine;

class EnemyProjectile : Projectile
{
    [Header("Movement")]
    public float XMovement = 1.0f;
    public float YMovement = 1.0f;

    void FixedUpdate()
    {
        if (gameController.IsRunning)
        {
            Vector3 moveVector = new Vector3(XMovement * this.transform.localScale.x, YMovement);
            this.transform.position += moveVector;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Top")
        {
            Destroy(this.gameObject);
        }
        else if (coll.gameObject.tag == "Player")
        {
            coll.GetComponent<PlayerController>().Hit();
            Destroy(this.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bounds")
        {
            Destroy(this.gameObject);
        }
    }
}