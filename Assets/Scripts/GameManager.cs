using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        GridManager.Instance.CreateEmptyGrid();
    }

    public void EndGame()
    {

    }
}

