using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class LevelFactorController : MonoBehaviour
    {
        public TextMeshProUGUI factorName;
        public GameObject semanticFactorPrefab;
        public GameObject semanticFactorParent;
        GameObject semanticFactor;
        Dictionary<string, int> semanticDataContributions;

        /// <summary>
        /// Show the level factor name and the semantic data contributions
        /// </summary>
        /// <param name="levelFactor"></param>
        public void SetLevelFactor(LevelFactor levelFactor)
        {
            this.factorName.text = levelFactor.name();

            semanticDataContributions = DataSetting.getTotalContributionsOfAllData();
            int i = 0; // count the index for getting the index of color
            foreach (var semanticDataContribution in semanticDataContributions)
            {
                // classify the semantic data with different level factors
                if (DataSetting.getDataSetting(semanticDataContribution.Key).levelFactor != levelFactor.name()) continue;
                semanticFactor = Instantiate(semanticFactorPrefab, semanticFactorParent.transform);
                Color color1 = Settings.UIColors[i % Settings.UIColors.Count];
                Color color2 = Settings.UIColors[(i + 1) % Settings.UIColors.Count];
                semanticFactor.GetComponent<SemanticDataFactorController>().SetSemanticFactor(semanticDataContribution.Key, semanticDataContribution.Value, color1, color2);
                i += 2;
            }

        }
    }
}

