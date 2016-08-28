using System.Collections;
using UnityEngine;


enum RiverPiece
{
    FULL,
    SE,
    SW,
    NE,
    NW
}

public class LevelGenerator : MonoBehaviour
{

    public GameObject[] RiverPrefabs;
    public GameObject LevelEndPrefab;
    public GameObject[] FriendlyPrefabs;
    public GameObject[] EnemyPrefabs;

    public int FriendlyCount;
    public int[] EnemyCounts;

    public GameController gameController;

    public int LevelLengthFactor = 50;
    public int StartY = -200;
    public int RiverWidth = 2;
    public int TileSize = 16;
    public float RiverSinuosity = 0.2f;
    public float RiverWidthChangeChance = 0.1f;

    public int RiverMaxWidth = 6;
    public int RiverMinWidth = 2;

    public Transform TerrainTransform;
    public Transform NPCTransform;

    private int targetLength = 0;
    private int screenTileXSize;
    private int minNPCYCoord;
    private int maxNPCYCoord;

    private bool generateRiverRunning = false;
    private bool generateFriendliesRunning = false;
    private bool generateEnemiesRunning = false;

    private bool generateRiverDone = false;
    private bool generateFriendliesDone = false;
    private bool generateEnemiesDone = false;
    private bool generationDone = false;

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameController.IsRunning = false;

        targetLength = ScoreManager.Instance.Level * 50;

