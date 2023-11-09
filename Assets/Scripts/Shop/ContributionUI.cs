using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UltimateClean
{
    public class ContributionUI : MonoBehaviour
    {
        public TextMeshProUGUI semanticData;
        public SlicedFilledImage progressBar;
        public TextMeshProUGUI progressText;
        public Image semanticDataImage;

        public void SetContributionUI(string semanticDataName, int contribution, int maxContribution, Color color1, Color color2)
        {
            semanticData.text = semanticDataName;
            progressBar.fillAmount = contribution / (float)maxContribution;
            progressText.text = contribution.ToString();
            semanticDataImage.sprite = Resources.Load<Sprite>("EnergyData/" + semanticDataName + "/" + semanticDataName);
            progressBar.transform.GetComponent<Gradient>().Color1 = color1;
            progressBar.transform.GetComponent<Gradient>().Color2 = color2;
        }

        public void ChangeContribution(int contribution, int maxContribution)
        {
            progressBar.fillAmount = contribution / (float)maxContribution;
            progressText.text = contribution.ToString();
        }
    }

}
