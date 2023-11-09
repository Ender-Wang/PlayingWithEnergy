using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TimeLimitationManager : MonoBehaviour
{
    public static TimeLimitationManager Instance;
    public TextMeshProUGUI timeRemainingText;
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
    /// Start a timer for a task
    /// </summary>
    /// <param name="minutes">Time in minutes</param>
    public void StartTaskTimer(int minutes)
    {
        StopAllCoroutines();
        // Activate timer board
        ActivateTimerBoard(true);
        int seconds = minutes * 60;
        StartCoroutine(Timer(seconds));
    }

    public void ActivateTimerBoard(bool status)
    {
        Scene GameUIScene = SceneManager.GetSceneByName("GameUIScene");
        GameObject[] GameUISceneRootObjects = GameUIScene.GetRootGameObjects();
        foreach (GameObject ob in GameUISceneRootObjects)
        {
            if (ob.name == "Canvas")
            {
                Transform canvas = ob.transform;
                for (int i = 0; i < canvas.childCount; i++)
                {
                    if (canvas.GetChild(i).name == "TimeLimit Board")
                    {
                        canvas.GetChild(i).gameObject.SetActive(status);
                        if (!status)
                        {
                            StopTaskTimer();
                        }
                    }
                }
            }
        }
    }

    public void StopTaskTimer()
    {
        StopAllCoroutines();
    }

    IEnumerator Timer(int seconds)
    {
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1);
            seconds--;
            timeRemainingText.text = " " + seconds / 60 + ":" + seconds % 60 + " mins";
        }
        // When time is up, close the timer board, and task failed
        ActivateTimerBoard(false);
        TaskCompletionManager.Instance.taskFailed("Time is up! Speed up next time!");
        StopAllCoroutines();
    }
}
