using UnityEngine;
using System.Collections;

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
    public GameController gameController;

    public int LevelLengthFactor = 50;
    public int StartY = -200;
    public int RiverWidth = 2;
    public int TileSize = 16;

    public Transform TerrainTransform;

    private int targetLength = 0;
    private int screenTileXSize;
    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameController.IsRunning = false;

        targetLength = ScoreManager.Instance.Level * 50;

        screenTileXSize = gameController.TargetX / TileSize;
        Debug.Log("World is " + screenTileXSize + " tiles wide");
        Debug.Log("Screen is " + gameController.TargetY / TileSize + " units tall");
        generateRiver();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void generateRiver()
    {
        int currentY = 0;
        int currentX = 0;
        float direction = Random.value - 0.5f;

        int targetLeadingEdge = screenTileXSize / 2 + RiverWidth / 2;
        if (direction < 0)
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
                        Instantiate(RiverPrefabs[(int)RiverPiece.SW], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentX && Mathf.Abs(currentX - tileX) < RiverWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NE], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX--;
                }
                currentX++;
                currentY++;
            }

        }
        else
        {
            currentX = 9;
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
                        Instantiate(RiverPrefabs[(int)RiverPiece.SE], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else if (tileX != currentX && Mathf.Abs(currentX - tileX) < RiverWidth)
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.FULL], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    else
                    {
                        Instantiate(RiverPrefabs[(int)RiverPiece.NW], TileCoordToPosition(tileX, currentY), Quaternion.identity, TerrainTransform);
                    }
                    tileX++;
                }
                currentX--;
                currentY++;
            }
        }
    }

    private Vector3 TileCoordToPosition(int x, int y)
    {
        int worldX = x * TileSize - (gameController.TargetX / 2);
        int worldY = StartY - y * TileSize;

        worldX += TileSize / 2;
        return new Vector3(worldX, worldY, 5);
    }
}
