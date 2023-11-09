using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShopInstallManager : MonoBehaviour
{
    public static ShopInstallManager Instance { get; private set; } // static singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            shopItems = ES3.Load<List<ShopItem>>("shopItems", "Player/ShopItems", new List<ShopItem>());
            GameObject[] gameObjects = Resources.LoadAll<GameObject>("ShopResources/Shop Items");
            shopItemPrefabs = gameObjects != null ? new List<GameObject>(gameObjects) : new List<GameObject>();
        }

        if (Instance != this)
            Destroy(gameObject);
    }

    List<ShopItem> shopItems;
    List<GameObject> shopItemPrefabs;
    public Transform shopItemParent;
    [SerializeField] GameObject colliderPrefab;
    Dictionary<ShopItem, List<GameObject>> influencedBuildings;
    Dictionary<ShopItem, GameObject> colliders;


    // Start is called before the first frame update
    void Start()
    {
        influencedBuildings = new Dictionary<ShopItem, List<GameObject>>();
        colliders = new Dictionary<ShopItem, GameObject>();
        foreach (ShopItem item in shopItems)
        {
            var shopItemPrefab = shopItemPrefabs.Find(e => string.Equals(e.name, item.name));
            var shopItem = Instantiate(shopItemPrefab, shopItemParent, false);
            shopItem.transform.position = item.transform[0];
            shopItem.transform.rotation = Quaternion.Euler(item.transform[1]);
            shopItem.transform.localScale = item.transform[2];
            var collider = Instantiate(colliderPrefab, new Vector3(item.transform[0].x, 0, item.transform[0].z), Quaternion.identity);
            collider.transform.localScale = new Vector3(item.range * 5, 200, item.range * 5);
            List<GameObject> buildings = FindInfluencedBuildings(collider);
            influencedBuildings.Add(item, buildings);
            colliders.Add(item, collider);
        }
        StartCoroutine(CheckCollidersAlive());
    }

    public ShopItem GetShopItem(Transform infrastructure)
    {
        return shopItems.Find(item => item.CompareTo(infrastructure));
    }

    /// <summary>
    /// Use collider to find the influenced buildings triggered with the collider which is corresponding to the shop item
    /// </summary>
    /// <param name="collider"></param>
    List<GameObject> FindInfluencedBuildings(GameObject collider)
    {
        return collider.GetComponent<ColisionDetection>().collisionObjects;
    }

    /// <summary>
    /// Find the buildings that are influenced by the shop item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<GameObject> FindInfluencedBuildings(ShopItem item)
    {
        return influencedBuildings[item] ?? null;
    }

    /// <summary>
    /// Given a shop item, show the corresponding range
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public void ShowRange(ShopItem item)
    {
        colliders[item].GetComponent<ColisionDetection>().show = true;
        colliders[item].SetActive(true);
    }

    /// <summary>
    /// Given a shop item, hide the corresponding range
    /// </summary>
    /// <param name="item"></param>
    public void HideRange(ShopItem item)
    {
        colliders[item].GetComponent<ColisionDetection>().show = false;
        colliders[item].SetActive(false);
    }

    /// <summary>
    /// Add the shop item to the shop item list
    /// </summary>
    /// <param name="item"></param>
    public void AddShopItem(ShopItem item)
    {
        shopItems.Add(item);
        List<GameObject> buildings = new List<GameObject>();
        SingleInstallController.Instance.selectedBuildings.ForEach(e => buildings.Add(e));
        influencedBuildings.Add(item, buildings);
        // add the collider range of the shop item
        var collider = Instantiate(colliderPrefab, new Vector3(item.transform[0].x, 0, item.transform[0].z), Quaternion.identity);
        collider.transform.localScale = new Vector3(item.range * 5, 200, item.range * 5);
        colliders.Add(item, collider);
        collider.SetActive(false);
    }

    public void SaveShopItems()
    {
        ES3.Save<List<ShopItem>>("shopItems", shopItems, "Player/ShopItems");
    }

    IEnumerator CheckCollidersAlive()
    {
        while (true)
        {
            foreach (var c in colliders)
            {
                if (c.Value == null)
                {
                    ShopItem item = c.Key;
                    var collider = Instantiate(colliderPrefab, new Vector3(item.transform[0].x, 0, item.transform[0].z), Quaternion.identity);
                    collider.transform.localScale = new Vector3(item.range * 5, 200, item.range * 5);
                    List<GameObject> buildings = FindInfluencedBuildings(collider);
                    influencedBuildings[item] = buildings;
                    colliders[item] = collider;
                    break;
                }
            }
            yield return null;
        }
    }
}
