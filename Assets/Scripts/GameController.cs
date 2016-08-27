using UnityEngine;

public class GameController : MonoBehaviour
{

    public Material RenderTexture;
    public Camera WorldCamera;

    [Header("Resolution")]
    public int TargetX = 160;
    public int TargetY = 200;
    private float pixelRatioAdjustment;

    [Header("Game Balance")]
    public float MoveSpeed = 1.0f;
    public float ProjectileSpeed = 2.0f;
    public float ShootSpeed = 5.0f;

    [Header("Prefabs")]
    public GameObject PlayerProjectilePrefab;

    // Use this for initialization
    void Start()
    {
        pixelRatioAdjustment = (float)TargetX / (float)TargetY;
        if (pixelRatioAdjustment <= 1)
        {
            RenderTexture.mainTextureScale = new Vector2(pixelRatioAdjustment, 1);
            RenderTexture.mainTextureOffset = new Vector2((1 - pixelRatioAdjustment) / 2, 0);
            WorldCamera.orthographicSize = TargetY / 2;
        }
        else
        {
            pixelRatioAdjustment = 1f / pixelRatioAdjustment;
            RenderTexture.mainTextureScale = new Vector2(1, pixelRatioAdjustment);
            RenderTexture.mainTextureOffset = new Vector2(0, (1 - pixelRatioAdjustment) / 2);
            WorldCamera.orthographicSize = TargetX / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
