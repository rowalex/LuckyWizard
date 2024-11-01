using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOpen = false;
    public bool isMine = false;
    [SerializeField] private int x, y;
    public int mineCount = -1;
    private GameObject model;
    [SerializeField] GameObject blank;

    public void SetupParams(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void SetupParams(bool ismine)
    {
        this.isMine = ismine;
    }
    public void SetupParams(int mineCount)
    {
        this.mineCount = mineCount;
    }
    
    public void SetupObject(GameObject model)
    {
        this.model = Instantiate(model, transform); this.model.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (mineCount == -1)
            GridManager.Instance.CreateGrid(x, y);
        GridManager.Instance.OpenTile(x, y);
    }

    private void OnMouseEnter()
    {
        blank.transform.position += new Vector3(0, 0.5f, 0);
    }

    private void OnMouseExit()
    {
        blank.transform.position += new Vector3(0, -0.5f, 0);
    }


    public void Reveal()
    {
        isOpen = true;
        blank.SetActive(false);
        model.SetActive(true);
        Debug.Log($"Tile at {x}, {y} revealed. and there are {mineCount} mines around");
    }
}
