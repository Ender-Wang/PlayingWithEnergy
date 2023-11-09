using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitTaskPanel : MonoBehaviour
{
    public GameObject exitTaskPanel;
    // public Button noButton;
    // public Button yesButton;

    /// <summary>
    /// Resume back to task after clicking no
    /// </summary>
    public void resumeTask()
    {
        exitTaskPanel.SetActive(false); //disable exit task panel
    }

    /// <summary>
    /// Exit task after clicking yes
    /// </summary>
    public void exitTask()
    {
        //Disable TimeLimitBoard
        TimeLimitationManager.Instance.ActivateTimerBoard(false);

        //disable Look & Move control script when leaving task or just disable Player if no interaction required after exiting the Task
        GameObject player = GameObject.Find("Player Camera");
        // player.GetComponent<PlayerMovement>().enabled = false;  //disable movement control
        // player.transform.GetChild(0).GetChild(0).GetComponent<MouseLook>().enabled = false; //disable mouse look control
        player.SetActive(false);    //disable player

        exitTaskPanel.SetActive(false); //disable exit task panel

        // re-enable selecting controller when leaving task
        SelectingController.Instance.enabled = true;

        //load GameUI scene when leaving task
        Scene GameUIScene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] gameUIObjects = GameUIScene.GetRootGameObjects();

        // De/activate components when leaving task
        foreach (GameObject ob in gameUIObjects)
        {
            //switch on GameUI camera
            Camera camera = ob.GetComponent<Camera>();
            if (camera != null && camera.name == "GameUI Camera")
            {
                // Debug.Log("camera name: " + camera.name);
                camera.gameObject.SetActive(true);
                // enable all buttons
                Transform buttons = GameManager.Instance.canvas.transform.Find("Buttons");
                for (int k = 0; k < buttons.childCount; k++)
                {
                    buttons.GetChild(k).gameObject.SetActive(true);
                }
                GameManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            //Destroy TaskList in GameUIScene
            if (ob.name == "Canvas")
            {
                foreach (Transform child in ob.transform)
                {
                    if (child.name == "TaskList")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        Scene CityScene = SceneManager.GetSceneByName("CityScene");
        GameObject[] cityObjects = CityScene.GetRootGameObjects();
        foreach (GameObject ob in cityObjects)
        {
            if (ob.name.Contains("Task-"))
            {
                Destroy(ob);
            }
        }
    }
}
