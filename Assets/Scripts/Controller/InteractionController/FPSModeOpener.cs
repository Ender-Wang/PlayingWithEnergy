using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSModeOpener : MonoBehaviour
{
    [SerializeField] private GameObject trees;
    [SerializeField] private GameObject citizens;

    private void OnEnable()
    {
        trees.SetActive(true);
        citizens.SetActive(true);
    }

    private void OnDisable()
    {
        trees.SetActive(false);
        citizens.SetActive(false);
    }
}
