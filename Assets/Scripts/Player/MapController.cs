using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mapCanvas;
    public BinnacleScript binnacleScript;
    public int minRadius = 3;
    public int maxRadius = 20;
    void Start()
    {
        if ( mapCanvas == null)
        {   
            mapCanvas = transform.GetChild(0).gameObject;
        }
        if (binnacleScript == null)
        {
            binnacleScript = GameObject.FindObjectOfType<BinnacleScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // press m to open/close map
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapCanvas.SetActive(!mapCanvas.activeSelf);
        }
    
    }

    public void MapBigger()
    {   
        binnacleScript.radarRadius = Math.Max(minRadius, binnacleScript.radarRadius - 3);
    }

    public void MapSmaller()
    {
        binnacleScript.radarRadius = Math.Min(maxRadius, binnacleScript.radarRadius + 3);
    }
}
