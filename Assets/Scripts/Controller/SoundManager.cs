using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;

    private float volume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            volume = ES3.Load<float>("Volume", "Player/Settings", 0.5f);
            AudioListener.volume = volume;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        AudioListener.volume = volume;
    }

    public float GetVolume()
    {
        return volume;
    }

    public void Save()
    {
        ES3.Save<float>("Volume", volume, "Player/Volume");
    }
}
