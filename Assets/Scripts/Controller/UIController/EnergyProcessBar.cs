using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyProcessBar : MonoBehaviour
{
    public static EnergyProcessBar Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple EnergyProcessBar Instance!");
        }
    }

    [SerializeField] GameObject energyProcessBar;
    [SerializeField] TextMeshProUGUI energyConsumptionText;
    [SerializeField] TextMeshProUGUI energyProvisionText;
    [SerializeField] GameObject consumptionToolTip;
    [SerializeField] GameObject provisionToolTip;
    [SerializeField] GameObject semanticPercentagePrefab;

    Transform consumptionSemanticPercantageParent;
    Transform provisionSemanticPercantageParent;
    TextMeshProUGUI consumptionAmount;
    TextMeshProUGUI provisionAmount;

    float energyConsumption = 0;
    float energyProvision = 0;

    Dictionary<string, float> semanticDataContribution;
    Dictionary<string, float> semanticDataPercentage;


    // Start is called before the first frame update
    void Start()
    {
        semanticDataContribution = new Dictionary<string, float>();
        semanticDataPercentage = new Dictionary<string, float>();
        consumptionSemanticPercantageParent = consumptionToolTip.transform.Find("Vertical Items");
        provisionSemanticPercantageParent = provisionToolTip.transform.Find("Vertical Items");
        consumptionAmount = consumptionToolTip.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        provisionAmount = provisionToolTip.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        // get the semantic data percentage
        Dictionary<string, Dictionary<string, float>> allData = DataSetting.getEnergyDistribution();
        semanticDataPercentage.Add("Energy Consumption", 1.0f / allData["Energy Consumption"].Count);
        semanticDataPercentage.Add("Energy Provision", 1.0f / allData["Energy Provision"].Count);
        // Update the semantic data contributions
        foreach (KeyValuePair<string, float> semanticData in allData["Energy Consumption"])
        {
            GameObject semanticPercentageObject = Instantiate(semanticPercentagePrefab, consumptionSemanticPercantageParent);
            semanticPercentageObject.GetComponent<SemanticPercentage>().SetSemanticData(semanticData.Key, semanticData.Value);
            semanticDataContribution.Add(semanticData.Key, semanticData.Value * semanticDataPercentage["Energy Consumption"]);
            UpdateEnergy(EnergyConsumption.Instance, semanticData.Key, semanticData.Value * semanticDataPercentage["Energy Consumption"]);
        }
        foreach (KeyValuePair<string, float> semanticData in allData["Energy Provision"])
        {
            GameObject semanticPercentageObject = Instantiate(semanticPercentagePrefab, provisionSemanticPercantageParent);
            semanticPercentageObject.GetComponent<SemanticPercentage>().SetSemanticData(semanticData.Key, semanticData.Value);
            semanticDataContribution.Add(semanticData.Key, semanticData.Value * semanticDataPercentage["Energy Provision"]);
            UpdateEnergy(EnergyProvision.Instance, semanticData.Key, semanticData.Value * semanticDataPercentage["Energy Provision"]);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Update the energy process bar and the semantic data percentage.
    /// </summary>
    /// <param name="levelFactor"></param>
    /// <param name="semanticName"></param>
    /// <param name="diff"></param>
    public void UpdateEnergy(LevelFactor levelFactor, string semanticName, float diff)
    {
        if (levelFactor == EnergyConsumption.Instance)
        {
            energyConsumption += diff;
        }
        else if (levelFactor == EnergyProvision.Instance)
        {
            energyProvision += diff;
        }

        float energyConsumptionProcess = 1;
        float energyProvisionProcess = 0;
        if (energyConsumption + energyProvision != 0)
        {
            energyProvisionProcess = energyProvision / (energyProvision + energyConsumption);
            energyConsumptionProcess = 1 - energyProvisionProcess;
        }
        energyProvisionText.text = (energyProvisionProcess * 100).ToString("0.0") + "%";
        energyConsumptionText.text = (energyConsumptionProcess * 100).ToString("0.0") + "%";
        consumptionAmount.text = "<color=#D32955>" + energyConsumption.ToString() + "</color>";
        provisionAmount.text = "<color=#61FFAC>" + energyProvision.ToString() + "</color>";
        energyProcessBar.GetComponent<GradientPercentage>().UpdateOffset(energyConsumptionProcess);

        // update the specific semantic data percentage
        foreach (var semanticPercentage in consumptionSemanticPercantageParent.GetComponentsInChildren<SemanticPercentage>())
        {
            semanticPercentage.SetSemanticData(semanticPercentage.semanticName.text, semanticDataContribution[semanticPercentage.semanticName.text] * semanticDataPercentage["Energy Consumption"]);
        }
        foreach (var semanticPercentage in provisionSemanticPercantageParent.GetComponentsInChildren<SemanticPercentage>())
        {
            semanticPercentage.SetSemanticData(semanticPercentage.semanticName.text, semanticDataContribution[semanticPercentage.semanticName.text] * semanticDataPercentage["Energy Provision"]);
        }
    }

    public float GetSemanticPercentage(string levelFactorName)
    {
        return semanticDataPercentage[levelFactorName];
    }
}
