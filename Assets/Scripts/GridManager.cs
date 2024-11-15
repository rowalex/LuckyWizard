using OpenCover.Framework.Model;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    [Header("PARAMS")]
    [SerializeField] private int width;
    [SerializeField] private float deltawidth;
    [SerializeField] private int height;
    [SerializeField] private float deltaheight;
    [SerializeField] private int mineCount;
    [SerializeField] private int skillcount = 1;
    [Header("GAME")]
    [SerializeField] private Text timerUI;
    [SerializeField] private float timerTime = 0;
    [SerializeField] private Text mine_countUI;
    [SerializeField] private int curminecount;
    [SerializeField] private int flagcount = 0;
    [SerializeField] private int cellsLeft;
    [SerializeField] private Text skillcountUI;
    [SerializeField] private int curskillcount = 1;
    [SerializeField] private bool skilltoggle = false;


    enum GamingState
    {
        win, lose, playing
    }

    private GamingState state;



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

        timerTime = 0;

        mine_countUI.text = mineCount.ToString();
        curminecount = mineCount;

        curskillcount = skillcount;
        skillcountUI.text = curskillcount.ToString();
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

    public void OpenTile(int x, int y, bool safe = true)
    {
        if (state == GamingState.playing)
        {
            var tile = grid[x, y];

            if (tile.isFlag)
                return;

            if (skilltoggle && !tile.isOpen)
            {
                Skill(x, y);
                return;
            }

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
            else if (safe)
                TryOpenAllAround(x, y);
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
        if (cellsLeft <= curminecount)
        {
            state = GamingState.win;
            mine_countUI.text = 0.ToString();
            GameManager.Instance.WinGame();
        }
           
    }

    public void SetFlag(int x, int y, bool safe = true)
    {
        if (state == GamingState.playing)
        {
            var tile = grid[x, y];

            if (tile.isOpen)
            {
                if (safe)
                    TrySetFlagAllAround(x, y);

                return;
            }

            if (tile.isFlag)
            {
                tile.SetFlag(false);
                flagcount--;
            }
            else
            {
                tile.SetFlag(true);
                flagcount++;
            }

            mine_countUI.text = Mathf.Clamp((curminecount - flagcount), 0, mineCount).ToString();

        }
    }

    public void TryOpenAllAround(int x, int y)
    {
        Debug.Log($"try open all around for {x} {y}");
        if (grid[x, y].mineCount == 0)
            return;

        int flag_count = 0;
        var tiles = new List<(int, int)>();


        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = x + dx;
                int neighborY = y + dy;
                if (dx == 0 && dy == 0) continue;
                else if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    var ad_tile = grid[x + dx, y + dy];
                    if (!ad_tile.isOpen)
                    {
                        tiles.Add((x + dx, y + dy));
                    }
                    if (ad_tile.isFlag || ad_tile.isBlowen)
                    {
                        flag_count++;
                        tiles.Remove((x + dx, y + dy));
                    }
                }
            }
        }
        Debug.Log($"flags {flag_count} and minecount is {grid[x,y].mineCount}: total {tiles.Count}s");

        if (flag_count == grid[x, y].mineCount)
            for (int i = 0; i < tiles.Count; i++)
                OpenTile(tiles[i].Item1, tiles[i].Item2, false);
    }

    public void TrySetFlagAllAround(int x, int y)
    {
        Debug.Log($"try Set Flag all around for {x} {y}");
        if (grid[x, y].mineCount == 0)
            return;

        int flag_count = 0;
        int close_count = 0;
        var tiles = new List<(int, int)>();


        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = x + dx;
                int neighborY = y + dy;
                if (dx == 0 && dy == 0) continue;
                else if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    var ad_tile = grid[x + dx, y + dy];
                    if (!ad_tile.isOpen)
                    {
                        close_count++;
                        tiles.Add((x + dx, y + dy));
                    }
                    if (ad_tile.isFlag || ad_tile.isBlowen)
                    {
                        flag_count++;
                        tiles.Remove((x + dx, y + dy));
                    }
                }
            }
        }

        Debug.Log($"there are {close_count - flag_count} close and minecount is {grid[x, y].mineCount}: total {tiles.Count}");
        if (close_count - flag_count == grid[x, y].mineCount)
        {
            for (int i = 0; i < tiles.Count; i++)
                SetFlag(tiles[i].Item1, tiles[i].Item2, false);
        }
    }

    public void ToggleSkill()
    {
        if (!skilltoggle && curskillcount > 0)
        {
            var bob = skillcountUI.gameObject.GetComponent<Bobbing>();

            bob.enabled = true;
            skilltoggle = true;
        }
        else
        {
            var bob = skillcountUI.gameObject.GetComponent<Bobbing>();
            bob.enabled = false;
            skilltoggle = false;
        }
    }


    private void Update()
    {
        if (state == GamingState.playing)
            timerTime += Time.deltaTime;
        timerUI.text = timerTime.ToString(".00");

        if (Input.GetKeyUp(KeyCode.Space))
            ToggleSkill();
    }

    public void Skill(int x , int y)
    {
        ToggleSkill();
        curskillcount--;
        skillcountUI.text = curskillcount.ToString();
        Tile tile = grid[x, y];
        if (tile.isMine)
        {
            tile.UseBlown();
            curminecount--;
            mine_countUI.text = Mathf.Clamp((curminecount - flagcount), 0, mineCount).ToString();
            ReduceUnknownCells();
        }else
            OpenTile(x, y);

    }

}
