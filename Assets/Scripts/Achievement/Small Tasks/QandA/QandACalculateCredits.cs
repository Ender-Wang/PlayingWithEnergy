using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UltimateClean;

public class QandACalculateCredits : MonoBehaviour
{
    public Button finishButton;
    public Image finishButtonColor;

    void Update()
    {
        if (QandAManager.Instance.currentQandAPopup != null)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                {
                    DeactivateESCNotificationBoard();
                    TimeLimitationManager.Instance.ActivateTimerBoard(false);
                    QandAManager.Instance.currentQandAPopup.GetComponent<Popup>().Close();
                    QandACameraSwitch.Instance.switchToOriginalCamera();
                }
            }
    }
    public void calculateCredits()
    {
        //Close Q&A panel
        QandAManager.Instance.currentQandAPopup.transform.GetChild(1).gameObject.SetActive(false);

        List<QandAItem> qandAItems = new List<QandAItem>();
        foreach (QandAItem QandAItem in QandAManager.Instance.QandAItems)
        {
            //calculate the total credit(s) gained from task with id = 1
            if (QandAItem.id == 1)
            {
                qandAItems.Add(QandAItem);
            }
        }
        int sum = 0;
        foreach (int credit in QandAManager.Instance.credits)
        {
            sum += credit;
        }
        if (sum > qandAItems.Count / 2)
        {
            TaskCompletionManager.Instance.taskCompleted(4);
        }
        else
        {
            TaskCompletionManager.Instance.taskFailed("To complete this task, you need to get " + qandAItems.Count / 2 + " out of " + qandAItems.Count + " questions correct. You got " + sum + " correct.");
        }
        // Debug.Log("credits count: " + QandAManager.Instance.credits.Count);

        //clear the list of credits after calculating
        QandAManager.Instance.credits.Clear();
        finishButton.interactable = false;
        finishButtonColor.color = Color.grey;
    }
    void DeactivateESCNotificationBoard()
    {
        //Deactivate ESC Notification Board
        Scene GameUIScene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] GameUISceneRootObjects = GameUIScene.GetRootGameObjects();
        foreach (GameObject ob in GameUISceneRootObjects)
        {
            if (ob.name == "Canvas")
            {
                //deactivate ESC board in canvas
                Transform canvas = ob.transform;
                for (int i = 0; i < canvas.childCount; i++)
                {
                    if (canvas.GetChild(i).name == "ESC Notification Board")
                    {
                        canvas.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
