using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class IntefaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lobbypool = null;

    [HideInInspector]
    public static IntefaceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instance of IntefaceManager");
        }
    }


    public void ActivateLobby(int index)
    {
        foreach (var item in lobbypool) 
        {
            item.SetActive(false);
        }
        lobbypool[index].SetActive(true);
    }


}
