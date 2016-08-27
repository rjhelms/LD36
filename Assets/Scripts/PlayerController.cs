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

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        playerTransform = GetComponent<Transform>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        projectileSpriteIndex = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire! " + projectileSpriteIndex);

            Vector3 projectilePosition = new Vector3(playerTransform.position.x, playerTransform.position.y - 8);
            GameObject projectile = (GameObject)Instantiate(gameController.PlayerProjectilePrefab,
                projectilePosition, Quaternion.identity);

            projectile.GetComponent<SpriteRenderer>().sprite = ProjectileSprites[projectileSpriteIndex];

            projectileSpriteIndex++;
            if (projectileSpriteIndex == ProjectileSprites.GetLength(0))
                projectileSpriteIndex = 0;
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        int moveMagnitude = Mathf.RoundToInt(moveInput * gameController.MoveSpeed);
        playerTransform.position += new Vector3(moveMagnitude, 0, 0);

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
