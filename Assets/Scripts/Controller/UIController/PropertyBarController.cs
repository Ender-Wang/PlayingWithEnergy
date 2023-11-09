using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Update the property bar in time
/// </summary>
public class PropertyBarController : MonoBehaviour
{
    public TextMeshProUGUI moneyBar;
    public TextMeshProUGUI energyBar;
    public TextMeshProUGUI crystalBar;

    private float m_money;
    private int m_energy;
    private int m_crystal;

    void start()
    {
        m_money = GameManager.Instance.money;
        m_energy = GameManager.Instance.energy;
        m_crystal = GameManager.Instance.crystal;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_money != GameManager.Instance.money)
        {
            if (moneyBar != null) moneyBar.text = GameManager.Instance.money.ToString("N0");
            m_money = GameManager.Instance.money;
        }
        if (m_energy != GameManager.Instance.energy)
        {
            if (energyBar != null) energyBar.text = GameManager.Instance.energy.ToString("N0");
            m_energy = GameManager.Instance.energy;
        }
        if (m_crystal != GameManager.Instance.crystal)
        {
            if (crystalBar != null) crystalBar.text = GameManager.Instance.crystal.ToString("N0");
            m_crystal = GameManager.Instance.crystal;
        }
    }
}
