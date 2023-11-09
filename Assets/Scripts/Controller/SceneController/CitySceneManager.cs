using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySceneManager : MonoBehaviour
{
    public static CitySceneManager Instance { get; private set; } // static singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);
    }
}
