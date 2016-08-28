using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public Material RenderTexture;
    public Camera WorldCamera;
    public Transform PlayerTransform;
    public bool IsRunning;

    [Header("Resolution and Display")]
    public int TargetX = 160;
    public int TargetY = 200;
    private float pixelRatioAdjustment;

    [Header("Position Offsets")]
    public int PlayerCameraYOffset = 120;
    public int PlayerXCoordBoundsMagnitude = 80;
    public int ProjectileYOffset = 12;

    [Header("UI Elements")]
    public Transform UICanvasTransform;
    public RectTransform UIHitPointPanelTransform;

    [Header("Game Balance")]
    public float MoveSpeedX = 1.0f;
    public float MoveSpeedY = 1.0f;
    public float ProjectileSpeed = 2.0f;
    public float ShootInterval = 5.0f;
    public float NPCWalkSpeed = 1.0f;
    public float NPCBaseStateChangeTime = 0.5f;

    [Header("Prefabs")]
    public GameObject PlayerProjectilePrefab;
    public GameObject[] NPCPrefabs;

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
    void LateUpdate()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        WorldCamera.transform.position = new Vector3(0, PlayerTransform.position.y - this.PlayerCameraYOffset, -10);
        UIHitPointPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScoreManager.Instance.HitPoints * 16);
        Canvas.ForceUpdateCanvases();
    }

    public void Lose()
    {
        IsRunning = false;
        Debug.Log("lost");
    }

    public void LevelClear()
    {
        IsRunning = false;
    }
}
