
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    /// <summary>
    /// The base class for all loading bar controller
    /// </summary>
    public class LoadingBarController : MonoBehaviour
    {
        public TextMeshProUGUI text;


        // public Image image;
        public SlicedFilledImage slicedImage;
        public TextMeshProUGUI toolTip;

        protected StringBuilder strBuilder = new StringBuilder(4);
        protected int lastPercentage = -1;

        protected void Start()
        {
            Initialize();
        }

        void Update()
        {
            ChangeLevel();
            if (toolTip)
                ToolTipSetUp();
        }

        // protected virtual void CreateInstance() {}

        public void ChangeLevel()
        {
            float ratio = loadingBarCalculation();

            // if (ratio >= 1) --ratio;

            // if (image != null)
            //     image.fillAmount = ratio;
            if (slicedImage != null)
                slicedImage.fillAmount = ratio;

            var percentage = (int)(ratio / 1.0f * 100);
            if (percentage != lastPercentage)
            {
                lastPercentage = percentage;
                if (text != null)
                {
                    strBuilder.Clear();
                    text.text = strBuilder.Append(lastPercentage).Append("%").ToString();
                }
            }
        }

        // Calculate the the percentage of the energy/level/heat... in the current level.
        protected virtual float loadingBarCalculation() { return 0.0f; }

        protected virtual void ToolTipSetUp() { }

        protected virtual void Initialize() { }
    }

}
