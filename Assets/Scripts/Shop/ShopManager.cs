using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;
using UltimateClean;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{


    [Header("Drag the items here")]
    public TextAsset shopFile;

    public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.3f);
    public List<ShopItem> shopItems { get; set; }
    public GameObject currentShopPopup { get; set; }
    public bool startShopping { get; set; }
    public float price { get; set; }
    public ShopItem currentShopItem { get; set; }
    public GameObject background { get; set; }

    public Canvas m_canvas { get; set; }
    private GameObject m_confirmBuyingPanel;

    public static ShopManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);

        startShopping = false;
        m_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        shopItems = ES3.Load<List<ShopItem>>("shopItems", "Shop/ShopItems", new List<ShopItem>());
        if (shopItems.Count == 0)
        {
            SetListShopItem(shopFile.text);
        }

    }

    // // Start is called before the first frame update
    // void Start()
    // {

    //     // StartCoroutine(SetListShopItem(shopFilePath));
    // }

    private void Update()
    {
        if (startShopping == true)
        { // start shopping
            MultipleBuildingInstallation();
            SingleBuildingInstallation();
        }

    }

    /// <summary>
    /// API: Initialize the prework for the buying process
    /// </summary>
    /// <param name="popup">m_popup</param>
    public void OpenPopup(GameObject popup)
    {
        if (currentShopPopup != null)
            currentShopPopup.GetComponent<Popup>().Close();
        currentShopPopup = popup;
        StartCoroutine(RemoveBackground());
    }

    /// <summary>
    /// API: Update the building with the shop item
    /// </summary>
    /// <param name="building"></param>
    /// <param name="shopItem"></param>
    public void UpdateBuilding(GameObject building, ShopItem shopItem, bool remove = false)
    {
        BuildingManager.Instance.UpdateBuilding(BuildingManager.Instance.GetBuildingName(building), shopItem, remove);
    }

    /// <summary>
    /// API: Add a background
    /// </summary>
    public void AddBackground()
    {
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        background = new GameObject("PopupBackground");
        var image = background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(0.5f, 0.4f, false);

        var canvas = GameObject.Find("Canvas");
        background.transform.localScale = new Vector3(1, 1, 1);
        background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        background.transform.SetParent(canvas.transform, false);
        background.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    /// <summary>
    /// API: Remove the background
    /// </summary>
    public IEnumerator RemoveBackground()
    {
        if (background != null)
        {
            var image = background.GetComponent<Image>();
            if (image != null)
            {
                image.CrossFadeAlpha(0.0f, 0.2f, false);
            }
            yield return new WaitForSeconds(0.5f);
            Destroy(background);
        }
    }

    /// <summary>
    /// Set the list of shop items from CSV file
    /// </summary>
    /// <param name="filePath"></param>
    private void SetListShopItem(string fileContent)
    {
        Stream fileStream = GenerateStreamFromString(fileContent);
        using (var reader = new StreamReader(fileStream))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split(',');

            // Find the indices of the ID, Name, Category, Description, Price, Contribution and Icon columns
            var idIndex = Array.IndexOf(headers, "ID");
            var nameIndex = Array.IndexOf(headers, "Name");
            var categoryIndex = Array.IndexOf(headers, "Category");
            var semanticDataIndex = Array.IndexOf(headers, "SemanticData");
            var descriptionIndex = Array.IndexOf(headers, "Description");
            var priceIndex = Array.IndexOf(headers, "Price");
            var contributionIndex = Array.IndexOf(headers, "Contribution");
            var iconIndex = Array.IndexOf(headers, "Icon");
            var upgradePriceIndex = Array.IndexOf(headers, "UpgradedPrice");
            var maxContributionsIndex = Array.IndexOf(headers, "MaxContribution");
            var rangeIndex = Array.IndexOf(headers, "Range");

            // Read the rest of the lines and store the corresponding values in the lists
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = CsvParser.parse(line);
                if (valuesArray == null || valuesArray.Length < 10)
                {
                    continue;
                }

                // Add the values to the lists
                // id
                var id = int.Parse(valuesArray[idIndex]);

                // name
                var name = valuesArray[nameIndex];

                // category
                List<string> category = new List<string>();
                category = valuesArray[categoryIndex].Split('|').ToList();
                category.RemoveAt(category.Count - 1);

                // semantic data
                List<string> semanticData = new List<string>();
                semanticData = valuesArray[semanticDataIndex].Split('|').ToList();
                semanticData.RemoveAt(semanticData.Count - 1);

                // description
                var description = valuesArray[descriptionIndex];

                // price
                var price = int.Parse(valuesArray[priceIndex]);

                // contribution
                List<string> contributionParse = new List<string>();
                contributionParse = valuesArray[contributionIndex].Split('|').ToList();
                contributionParse.RemoveAt(contributionParse.Count - 1);
                List<int> contribution = new List<int>();
                contribution = contributionParse.Select(x => int.Parse(x)).ToList();

                // icon
                var icon = valuesArray[iconIndex];

                // upgrade price
                var upgradePrice = int.Parse(valuesArray[upgradePriceIndex]);


                // max contribution
                List<string> maxContributionParse = new List<string>();
                maxContributionParse = valuesArray[maxContributionsIndex].Split('|').ToList();
                maxContributionParse.RemoveAt(maxContributionParse.Count - 1);
                List<int> maxContribution = new List<int>();
                maxContribution = maxContributionParse.Select(x => int.Parse(x)).ToList();

                // range
                var range = float.Parse(valuesArray[rangeIndex] != "" ? valuesArray[rangeIndex] : "0");

                ShopItem shopItem = new ShopItem(id, name, category, semanticData, description, price, contribution, "ShopResources/Icons/" + valuesArray[iconIndex], upgradePrice, maxContribution, range);
                shopItems.Add(shopItem);

            }
        }
    }

    /// <summary>
    /// Install the shop item to multiple buildings
    /// </summary>
    private void MultipleBuildingInstallation()
    {
        if (MultiSelectController.Instance.enabled && MultiSelectController.Instance.selectedBuildings.Count > 0 && MultiSelectController.Instance.drawRectangle == false) // after end up with selecting the buildings
        {
            GetComponent<ConfirmBuyingPanelOpener>().OpenPopup();
            startShopping = false; // stop shopping
        }
    }


    private void SingleBuildingInstallation()
    {
        if (SingleInstallController.Instance.enabled && SingleInstallController.Instance.isSelected)
        {
            GetComponent<ConfirmBuyingPanelOpener>().OpenPopup();
            SingleInstallController.Instance.isSelected = false;
            startShopping = false; // stop shopping
        }
    }

    /// <summary>
    /// API: activate the shop item
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void ActivateShopItem(string name, int maxLevel)
    {
        foreach (var shopItem in shopItems)
        {
            if (shopItem.name == name)
            {
                shopItem.isVisible = true;
                shopItem.maxLevel = maxLevel;
                break;
            }
        }
    }

    /// <summary>
    /// API: Get the shop item with its name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ShopItem GetShopItem(string name)
    {
        foreach (var shopItem in shopItems)
        {
            if (shopItem.name == name)
            {
                return shopItem;
            }
        }
        return null;
    }

    public void SaveShopItem()
    {
        ES3.Save<List<ShopItem>>("shopItems", shopItems, "Shop/ShopItems");
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
