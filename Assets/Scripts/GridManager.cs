using UnityEditor.Animations;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private float deltawidth;
    [SerializeField] private int height;
    [SerializeField] private float deltaheight;
    [SerializeField] private int mineCount;

    enum GamingState
    {
        win, lose, playing
    }

    private GamingState state;

    [SerializeField] private int cellsLeft;

    [HideInInspector]
    public static GridManager Instance;

    private Tile[,] grid;
    [SerializeField] private GameObject tilePrefab; 
    [SerializeField] private Transform gridParent; 

    [SerializeField] private GameObject[] objectPool;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instance of gridManager");
        }
    }

    public void CreateEmptyGrid()
    {
        Debug.Log("Creating Grid");

        grid = new Tile[width, height];

        ClearSceen();

        TerrainGenerator.Instance.CreatNewTerrain();

        cellsLeft = width * height;

        state = GamingState.playing;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, gridParent);
                tileObject.transform.position = new Vector3(x * deltawidth, 0, y * deltaheight);
                Tile tile = tileObject.GetComponent<Tile>();
                tile.SetupParams(x, y);
                grid[x, y] = tile;
            }
        }
    }

    public void CreateGrid(int posx, int posy)
    {


        PlaceMines();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];

                if (tile.isMine)
                {
                    tile.SetupParams(9);
                    continue;
                }

                int mineCount = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        int neighborX = x + dx;
                        int neighborY = y + dy;

                        if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                        {
                            if (grid[neighborX, neighborY].isMine)
                            {
                                mineCount++;
                            }
                        }
                    }
                }

                tile.SetupParams(mineCount);
            }
        }

        if (grid[posx, posy].mineCount != 0)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].isMine = false;
                    grid[x, y].mineCount = -1;
                }
            }
            CreateGrid(posx,posy);
        }else
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].SetupObject(objectPool[grid[x, y].mineCount]);
                }
            }
        }

    }

    private void PlaceMines()
    {
        int minesPlaced = 0;
        while (minesPlaced < mineCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (!grid[x, y].isMine)
            {
                grid[x, y].SetupParams(true);
                minesPlaced++;
            }
        }
        

    }

    public void OpenTile(int x, int y)
    {
        if (state == GamingState.playing)
        {
            var tile = grid[x, y];

            if (tile.isMine)
            {
                state = GamingState.lose;
                GameManager.Instance.EndGame();
            }

            if (!tile.isOpen)
            {
                tile.Reveal();
                ReduceUnknownCells();
            }


            if (tile.mineCount == 0)
                OpenBlankTiles(x, y);
        }

    }

    public void OpenBlankTiles(int x, int y)
    {

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = x + dx;
                int neighborY = y + dy;
                if (dx == 0 && dy == 0) continue;
                else if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                    if (!grid[neighborX, neighborY].isOpen)
                        if (grid[neighborX, neighborY].mineCount == 0)
                        {
                            grid[neighborX, neighborY].Reveal();
                            ReduceUnknownCells();
                            OpenBlankTiles(neighborX, neighborY);
                        }
                        else
                        {
                            grid[neighborX, neighborY].Reveal();
                            ReduceUnknownCells();
                        }
            }
        }

    }

    public Vector2 GetSize() => new Vector2((width-1) * deltawidth, (height-1) * deltaheight);
    public Vector2Int GetGridSize() => new Vector2Int(width, height);

    private void ClearSceen()
    {
        foreach(Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }


    public void ReduceUnknownCells()
    {
        cellsLeft--;
        if (cellsLeft <= mineCount)
        {
            state = GamingState.win;
            GameManager.Instance.WinGame();
        }
           
    }


}
