using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitFirstPersonMode : MonoBehaviour
{
    private GameObject exitTaskPanel;   // Exit Task Panel involved on escape key pressed
    private GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //find Exit Task Panel in GameUIScene and set it active
            Scene GameUIScene = SceneManager.GetSceneByName("GameUIScene");
            GameObject[] gameUIObjects = GameUIScene.GetRootGameObjects();
            foreach (GameObject ob in gameUIObjects)
            {
                if (ob.name == "Canvas")
                {
                    Transform canvas = ob.transform;
                    for (int i = 0; i < canvas.childCount; i++)
                    {
                        if (canvas.GetChild(i).name == "Prefabs")
                        {

                            Transform prefabs = canvas.GetChild(i);
                            for (int j = 0; j < prefabs.childCount; j++)
                            {
                                if (prefabs.GetChild(j).name == "Exit Task Panel")
                                {
                                    exitTaskPanel = prefabs.GetChild(j).gameObject;
                                    exitTaskPanel.SetActive(true);
                                    //Close ESC Notification Board
                                    ESCNotification.CloseESCNotification();
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
