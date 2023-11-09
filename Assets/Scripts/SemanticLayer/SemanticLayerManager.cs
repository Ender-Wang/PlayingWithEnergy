using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;

public class SemanticLayerManager : MonoBehaviour
{
    public static SemanticLayerManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private GameObject semanticLayerButtonPrefab;
    [SerializeField]
    private Transform semanticLayerButtonParentLeft;
    [SerializeField]
    private Transform semanticLayerButtonParentRight;
    [SerializeField]
    private Transform shopProcessButtonParent;
    private List<string> semanticName;
    public GameObject currentSemanticLayerButton { get; set; }
    GameObject roofSurfaces;
    public bool isSemanticLayerButtonActive { get; set; }
    public bool isSemanticItemLayerButtonActive { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        roofSurfaces = GameObject.FindWithTag("Roof");
        semanticName = DataSetting.getSemanticNames();
        foreach (string name in semanticName)
        {
            // Semantic Layer Button
            currentSemanticLayerButton = Instantiate(semanticLayerButtonPrefab, semanticLayerButtonParentLeft, false);
            currentSemanticLayerButton.GetComponent<SemanticLayerButton>().SetSemanticLayerButton(name);

            // Semantic Item Layer Button
            currentSemanticLayerButton = Instantiate(semanticLayerButtonPrefab, semanticLayerButtonParentRight, false);
            currentSemanticLayerButton.GetComponent<SemanticLayerButton>().SetSemanticLayerContainsInstalledItemButton(name);
        }

        List<GameObject> buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Selectable"));
        List<GameObject> buildingObjects = new List<GameObject>();

        // for (int i = buildings.Count - 1; i > 0; i--)
        // {
        //     GameObject obj = buildings[i];
        //     if (obj.name != "Building")
        //     {
        //         buildings.Remove(obj);
        //     }
        // }

        // for (int i = 0; i < buildings.Count; i++)
        // {
        //     for (int j = 0; j < buildings[i].transform.childCount; j++)
        //     {
        //         var child = buildings[i].transform.GetChild(j).gameObject;
        //         buildingObjects.Add(child);
        //     }
        // }
    }

    void Update()
    {
        if (isSemanticLayerButtonActive)
        {
            StartCoroutine(BuildingColoringWithData.Instance.setBuildingColorContainData(currentSemanticLayerButton.GetComponent<SemanticLayerButton>().semanticDataName));
            isSemanticLayerButtonActive = false;
        }
        if (isSemanticItemLayerButtonActive)
        {
            StartCoroutine(BuildingColoringWithData.Instance.setBuildingColorContainInstalledItemWithSemanticData(currentSemanticLayerButton.GetComponent<SemanticLayerButton>().semanticDataName));
            isSemanticItemLayerButtonActive = false;
        }
    }

    /// <summary>
    /// API: show the semantic layer btns corresponding to the selected item in shop
    /// </summary>
    /// <param name="shopItem"></param>
    public void ShowSemanticLayerButtons(ShopItem shopItem)
    {
        foreach (string name in shopItem.semanticData)
        {
            if (shopProcessButtonParent)
            {
                shopProcessButtonParent.gameObject.SetActive(true);
                currentSemanticLayerButton = Instantiate(semanticLayerButtonPrefab, shopProcessButtonParent, false);
                currentSemanticLayerButton.GetComponent<SemanticLayerButton>().SetSemanticLayerButton(name);
            }
        }
        if (shopItem.semanticData.Count > 0)
        {
            shopProcessButtonParent.GetChild(0).GetComponent<SemanticLayerButton>().button.onClick.Invoke();
        }
    }

    /// <summary>
    /// API: Destroy the semantic layer btns corresponding to the selected item in shop
    /// </summary>
    public void HideSemanticLayerButtons()
    {
        foreach (Transform child in shopProcessButtonParent)
        {
            if (child.GetComponent<SemanticLayerButton>().isActive)
                child.GetComponent<SemanticLayerButton>().button.onClick.Invoke();
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// API: Activate the roof or deactivate
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    public IEnumerator ActivateRoof(bool active)
    {
        roofSurfaces.SetActive(active);
        yield return null;
    }

}
