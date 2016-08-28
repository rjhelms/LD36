using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite SpriteLeft;
    public Sprite SpriteRight;
    public Sprite[] ProjectileSprites;

    private GameController gameController;
    private Transform playerTransform;
    private SpriteRenderer playerSpriteRenderer;
    private int projectileSpriteIndex;

    private float nextProjectileTime;

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        playerTransform = GetComponent<Transform>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        projectileSpriteIndex = 0;
        nextProjectileTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsRunning)
        {
            if (Input.GetButton("Fire1"))
            {
                if (Time.fixedTime > nextProjectileTime)
                {
                    Debug.Log("Fire! " + projectileSpriteIndex);
                    nextProjectileTime = Time.fixedTime + gameController.ShootInterval;

                    Vector3 projectilePosition = new Vector3(playerTransform.position.x, playerTransform.position.y - gameController.ProjectileYOffset);
                    GameObject projectile = (GameObject)Instantiate(gameController.PlayerProjectilePrefab,
                        projectilePosition, Quaternion.identity);

                    projectile.GetComponent<SpriteRenderer>().sprite = ProjectileSprites[projectileSpriteIndex];

                    projectileSpriteIndex++;
                    if (projectileSpriteIndex == ProjectileSprites.GetLength(0))
                        projectileSpriteIndex = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (gameController.IsRunning)
        {
            float moveInput = Input.GetAxis("Horizontal");
            int moveMagnitude = Mathf.RoundToInt(moveInput * gameController.MoveSpeedX);
            playerTransform.position += new Vector3(moveMagnitude, -gameController.MoveSpeedY, 0);

            if (Mathf.Abs(playerTransform.position.x) >= gameController.PlayerXCoordBoundsMagnitude)
            {

                if (playerTransform.position.x < 0)
                {
                    playerTransform.position = new Vector3(-gameController.PlayerXCoordBoundsMagnitude, playerTransform.position.y);
                }
                else
                {
                    playerTransform.position = new Vector3(gameController.PlayerXCoordBoundsMagnitude, playerTransform.position.y);
                }
            }
            if (moveMagnitude > 0)
            {
                playerSpriteRenderer.sprite = SpriteRight;
            }
            else if (moveMagnitude < 0)
            {
                playerSpriteRenderer.sprite = SpriteLeft;
            }
        }
    }

    public void Hit()
    {
        Debug.Log("Hit!");
        ScoreManager.Instance.HitPoints--;
        if (ScoreManager.Instance.HitPoints == 0)
        {
            gameController.Lose();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "LevelEnd")
        {
            gameController.LevelClear();
        }
    }
}
