using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EnterThirdPersonView : MonoBehaviour
{
    public Camera gameUICamera;

    public void switchToThirdPersonView()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        gameUICamera.gameObject.SetActive(false);
        Scene cityScene = SceneManager.GetSceneByName("CityScene");
        GameObject[] citySceneObjects = cityScene.GetRootGameObjects();
        foreach (GameObject ob in citySceneObjects)
        {
            if (ob.name == "FPS")
            {
                // get the child object which name is FPS Camera of FPS
                GameObject FPSCamera = ob.transform.Find("Player Camera").gameObject;
                FPSCamera.SetActive(true);
                // disable other buttons
                Transform buttons = canvas.transform.Find("Buttons");
                for (int i = 0; i < buttons.childCount; i++)
                {
                    if (buttons.GetChild(i).name == "Buttons Bottom Left" || buttons.GetChild(i).name == "Buttons Bottom Right")
                        continue;
                    buttons.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
