using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoneyBarCalculator : MonoBehaviour
{
    public static MoneyBarCalculator Instance;

    [SerializeField] GameObject bubblePrefab;
    [SerializeField] GameObject bubbleParent;

    public float moneyAmount { get; set; }
    public float multiplier { get; set; }
    public float baseAmount { get; set; }

    private Animator animator;
    private GameObject bubble;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        baseAmount = 1000;
        multiplier = 1000;
        bubble = Instantiate(bubblePrefab, bubbleParent.transform);
        bubble.SetActive(false);
        moneyAmount = ES3.Load<float>("money", "Player/Time", baseAmount);
    }

    public void IncreaseMoney()
    {
        moneyAmount = baseAmount + GameManager.Instance.tempScore * multiplier;
        GameManager.Instance.AddMoney(moneyAmount, false);
        // show the bubble
        bubble.SetActive(true);
        animator = bubble.GetComponent<Animator>();
        bubble.GetComponentInChildren<TextMeshProUGUI>().text = (moneyAmount >= 0 ? "+" : "") + moneyAmount.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubble.GetComponent<RectTransform>());
        StartCoroutine(RunNotificationSequence(0.5f));
    }

    private IEnumerator RunNotificationSequence(float duration)
    {
        animator?.SetTrigger("Open");
        yield return new WaitForSeconds(duration);
        animator?.SetTrigger("Close");
        yield return new WaitForSeconds(1.0f);
        bubble.SetActive(false);
    }

    public void Save()
    {
        ES3.Save<float>("money", moneyAmount, "Player/Time");
    }
}
