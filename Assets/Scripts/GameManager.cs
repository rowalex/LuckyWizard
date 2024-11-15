using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance;

    [SerializeField]
    private Transform startLobbyTarget;
    [SerializeField]
    private Transform LobbyTarget;
    [SerializeField]
    private Transform GameplayTarget;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        StartStartLobby();
        SoundManager.Instance.SetBackGroundMusic("background");
    }


    public void StartStartLobby()
    {
        CameraController.Instance.SetCamera(new Vector3(-47.3f, 1.36f, -53.8f),startLobbyTarget);
        IntefaceManager.Instance.ActivateLobby(0);
    }

    public void StartLobby()
    {
        CameraController.Instance.SetCamera(new Vector3(-50.3f, 1f, -52.5f), LobbyTarget);
        IntefaceManager.Instance.ActivateLobby(1);
    }

    public void StartSettings()
    {
        CameraController.Instance.SetCamera(new Vector3(-50.3f, 1f, -52.5f), LobbyTarget);
        IntefaceManager.Instance.ActivateLobby(5);
    }


    public void StartGame()
    {
        GridManager.Instance.CreateEmptyGrid();
        CameraController.Instance.StartGameCamera(GameplayTarget);
        IntefaceManager.Instance.ActivateLobby(2);
    }

    public void EndGame()
    {
        SoundManager.Instance.Play("lose");
        IntefaceManager.Instance.ActivateLobby(4);
    }

    public void WinGame()
    {
        SoundManager.Instance.Play("win");
        IntefaceManager.Instance.ActivateLobby(3);
    }

    public void CloseApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }
}

