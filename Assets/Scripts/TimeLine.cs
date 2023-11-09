using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AzureSky;
using System;

public class TimeLine : MonoBehaviour
{
    public static TimeLine Instance { get; private set; } // static singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            time = ES3.Load<float>("time", "Player/Time", 7f);
            lastQuitTime = ES3.Load<DateTime>("lastQuitTime", "Player/Time", DateTime.Now);
        }

        if (Instance != this)
            Destroy(gameObject);
    }


    public int updateFrequency = 15;       // seconds
    [SerializeField] GameObject nightLight;           // Light in the night 
    private float timer;                 // Timer
    public float time { get; set; }                 // total time in the game

    [ES3Serializable]
    public DateTime lastQuitTime { get; set; }       // last quit time


    // Start is called before the first frame update
    void Start()
    {
        // Initialize the timer
        timer = updateFrequency;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update the timer
        timer -= Time.deltaTime;

        // When the timer reaches 1 minute
        if (timer <= 0f)
        {
            // Increase energy
            EnergyBarCalculator.Instance?.IncreaseEnergy();

            // Increase money
            MoneyBarCalculator.Instance?.IncreaseMoney();


            // Reset the timer
            timer = updateFrequency;

        }

        if (time > 18f || time < 6f)
        {
            nightLight.SetActive(true);
        }
        else
        {
            nightLight.SetActive(false);
        }


    }

    public void SaveTimeStamp()
    {
        ES3.Save<float>("time", time, "Player/Time");
        ES3.Save<DateTime>("lastQuitTime", DateTime.Now, "Player/Time");
        EnergyBarCalculator.Instance?.Save();
        MoneyBarCalculator.Instance?.Save();
    }
}
