using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool isOpen = false;
    public bool isMine = false;
    public bool isFlag = false;
    public bool isBlowen = false;
    [SerializeField] private int x, y;
    public int mineCount = -1;
    private GameObject model;
    [SerializeField] GameObject blank;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject blow;

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

    public void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            if (Input.GetMouseButtonUp(0))
            {
                if (mineCount == -1)
                    GridManager.Instance.CreateGrid(x, y);
                GridManager.Instance.OpenTile(x, y);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                GridManager.Instance.SetFlag(x, y);
            }

    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            if (isOpen)
                model.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            else
                blank.transform.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    private void OnMouseExit()
    {

        if (isOpen)
            model.transform.position = transform.position;
        else
            blank.transform.position = transform.position;
    }


    public void Reveal()
    {
        isOpen = true;
        blank.SetActive(false);
        model.SetActive(true);
        SetFlag(false);
        Debug.Log($"Tile at {x}, {y} revealed. and there are {mineCount} mines around");
    }

    public void SetFlag(bool state)
    {
        flag.SetActive(state);
        isFlag = state;
    }

    public void UseBlown()
    {
        if (isMine)
        {
            var buff = model;
            model = Instantiate(blow, transform); 
            model.SetActive(buff.activeSelf);
            Destroy(buff);
            isBlowen = true;
        } 
        Reveal();
    }

}
