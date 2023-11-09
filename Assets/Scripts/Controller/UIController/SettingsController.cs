using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicPercentage;

    float volume;

    // Start is called before the first frame update
    void Start()
    {
        volume = SoundManager.Instance.GetVolume();
        musicSlider.value = volume;
        int percentage = (int)(volume * 100);
        musicPercentage.text = percentage.ToString();
    }

    public void BackToHomepage()
    {
        StartCoroutine(GameManager.Instance.Save());
        StartCoroutine(WaitForSavingBack());
    }

    public void ChangeVolume()
    {
        SoundManager.Instance.ChangeVolume(musicSlider.value);
        musicPercentage.text = (musicSlider.value * 100).ToString("0");
    }

    public void SaveSettings()
    {
        volume = musicSlider.value;
        SoundManager.Instance.ChangeVolume(musicSlider.value);
    }

    public void ResetSettings()
    {
        musicSlider.value = volume;
        SoundManager.Instance.ChangeVolume(musicSlider.value);
    }

    /// <summary>
    /// Save the game and wait for the saving process to finish and then back to the start scene
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForSavingBack()
    {
        while (GameManager.Instance.isSaved == false)
        {
            yield return null;
        }
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("StartScene");
    }
}
