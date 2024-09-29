using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int deltawidth;
    [SerializeField] private int height;
    [SerializeField] private int deltaheight;
    [SerializeField] private int mineCount;

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
        Debug.Log("Instance of Grid");

        grid = new Tile[width, height];

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
        var tile = grid[x, y];

        if (tile.mineCount == 0)
            OpenBlankTiles(x, y);
        
        if (!tile.isOpen)
            tile.Reveal();

        if(tile.isMine)
            GameManager.Instance.EndGame();


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
                    if (grid[neighborX, neighborY].mineCount == 0 && !grid[neighborX, neighborY].isOpen)
                    {
                        grid[neighborX, neighborY].Reveal();
                        OpenBlankTiles(neighborX, neighborY);
                    }else
                        grid[neighborX, neighborY].Reveal();
            }
        }

    }
}
