using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltimateClean;
using TMPro;

public class LoadScene : MonoBehaviour
{
    [SerializeField] SlicedFilledImage slicedImage;
    [SerializeField] TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Load the city scene and the UI scene asynchronously
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadYourAsyncScene()
    {
        yield return new WaitForSeconds(1.0f);
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation asyncLoadCity = SceneManager.LoadSceneAsync("CityScene", LoadSceneMode.Additive);
        asyncLoadCity.allowSceneActivation = false;

        while (asyncLoadCity.progress < 0.9f)
        {
            toProgress = (int)(asyncLoadCity.progress * 100) / 2;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                if (slicedImage != null)
                    slicedImage.fillAmount = displayProgress / 100.0f;
                var percentage = (int)(displayProgress / 100.0f * 100);
                text.text = percentage.ToString() + "%";
                yield return new WaitForEndOfFrame();
            }
        }
        asyncLoadCity.allowSceneActivation = true;
        var temp = toProgress;
        AsyncOperation asyncLoadUI = SceneManager.LoadSceneAsync("GameUIScene", LoadSceneMode.Additive);
        asyncLoadUI.allowSceneActivation = false;

        while (asyncLoadUI.progress < 0.9f)
        {
            toProgress = temp + (int)(asyncLoadUI.progress * 100) / 2;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                if (slicedImage != null)
                    slicedImage.fillAmount = displayProgress / 100.0f;
                var percentage = (int)(displayProgress / 100.0f * 100);
                text.text = percentage.ToString() + "%";
                yield return new WaitForEndOfFrame();
            }
        }

        while (!CitySceneManager.Instance) // city scene is not able to active
            yield return null;

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            if (slicedImage != null)
                slicedImage.fillAmount = displayProgress / 100.0f;
            var percentage = (int)(displayProgress / 100.0f * 100);
            text.text = percentage.ToString() + "%";
            yield return new WaitForEndOfFrame();
        }
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName("CityScene"));
        asyncLoadUI.allowSceneActivation = true;
    }
}
