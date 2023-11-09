using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UltimateClean;

public class StartTask : MonoBehaviour
{
    public TextMeshProUGUI taskID;
    private Vector3[] airdropLocations;

    public TextMeshProUGUI coinRewardAmount;
    public TextMeshProUGUI levelFactorPointRewardAmount;
    public TextMeshProUGUI timeLimitText;

    public void startTask()
    {
        //Start timer
        int timeLimit = int.Parse(timeLimitText.text.Substring(0, timeLimitText.text.Length - 5));
        TimeLimitationManager.Instance.StartTaskTimer(timeLimit);

        //Hide achievement board popup when start task
        AchievementBoardManager.Instance.currentAchievementBoardPopup.GetComponent<Popup>().Close();

        // Prepare player airdrop locations
        int tID = int.Parse(taskID.text);

        // task with taskID 4 is the Q&A task, which is not a task that requires the player to find an NPC
        if (tID != 4)
        {
            //Switch camera
            enabledPlayerCamera();

            foreach (TaskItem task in TaskManager.Instance.taskItems)
            {
                if (task.taskID == tID)
                {
                    airdropLocations = task.taskLocation;
                }
            }

            //Hide other UI elements
            Transform buttons = GameManager.Instance.canvas.transform.Find("Buttons");
            for (int i = 0; i < buttons.childCount; i++)
            {
                buttons.GetChild(i).gameObject.SetActive(false);
            }

            //Prepare taskID, airdropLocation, taskSpecification, coinRewardAmount, levelFactorPointRewardAmount
            Vector3 airdropLocation = airdropLocations[Random.Range(0, airdropLocations.Length)];
            string taskSpecificationData = TaskSpecificationManager.Instance.GetTaskSpecification(tID);
            // Debug.Log("taskSpecificationData in StartTask: " + taskSpecificationData);
            int coinRewardAmountData = 0;
            int levelFactorPointRewardAmountData = 0;

            new ActivateFindNPCTask().InitNPCTaskN(tID, airdropLocation, taskSpecificationData, coinRewardAmountData, levelFactorPointRewardAmountData);
        }
        else
        {
            QandABoardPopupOpener.Instance.OpenPopup();
            QandACameraSwitch.Instance.switchToQandACamera();
        }
    }

    /// <summary>
    /// Enable Player Camera and disable GameUI Camera.
    /// </summary>
    public static void enabledPlayerCamera()
    {
        Scene cityScene = SceneManager.GetSceneByName("CityScene");
        GameObject[] citySceneObjects = cityScene.GetRootGameObjects();
        foreach (GameObject ob in citySceneObjects)
        {
            if (ob.name == "FPS")
            {
                Transform[] fpsObjects = ob.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform go in fpsObjects)
                {
                    if (go.gameObject.name == "Player Camera")
                    {
                        go.gameObject.SetActive(true);
                    }
                }
            }
        }

        Scene gameUIScene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] gameUISceneObjects = gameUIScene.GetRootGameObjects();
        foreach (GameObject ob in gameUISceneObjects)
        {
            if (ob.name == "GameUI Camera")
            {
                ob.SetActive(false);
            }
        }
    }


}
