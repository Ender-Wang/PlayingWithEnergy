using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QandACameraSwitch : MonoBehaviour
{
    public static QandACameraSwitch Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Switch camera to QandA setup scenario
    /// </summary>
    public void switchToQandACamera()
    {
        setAudioListenerInGameUIScene(false);
        setQandASetup(true);
    }

    /// <summary>
    /// Switch camera to original scenario
    /// </summary>
    public void switchToOriginalCamera()
    {
        setAudioListenerInGameUIScene(true);
        setQandASetup(false);
    }

    /// <summary>
    /// Set audio listener on or off in GameUIScene
    /// </summary>
    /// <param name="status">Status of the audio listener {true, false}</param>
    private void setAudioListenerInGameUIScene(bool status)
    {
        Scene gameUIScene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] gameUISceneObjects = gameUIScene.GetRootGameObjects();
        foreach (GameObject ob in gameUISceneObjects)
        {
            if (ob.name == "GameUI Camera")
            {
                AudioListener audioListener = ob.GetComponent<AudioListener>();
                audioListener.enabled = status;
            }
        }
    }

    /// <summary>
    /// Set QandA Setup active or inactive in CityScene
    /// </summary>
    /// <param name="status">Status of the QandA Setup scenario</param>
    private void setQandASetup(bool status)
    {
        Scene cityScene = SceneManager.GetSceneByName("CityScene");
        GameObject[] citySceneObjects = cityScene.GetRootGameObjects();
        foreach (GameObject ob in citySceneObjects)
        {
            if (ob.name == "QandA Setup")
            {
                ob.SetActive(status);
            }
        }
    }
}
