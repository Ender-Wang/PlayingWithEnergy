using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class EnergyBarCalculator : MonoBehaviour
{
    public static EnergyBarCalculator Instance;

    [SerializeField] GameObject bubblePrefab;
    [SerializeField] GameObject bubbleParent;

    public int energy;

    private Animator animator;
    private GameObject bubble;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            energy = ES3.Load<int>("energy", "Player/Time", 0);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        bubble = Instantiate(bubblePrefab, bubbleParent.transform);
        bubble.SetActive(false);
    }

    /// <summary>
    /// Add energy to the player.
    /// </summary>
    public void IncreaseEnergy()
    {
        energy = BuildingManager.Instance.GetTotalEnergyProvision();
        GameManager.Instance.AddEnergy(energy, false);

        // show the bubble
        bubble.SetActive(true);
        bubble.GetComponent<Image>().color = new Color(0.05098039f, 0.827451f, 1f);
        animator = bubble.GetComponent<Animator>();
        bubble.GetComponentInChildren<TextMeshProUGUI>().text = (energy >= 0 ? "+" : "") + energy.ToString("N0");
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
        ES3.Save<int>("energy", energy, "Player/Time");
    }

}