        screenTileXSize = gameController.TargetX / TileSize;
        Debug.Log("World is " + screenTileXSize + " tiles wide");
        Debug.Log("Screen is " + gameController.TargetY / TileSize + " units tall");
    }

    // Update is called once per frame
    void Update()
    {
        if (!generateRiverRunning && !generateRiverDone)
        {
            generateRiverRunning = true;
            Debug.Log("Generating river");
            StartCoroutine(generateRiver());
        }
        else if (generateRiverDone && !generateFriendliesRunning && !generateFriendliesDone)
        {
            generateFriendliesRunning = true;
            generateEnemiesRunning = true;
            Debug.Log("Placing NPCs");
            StartCoroutine(generateFriendlies());
            StartCoroutine(generateEnemies());
        }
        else if (generateRiverDone && generateFriendliesDone && generateEnemiesDone && !generationDone)
        {
            generationDone = true;
            gameController.IsRunning = true;
        }
    }

    IEnumerator generateRiver()
    {
        int currentY = 0;
        int currentX = 0;

        int currentRowX = 0;
        int currentRowWidth = 0;
        int lastRowX = 0;
        int lastRowWidth = 0;

        // create river entry section

        int targetLeadingEdge = screenTileXSize / 2 + RiverWidth / 2;
        float direction = Random.value - 0.5f;
        if (direction < 0.0f)
        {
            Debug.Log("River coming from left");
            Debug.Log("Target leading edge: " + targetLeadingEdge);
            while (currentX < targetLeadingEdge)
            {
                int tileX = currentX;
                while (Mathf.Abs(currentX - tileX) <= RiverWidth)
                {
                    if (tileX == currentX)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.SW], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentX && Mathf.Abs(currentX - tileX) < RiverWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NE], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX--;
                }
                currentX++;
                currentY++;
                yield return null;
            }

            lastRowX = targetLeadingEdge - RiverWidth;
        }
        else
        {
            currentX = screenTileXSize - 1;
            targetLeadingEdge = screenTileXSize - targetLeadingEdge;
            Debug.Log("River coming from right");
            Debug.Log("Target leading edge: " + targetLeadingEdge);
            while (currentX > targetLeadingEdge)
            {
                int tileX = currentX;
                while (Mathf.Abs(currentX - tileX) <= RiverWidth)
                {
                    if (tileX == currentX)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.SE], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentX && Mathf.Abs(currentX - tileX) < RiverWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NW], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX++;
                }
                currentX--;
                currentY++;
                yield return null;
            }
            lastRowX = targetLeadingEdge + 1;   // TODO: why plus one?
        }

        minNPCYCoord = currentY;

        // generate main river section

        lastRowWidth = RiverWidth;
        while (currentY < targetLength)
        {
            // calculate shift
            float changeRoll = Random.value;
            if (changeRoll < RiverSinuosity)
            {
                float shiftDirectionRoll = Random.value;
                if (shiftDirectionRoll < 0.5)
                {
                    currentRowX = lastRowX - 1;
                }
                else
                {
                    currentRowX = lastRowX + 1;
                }

            }
            else { currentRowX = lastRowX; }

            if (currentRowX < 0)
                currentRowX = 0;
            if ((currentRowX + lastRowWidth) >= screenTileXSize)
                currentRowX = (screenTileXSize - lastRowWidth) - 1;

            // calculate width change possibility, if hasn't shifted
            if (RiverSinuosity < changeRoll && changeRoll < RiverWidthChangeChance)
            {
                float widthDirectionRoll = Random.value;

                // bias to drift back to target width
                if (lastRowWidth < RiverWidth)
                    widthDirectionRoll *= 2;
                if (lastRowWidth > RiverWidth)
                    widthDirectionRoll /= 2;

                if (widthDirectionRoll < 0.5)
                {
                    currentRowWidth = lastRowWidth - 1;
                }
                else
                {
                    currentRowWidth = lastRowWidth + 1;
                }

                if (currentRowWidth < RiverMinWidth)
                    currentRowWidth = RiverMinWidth;
                if (currentRowWidth > RiverMaxWidth)
                    currentRowWidth = RiverMaxWidth;
            }
            else
            {
                currentRowWidth = lastRowWidth;
            }
            // hack to stop river from spilling off right size
            if (currentRowX + currentRowWidth == screenTileXSize)
                currentRowWidth -= 1;

            // instantiate thingies
            Debug.Log("CurrentY: " + currentY);
            Debug.Log("CurrentRowX: " + currentRowX);
            Debug.Log("CurrentRowWidth: " + currentRowWidth);
            // create left bank
            if (currentRowX < lastRowX)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.SE], tileCoordToPosition(currentRowX, currentY), Quaternion.identity, TerrainTransform);
            }
            else if (currentRowX > lastRowX)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.NE], tileCoordToPosition(currentRowX - 1, currentY), Quaternion.identity, TerrainTransform);
                Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(currentRowX, currentY), Quaternion.identity, TerrainTransform);
            }
            else if (currentRowX == lastRowX)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(currentRowX, currentY), Quaternion.identity, TerrainTransform);
            }
            else
            {
                throw new System.InvalidOperationException("River generation broke the universe!");
            }

            // create right bank
            int lastRowRightBank = lastRowX + lastRowWidth - 1;
            int currentRowRightBank = currentRowX + currentRowWidth - 1;
            Debug.Log("currentRowRightBank: " + currentRowRightBank);
            if (currentRowRightBank < lastRowRightBank)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.NW], tileCoordToPosition(currentRowRightBank + 1, currentY), Quaternion.identity, TerrainTransform);
                Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(currentRowRightBank, currentY), Quaternion.identity, TerrainTransform);
            }
            else if (currentRowRightBank > lastRowRightBank)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.SW], tileCoordToPosition(currentRowRightBank, currentY), Quaternion.identity, TerrainTransform);
            }
            else if (currentRowRightBank == lastRowRightBank)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(currentRowRightBank, currentY), Quaternion.identity, TerrainTransform);
            }
            else
            {
                throw new System.InvalidOperationException("River generation broke the universe!");
            }
            // fill in
            for (int i = currentRowX + 1; i < currentRowRightBank; i++)
            {
                Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(i, currentY), Quaternion.identity, TerrainTransform);
            }
            // increment currentY and shift X and width variables
            lastRowX = currentRowX;
            lastRowWidth = currentRowWidth;
            currentY++;
            yield return null;
        }

        maxNPCYCoord = currentY;
        // generate river exit section

        if (direction < 0.0f)
        {
            currentRowX += currentRowWidth;
            Debug.Log("River leaving to the right");
            Debug.Log("Target leading edge: " + targetLeadingEdge);
            while (currentRowX - currentRowWidth < screenTileXSize)
            {
                int tileX = currentRowX;
                while (Mathf.Abs(currentRowX - tileX) <= currentRowWidth)
                {
                    if (tileX == currentRowX)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.SW], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentRowX && Mathf.Abs(currentRowX - tileX) < currentRowWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NE], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX--;
                }
                currentRowX++;
                currentY++;
                yield return null;
            }
        }
        else
        {
            currentRowX--;
            Debug.Log("River leaving to the left");
            Debug.Log("Target leading edge: " + targetLeadingEdge);
            while (currentRowX + currentRowWidth + 1 > 0) // TODO: why plus one?
            {
                int tileX = currentRowX;
                while (Mathf.Abs(currentRowX - tileX) <= currentRowWidth)
                {
                    if (tileX == currentRowX)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.SE], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentRowX && Mathf.Abs(currentRowX - tileX) < currentRowWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NW], tileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX++;
                }
                currentRowX--;
                currentY++;
                yield return null;
            }
        }
        Instantiate(LevelEndPrefab, new Vector3(0, StartY - (currentY * TileSize), 5), Quaternion.identity, TerrainTransform);
        generateRiverRunning = false;
        generateRiverDone = true;
    }

    IEnumerator generateFriendlies()
    {
        int count = 0;
        while (count < FriendlyCount)
        {
            bool hasPlacedFriendly = false;
            while (!hasPlacedFriendly)
            {
                int friendlyX = Random.Range(1, screenTileXSize - 1);
                int friendlyY = Random.Range(minNPCYCoord, maxNPCYCoord);
                Vector3 friendlyPositionVector = tileCoordToPosition(friendlyX, friendlyY, 3);
                Collider2D[] collisions = Physics2D.OverlapCircleAll((Vector2)friendlyPositionVector, 8);
                if (collisions.Length == 0)
                {
                    int prefabIndex = Random.Range(0, FriendlyPrefabs.Length);
                    GameObject newFriendly = (GameObject)Instantiate(FriendlyPrefabs[prefabIndex], friendlyPositionVector, Quaternion.identity, NPCTransform);
                    if (Random.value < 0.5f)
                    {
                        newFriendly.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    hasPlacedFriendly = true;
                }
                else
                {
                    yield return null;
                }
            }
            count++;
            yield return null;
        }
        generateFriendliesRunning = false;
        generateFriendliesDone = true;
    }

    IEnumerator generateEnemies()
    {
        for (int i = 0; i < EnemyCounts.Length; i++)
        {
            int count = 0;
            while (count < EnemyCounts[i])
            {
                bool hasPlacedEnemy = false;
                while (!hasPlacedEnemy)
                {
                    int EnemyX = Random.Range(1, screenTileXSize - 1);
                    int EnemyY = Random.Range(minNPCYCoord, maxNPCYCoord);
                    Vector3 enemyPositionVector = tileCoordToPosition(EnemyX, EnemyY, 3);
                    Collider2D[] collisions = Physics2D.OverlapCircleAll((Vector2)enemyPositionVector, 8);
                    if (collisions.Length == 0)
                    {
                        GameObject newEnemy = (GameObject)Instantiate(EnemyPrefabs[i], enemyPositionVector, Quaternion.identity, NPCTransform);
                        if (Random.value < 0.5f)
                        {
                            newEnemy.transform.localScale = new Vector3(-1, 1, 1);
                        }
                        hasPlacedEnemy = true;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                count++;
                yield return null;
            }
        }
        generateEnemiesRunning = false;
        generateEnemiesDone = true;
    }

    private Vector3 tileCoordToPosition(int x, int y)
    {
        int worldX = x * TileSize - (gameController.TargetX / 2);
        int worldY = StartY - y * TileSize;

        worldX += TileSize / 2;
        return new Vector3(worldX, worldY, 5);
    }

    private Vector3 tileCoordToPosition(int x, int y, float z)
    {
        int worldX = x * TileSize - (gameController.TargetX / 2);
        int worldY = StartY - y * TileSize;

        worldX += TileSize / 2;
        return new Vector3(worldX, worldY, z);
    }
}
