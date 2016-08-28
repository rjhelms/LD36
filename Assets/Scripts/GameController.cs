using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PyramidConstructionState
{
    UFO_CLEAR,
    BUILDING,
    RETURN,
    DONE
}

enum PyramidTiles
{
    LEFT,
    FULL,
    RIGHT
}

public class GameController : MonoBehaviour
{

    public Material RenderTexture;
    public Camera WorldCamera;
    public Transform PlayerTransform;
    public bool IsRunning;
    public bool IsWinning;

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
    public GameObject GetReadyPanel;
    public Text ScoreText;
    public Text LivesText;

    [Header("Game Balance")]
    public float MoveSpeedX = 1.0f;
    public float MoveSpeedY = 1.0f;
    public float ProjectileSpeed = 2.0f;
    public float ShootInterval = 5.0f;
    public float NPCWalkSpeed = 1.0f;
    public float NPCBaseStateChangeTime = 0.5f;
    public float BaseNPCFireCooldown = 1.0f;
    public int PyramidTilePoints = 10;
    public float PotentialPoints = 0;
    private int levelStartScore;

    [Header("Prefabs")]
    public GameObject PlayerProjectilePrefab;
    public GameObject[] NPCPrefabs;

    [Header("Sounds")]
    public AudioClip PlayerHitSound;
    public AudioClip PlayerLoseSound;
    public AudioClip NPCConvertedSound;
    public AudioClip PyramidBuildSound;
    private AudioSource audioSource;

