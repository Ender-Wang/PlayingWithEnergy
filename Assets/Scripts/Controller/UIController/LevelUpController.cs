using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Level Up UI Pop Up
/// </summary>
public class LevelUpController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI originLevel;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI coinAward;
    [SerializeField] private TextMeshProUGUI energyAward;
    [SerializeField] private TextMeshProUGUI coinPackNumber;
    [SerializeField] private TextMeshProUGUI energyPackNumber;

    // Start is called before the first frame update
    void Start()
    {
        originLevel.text = "Level " + (GameManager.Instance.level - 1).ToString();
        level.text = "Level " + GameManager.Instance.level.ToString();
        int coinPack = GameManager.Instance.level + Random.Range(0, 5);
        int energyPack = GameManager.Instance.level + Random.Range(0, 2);
        coinPackNumber.text = coinPack.ToString();
        energyPackNumber.text = energyPack.ToString();
        coinAward.text = "+ " + (coinPack * 10000).ToString("N0");
        energyAward.text = "+ " + (energyPack * 1000).ToString("N0");
        GameManager.Instance.AddMoney(coinPack * 10000);
        GameManager.Instance.AddEnergy(energyPack * 1000);
        LayoutRebuilder.ForceRebuildLayoutImmediate(originLevel.transform.parent.GetComponent<RectTransform>());
    }

}
