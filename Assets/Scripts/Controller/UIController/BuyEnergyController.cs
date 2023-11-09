using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Link to the energy buying objects.
/// </summary>
public class BuyEnergyController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI energyAmount;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] GameObject animationPrefab;
    [SerializeField] Transform animationStartPosition;

    int m_price;
    int m_energyAmount;

    // Start is called before the first frame update
    void Start()
    {
        m_energyAmount = int.Parse(energyAmount.text.Replace(",", ""));
        m_price = int.Parse(price.text.Replace(",", ""));
    }

    public void Buy()
    {
        if (GameManager.Instance.AddMoney(-m_price))
        {
            GameManager.Instance.AddEnergy(m_energyAmount);
            Instantiate(animationPrefab, GameManager.Instance.canvas.transform).transform.position = animationStartPosition.position;
        }
    }
}
