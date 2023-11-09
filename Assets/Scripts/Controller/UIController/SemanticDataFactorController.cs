using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class SemanticDataFactorController : LoadingBarController
    {
        public TextMeshProUGUI factorName;
        private int contribution;

        public void SetSemanticFactor(string semanticName, int contribution, Color color1, Color color2)
        {
            this.factorName.text = semanticName;
            this.contribution = contribution;
            slicedImage.transform.GetComponent<Gradient>().Color1 = color1;
            slicedImage.transform.GetComponent<Gradient>().Color2 = color2;
        }

        protected override float loadingBarCalculation()
        {
            //TODO: Modify when change the multiplier factor
            if (contribution == 0) return 0.0f;
            return (float)contribution / LevelManager.level; // the ratio of the semantic data contribution to the total level
        }

        protected override void ToolTipSetUp()
        {
            toolTip.text = contribution.ToString() + " / " + LevelManager.level.ToString();
        }

    }
}