    [Header("PyramidBuilder")]
    public PyramidConstructionState WinState;
    public GameObject PyramidBuilderPrefab;
    public GameObject[] PyramidTilePrefabs;
    private int pyramidSize;
    private GameObject pyramidBuilder;
    public GameObject[] currentPyramidRow;
    private int currentPyramidRowSize;

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
        audioSource = this.GetComponent<AudioSource>();
        ScoreManager.Instance.GameController = this;
    }

    void Update()
    {
        if (IsWinning && WinState == PyramidConstructionState.DONE)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }
    void FixedUpdate()
    {
        UpdateUI();
        if (IsWinning)
            doPyramidConstruction();
    }

    public void UpdateUI()
    {
        if (IsRunning)
            WorldCamera.transform.position = new Vector3(0, PlayerTransform.position.y - this.PlayerCameraYOffset, -10);
        UIHitPointPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScoreManager.Instance.HitPoints * 16);
        ScoreText.text = System.String.Format("{0}", ScoreManager.Instance.Score);
        LivesText.text = System.String.Format("LEVEL {0,-3} LIVES {0,-2}", ScoreManager.Instance.Level, ScoreManager.Instance.Lives);
        Canvas.ForceUpdateCanvases();
    }

    public void PlayerHit()
    {
        Debug.Log("Hit!");
        ScoreManager.Instance.HitPoints--;
        if (ScoreManager.Instance.HitPoints == 0)
        {
            this.Lose();
        }
        else
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(PlayerHitSound);
        }
    }
    public void Lose()
    {
        IsRunning = false;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(PlayerLoseSound);
        Debug.Log("lost");
    }

    public void LevelClear()
    {
        IsRunning = false;
        int pointsScored = ScoreManager.Instance.Score - levelStartScore;
        Debug.Log(pointsScored + " points out of " + PotentialPoints);
        pyramidSize = Mathf.RoundToInt((pointsScored / PotentialPoints) * 5);
        pyramidSize *= 2;
        if (pyramidSize < 2)
        {
            pyramidSize = 2;
        }
        Debug.Log("Gets you a " + pyramidSize + "tile pyramid");
        WinState = PyramidConstructionState.UFO_CLEAR;
        IsWinning = true;
    }

    public void AddPoints(int value)
    {
        ScoreManager.Instance.Score += value;

    }
    public void RegisterConversion(int value)
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(NPCConvertedSound);
        AddPoints(value);
    }

    public void StartRunning()
    {
        GetReadyPanel.SetActive(false);
        levelStartScore = ScoreManager.Instance.Score;
        this.IsRunning = true;
    }

    private void BuildPyramidRow()
    {
        currentPyramidRow = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            currentPyramidRow[i] = null;
        }
        int leftPyramidPosition = 5 - currentPyramidRowSize / 2;
        int rightPyramidPosition = 4 + currentPyramidRowSize / 2;
        currentPyramidRow[leftPyramidPosition] = PyramidTilePrefabs[(int)PyramidTiles.LEFT];
        currentPyramidRow[rightPyramidPosition] = PyramidTilePrefabs[(int)PyramidTiles.RIGHT];
        if (currentPyramidRowSize > 2)
        {
            for (int i = leftPyramidPosition + 1; i < rightPyramidPosition; i++)
            {
                currentPyramidRow[i] = PyramidTilePrefabs[(int)PyramidTiles.FULL];
            }
        }

    }
    private void doPyramidConstruction()
    {
        switch (WinState)
        {
            case PyramidConstructionState.UFO_CLEAR:
                PlayerTransform.position -= new Vector3(0, MoveSpeedY * 2);
                if (WorldCamera.transform.position.y - PlayerTransform.position.y > 100)
                {
                    WinState++;
                    pyramidBuilder = (GameObject)Instantiate(PyramidBuilderPrefab, new Vector3(-84, WorldCamera.transform.position.y - 36, 1), Quaternion.identity);
                    currentPyramidRowSize = pyramidSize;
                    BuildPyramidRow();
                }
                break;
            case PyramidConstructionState.BUILDING:

                pyramidBuilder.transform.position += new Vector3(2 * pyramidBuilder.transform.localScale.x, 0);

                if (pyramidBuilder.transform.position.x > 84 && pyramidBuilder.transform.localScale.x > 0)
                {
                    pyramidBuilder.transform.position += new Vector3(0, 16);
                    pyramidBuilder.transform.localScale = new Vector3(-1, 1, 1);
                    currentPyramidRowSize -= 2;
                    BuildPyramidRow();
                    if (currentPyramidRowSize <= 1)
                    {
                        WinState++;
                    }
                }
                else if (pyramidBuilder.transform.position.x < -84 && pyramidBuilder.transform.localScale.x < 0)
                {
                    pyramidBuilder.transform.position += new Vector3(0, 16);
                    pyramidBuilder.transform.localScale = new Vector3(1, 1, 1);
                    currentPyramidRowSize -= 2;
                    BuildPyramidRow();
                    if (currentPyramidRowSize <= 1)
                    {
                        WinState++;
                    }
                }

                if (((int)pyramidBuilder.transform.position.x + 72) % 16 == 0)
                {
                    int position = Mathf.RoundToInt((pyramidBuilder.transform.position.x + 72) / 16);
                    Debug.Log(position);
                    if (currentPyramidRow[position] != null)
                    {
                        Instantiate(currentPyramidRow[position], new Vector3(pyramidBuilder.transform.position.x, pyramidBuilder.transform.position.y - 8, 2), Quaternion.identity);
                        audioSource.pitch = Random.Range(0.8f, 1.2f);
                        audioSource.PlayOneShot(PyramidBuildSound);
                        AddPoints(PyramidTilePoints);
                    }
                }
                break;
            case PyramidConstructionState.RETURN:
                if (pyramidBuilder.transform.position.x != 0)
                {
                    pyramidBuilder.transform.position += new Vector3(2 * pyramidBuilder.transform.localScale.x, 0);
                }
                else
                {
                    pyramidBuilder.GetComponent<NPC>().Converted = true;
                    pyramidBuilder.GetComponent<NPC>().AnimateInterval = 0.25f;
                    if (pyramidSize == 10)
                    {
                        pyramidBuilder.transform.localScale *= 4;
                    }
                    else if (pyramidSize >= 8)
                    {
                        pyramidBuilder.transform.localScale *= 3;
                    }
                    else if (pyramidSize >= 6)
                    {
                        pyramidBuilder.transform.localScale *= 2;
                    }
                    WinState++;
                }
                break;
            case PyramidConstructionState.DONE:
                if (Input.GetButtonDown("Fire1"))
                    SceneManager.LoadScene("Main");
                break;
        }
    }
}
