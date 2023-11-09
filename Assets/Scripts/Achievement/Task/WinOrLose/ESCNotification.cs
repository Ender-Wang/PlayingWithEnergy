using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ESCNotification : MonoBehaviour
{

    public static void OpenESCNotification()
    {
        setESCNotificationBoardState(true);
    }

    public static void CloseESCNotification()
    {
        setESCNotificationBoardState(false);
    }

    public static void setESCNotificationBoardState(bool state)
    {
        Scene scene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject ob in rootObjects)
        {
            if (ob.name == "Canvas")
            {
                Transform[] canvasObjects = ob.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform go in canvasObjects)
                {
                    if (go.gameObject.name == "ESC Notification Board")
                    {
                        go.gameObject.SetActive(state);
                    }
                }
            }
        }
    }
}
