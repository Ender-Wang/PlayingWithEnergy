using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QandAAnswerButton : MonoBehaviour
{
    [Header("Metrics")]
    public TextMeshProUGUI solution;
    public TextMeshProUGUI point;

    public Button thisButton;
    public Button theOtherButton;

    public TextMeshProUGUI thisButtonText;
    public TextMeshProUGUI theOtherButtonText;

    public Image thisButtonImage;
    public Image theOtherButtonImage;

    public GameObject currentQandAItem;

    public void CheckAnswer()
    {
        thisButton.interactable = false;
        theOtherButton.interactable = false;
        if (thisButtonText.text == solution.text)
        {
            thisButtonImage.color = Color.green;
            theOtherButtonImage.color = Color.grey;
            // Debug.Log("credit in button: " + credit.text);
            QandAManager.Instance.credits.Add(int.Parse(point.text));
        }
        else
        {
            thisButtonImage.color = Color.red;
            theOtherButtonImage.color = Color.grey;
            QandAManager.Instance.credits.Add(0);
        }
        Invoke("hideCurrentQandA", 1.5f);
    }

    /// <summary>
    /// Hide the current QandA 3 seconds after answering
    /// </summary>
    public void hideCurrentQandA(){
        currentQandAItem.SetActive(false);
    }
}
