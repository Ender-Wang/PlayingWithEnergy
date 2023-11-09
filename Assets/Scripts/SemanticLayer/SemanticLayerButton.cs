using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateClean
{
    public class SemanticLayerButton : MonoBehaviour
    {
        public string semanticDataName { get; set; }

        public ButtonWithSound button;
        [SerializeField]
        private Image buttonImage;
        [SerializeField]
        private TextMeshProUGUI tooltip;
        [SerializeField] private GameObject filterPrefab;
        [SerializeField] private Transform filterParent;
        private GameObject filter;
        public bool isActive { get; set; }

        void OnEnable()
        {
            // TODO: Optimize
            isActive = false;
            GameObject currentButton = SemanticLayerManager.Instance.currentSemanticLayerButton;
            if (currentButton != null && currentButton != gameObject && currentButton.GetComponent<SemanticLayerButton>().isActive)
            {
                SemanticLayerManager.Instance.currentSemanticLayerButton.GetComponent<SemanticLayerButton>().button.onClick.Invoke();
            }
            StartCoroutine(SemanticLayerManager.Instance.ActivateRoof(true));
            StartCoroutine(BuildingColoringWithData.Instance.setBuildingColorContainData());
            foreach (Transform child in filterParent)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// set the button for each semantic data
        /// </summary>
        /// <param name="semanticDataName">semantic data name</param>
        public void SetSemanticLayerButton(string semanticDataName)
        {
            this.semanticDataName = semanticDataName;
            isActive = false;
            Sprite sprite = Resources.Load<Sprite>("EnergyData/" + semanticDataName + "/" + semanticDataName);
            if (sprite != null)
            {

                buttonImage.sprite = sprite;
            }
            button.onClick.AddListener(ChangeLayer);
            tooltip.text = semanticDataName;
            filterParent = GameObject.Find("LayerColorLegend").transform;
        }

        /// <summary>
        /// Layer for each semantic data
        /// </summary>
        void ChangeLayer()
        {
            isActive = !isActive;
            // forbid to activate two buttons at one time
            GameObject currentButton = SemanticLayerManager.Instance.currentSemanticLayerButton;
            if (currentButton != null && currentButton != gameObject && currentButton.GetComponent<SemanticLayerButton>().isActive)
            {
                SemanticLayerManager.Instance.currentSemanticLayerButton.GetComponent<SemanticLayerButton>().button.onClick.Invoke();
            }
            SemanticLayerManager.Instance.currentSemanticLayerButton = gameObject;

            if (!isActive)
            {
                StartCoroutine(SemanticLayerManager.Instance.ActivateRoof(true));

                StartCoroutine(BuildingColoringWithData.Instance.setBuildingColorContainData());
                foreach (Transform child in filterParent)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                SemanticLayerManager.Instance.isSemanticLayerButtonActive = true;
                StartCoroutine(SemanticLayerManager.Instance.ActivateRoof(false));
                // show the layer filter
                List<Color> colors = DataSetting.getDataSetting(semanticDataName).colors;
                int i = 0;
                foreach (Color color in colors)
                {
                    filter = Instantiate(filterPrefab, filterParent);
                    filter.GetComponent<SemanticLayerFilter>().SetSemanticLayerFilter(color, DataSetting.getDataSetting(semanticDataName).displayedInfos[i]);
                    ++i;
                }
            }
        }

        /// <summary>
        /// set the button for installed item for each semantic data
        /// </summary>
        /// <param name="semanticDataName">semantic data name</param>
        public void SetSemanticLayerContainsInstalledItemButton(string semanticDataName)
        {
            this.semanticDataName = semanticDataName;
            isActive = false;
            Sprite sprite = Resources.Load<Sprite>("EnergyData/" + semanticDataName + "/" + semanticDataName + " Item"); //+ "Item"
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
            }
            button.onClick.AddListener(ItemLayer);
            tooltip.text = "Item installed with " + semanticDataName;
            filterParent = GameObject.Find("LayerColorLegend").transform;
        }

        /// <summary>
        /// Layer for installed item for each semantic data
        /// </summary>
        void ItemLayer()
        {
            isActive = !isActive;
            // forbid to activate two buttons at one time
            GameObject currentButton = SemanticLayerManager.Instance.currentSemanticLayerButton;
            if (currentButton != null && currentButton != gameObject && currentButton.GetComponent<SemanticLayerButton>().isActive)
            {
                SemanticLayerManager.Instance.currentSemanticLayerButton.GetComponent<SemanticLayerButton>().button.onClick.Invoke();
            }
            SemanticLayerManager.Instance.currentSemanticLayerButton = gameObject;

            if (!isActive)
            {
                StartCoroutine(SemanticLayerManager.Instance.ActivateRoof(true));
                StartCoroutine(BuildingColoringWithData.Instance.setBuildingColorContainData());
                foreach (Transform child in filterParent)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                SemanticLayerManager.Instance.isSemanticItemLayerButtonActive = true;
                StartCoroutine(SemanticLayerManager.Instance.ActivateRoof(false));
                // show the layer filter
                List<Color> colors = DataSetting.getDataSetting(semanticDataName).colors;
                filter = Instantiate(filterPrefab, filterParent);
                filter.GetComponent<SemanticLayerFilter>().SetSemanticLayerFilter(Color.green, "Provided");
                filter = Instantiate(filterPrefab, filterParent);
                filter.GetComponent<SemanticLayerFilter>().SetSemanticLayerFilter(Color.white, "Not Provided");
            }
        }
    }
}