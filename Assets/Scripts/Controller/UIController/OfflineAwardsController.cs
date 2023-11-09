using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Offline awards UI Pop Up
/// </summary>
public class OfflineAwardsController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI offlineTimeText;
    [SerializeField] TextMeshProUGUI coinAward;
    [SerializeField] TextMeshProUGUI energyAward;

    // Start is called before the first frame update
    void Start()
    {
        offlineTimeText.text = GameManager.Instance.offlineInterval.Days.ToString() + "d " + GameManager.Instance.offlineInterval.Hours.ToString() + "h " + GameManager.Instance.offlineInterval.Minutes.ToString() + "m " + GameManager.Instance.offlineInterval.Seconds.ToString() + "s ";
        coinAward.text = "+ " + GameManager.Instance.offlineAwards["Money"].ToString("N0");
        energyAward.text = "+ " + GameManager.Instance.offlineAwards["Energy"].ToString("N0");
        GameManager.Instance.AddMoney(GameManager.Instance.offlineAwards["Money"]);
        GameManager.Instance.AddEnergy(GameManager.Instance.offlineAwards["Energy"]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
