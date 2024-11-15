using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

[Serializable]
struct MusicFile
{
    public string name;
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool loop;
} 

public class SoundManager : MonoBehaviour
{
    [HideInInspector]
    public static SoundManager Instance;

    [SerializeField]
    private MusicFile[] media;

    [SerializeField]
    private AudioSource player;
    private float player_base = 1;
    [SerializeField]
    private AudioSource backplayer;
    private float backplayer_base = 1;

    [SerializeField] private float global_volume = 1; 



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Play(string name)
    {
        MusicFile curfile = new MusicFile();
        for (int i = 0; i < media.Length; i++)
        {
            if (media[i].name == name) 
            {
                curfile = media[i];
                break;
            }
        }
        if (curfile.name == null)
        {
            Debug.Log($"нет такого файла {name}");
            return;
        }

        player.clip = curfile.clip; 
        player_base = curfile.volume;
        player.volume = player_base * global_volume;
        player.pitch = curfile.pitch;
        player.loop = curfile.loop;
        player.Play();

    }
    public void SetBackGroundMusic(string name)
    {
        MusicFile curfile = new MusicFile();
        for (int i = 0; i < media.Length; i++)
        {
            if (media[i].name == name)
            {
                curfile = media[i];
                break;
            }
        }
        if (curfile.name == null)
        {
            Debug.Log($"нет такого файла {name}");
            return;
        }

        backplayer.clip = curfile.clip;
        backplayer_base = curfile.volume;
        backplayer.volume = backplayer_base * global_volume;
        backplayer.pitch = curfile.pitch;
        backplayer.loop = curfile.loop;
        backplayer.Play();

    }

    public void SetVolume(Scrollbar bar)
    {
        global_volume  = Math.Clamp(bar.value, 0, 1);

        player.volume = player_base * global_volume;
        backplayer.volume = backplayer_base * global_volume;
    }

}
