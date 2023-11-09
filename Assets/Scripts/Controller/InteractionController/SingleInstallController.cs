using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;
using UnityEditor;

public class SingleInstallController : MonoBehaviour
{
    public static SingleInstallController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);

    }

    public bool drawRange = false;//the sign of the beginning of drawing
    public bool isSelected = false;//the sign of the end of drawing
    public bool shopMode = false; // when shopping begins, we need to outline the available buildings

    public GameObject cancelBtn;
    private Transform buttons;
    [SerializeField]
    public GameObject colliderPrefab;
    private GameObject colliderSphere;
    private Vector3 cameraOldPosition; // save the original camera position
    private Vector3 cameraOldRotation; // save the camera look at position

    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> selectedBuildings = new List<GameObject>();
    public Dictionary<string, Building> disableBuildings = new Dictionary<string, Building>();
    public float range { get; set; }

    private GameObject shopItem;
    private GameObject shopItemPrefab;

    public void SetRange(float range)
    {
        this.range = range;
        colliderSphere.transform.localScale = new Vector3(range * 5, 200, range * 5);
    }

    /// <summary>
    /// continue the controller
    /// </summary>
    /// <param name="oneMoreItem">if true, instantiate one more shop item</param>
    public void Restart(bool oneMoreItem)
    {
        isSelected = false;
        drawRange = true;
        CheckAvailableBuildings();
        if (oneMoreItem)
        {
            shopItem = Instantiate(shopItemPrefab, ShopInstallManager.Instance.shopItemParent);
        }
        shopItem.GetComponent<Rigidbody>().useGravity = false;
    }



    void OnEnable()
    {
        cameraOldPosition = Camera.main.transform.position;
        cameraOldRotation = Camera.main.transform.localEulerAngles;
        // Get world coordinate of the screen center
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        Vector3 target = new Vector3(cameraOldPosition.x, cameraOldPosition.y, cameraOldPosition.z - 4);
        Vector3 rotation = new Vector3(90, 180, 0);
        float distance = 10f;
        if (Physics.Raycast(ray, out hit))
        {
            target = new Vector3(hit.point.x, hit.point.y + 10, hit.point.z - 4); // move to above the targert
            distance = Mathf.Abs(target.y - cameraOldPosition.y) + 4;
        }
        // move camera
        GetComponent<CameraAnimation>().CameraMove(target, rotation, 3f, true, distance);
        buttons = GameManager.Instance.canvas.transform.Find("Buttons");
        DisableButtons();
        cancelBtn.SetActive(true);
        cancelBtn.GetComponent<ButtonWithSound>().onClick.AddListener(Cancel);
        if (colliderSphere == null)
            colliderSphere = Instantiate(colliderPrefab);
        else
            colliderSphere.SetActive(true);

        shopItemPrefab = Resources.Load<GameObject>("ShopResources/Shop Items/" + ShopManager.Instance.currentShopItem.name);
        shopItem = Instantiate(shopItemPrefab, ShopInstallManager.Instance.shopItemParent);
        shopItem.GetComponent<Rigidbody>().useGravity = false;
    }

    // Use this for initialization
    void Start()
    {
        // initialize the building list
        foreach (KeyValuePair<Building, GameObject> buildingDic in BuildingManager.Instance.buildingObjects)
        {
            buildings.Add(buildingDic.Value);
        }
        drawRange = true; // start drawing the range
    }

    void Update()
    {
        if (shopMode)
        {
            StartCoroutine(OutlineAvailable());
            shopMode = false;
        }
    }

    private void OnGUI()
    {
        if (Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
        {
            drawRange = false;//end drawing
            isSelected = true;// the buildings have been selected
        }
        if (isSelected)
        {
            // change the shader color after the buildings have been selected
            StartCoroutine(ChangeSelectionsShader(Settings.selectedColor));
            // save the position of the shop item installed
            ShopManager.Instance.currentShopItem.transform = new List<Vector3>() { shopItem.transform.position, shopItem.transform.rotation.eulerAngles, shopItem.transform.localScale };
            shopItem.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        if (drawRange)
        {
            Vector3 center = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);//current position of the mouse
            colliderSphere.transform.position = new Vector3(center.x, 0, center.z);
            shopItem.transform.position = new Vector3(center.x, 8f, center.z);
        }
    }

    private IEnumerator OutlineAvailable()
    {
        //check if the buildings are in the rectangle
        foreach (GameObject obj in buildings)
        {
            string name = BuildingManager.Instance.GetBuildingName(obj);
            if (disableBuildings.ContainsKey(name))
            {
                obj.GetComponent<ChangeShader>().DisSelect();
                continue;
            }
            obj.GetComponent<ChangeShader>().Select();
            obj.GetComponent<ChangeShader>().ChangeColor(Settings.availableColor);

        }
        yield return null;
    }

    private IEnumerator ChangeSelectionsShader(Color color)
    {
        foreach (GameObject obj in selectedBuildings)
        {
            obj.gameObject.GetComponent<ChangeShader>().ChangeColor(color);
            Selecting(obj);
        }
        yield return null;
    }

    /// <summary>
    /// API: clear the buildings selected before
    /// </summary>
    /// <returns></returns>
    public IEnumerator DisSelectBuildings()
    {
        foreach (GameObject obj in selectedBuildings)
        {
            Disselecting(obj);
        }
        selectedBuildings.Clear();
        yield return null;
    }


    public void Disselecting(GameObject obj)
    {
        obj.gameObject.GetComponent<ChangeShader>().DisSelect();
    }

    public void Selecting(GameObject obj)
    {
        obj.gameObject.GetComponent<ChangeShader>().Select();
    }

    /// <summary>
    /// Btn action
    /// </summary>
    public void Cancel()
    {
        foreach (GameObject obj in buildings)
        {
            Disselecting(obj);
        }
        selectedBuildings.Clear();
        disableBuildings.Clear();
        StartCoroutine(ShopManager.Instance.RemoveBackground());
        SemanticLayerManager.Instance.HideSemanticLayerButtons();
        EnableButtons();
        cancelBtn.GetComponent<ButtonWithSound>().onClick.RemoveAllListeners();
        cancelBtn.SetActive(false);
        Destroy(shopItem);
        SelectingController.Instance.enabled = true;
        this.enabled = false;
    }

    void OnDisable()
    {
        // move back camera
        GetComponent<CameraAnimation>().CameraMove(cameraOldPosition, cameraOldRotation, 3f);
        colliderSphere.SetActive(false);
        Cancel();
    }

    /// <summary>
    /// Clear the UI
    /// </summary>
    public void DisableButtons()
    {
        for (int i = 0; i < buttons.childCount; i++)
        {
            buttons.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activate the buttons
    /// </summary>
    public void EnableButtons()
    {
        for (int i = 0; i < buttons.childCount; i++)
        {
            buttons.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// API: check available buildings
    /// </summary>
    public void CheckAvailableBuildings()
    {
        ShopItem currentItem = ShopManager.Instance.currentShopItem;
        foreach (KeyValuePair<string, Building> buildingDic in BuildingManager.Instance.buildings)
        {
            ShopItem item = buildingDic.Value.GetItemsInstalled().ContainsKey(currentItem.id) ? buildingDic.Value.GetItemsInstalled()[currentItem.id] : null;
            if (item != null)
            {
                if (item.currentLevel != currentItem.currentLevel - 1)
                {
                    disableBuildings[buildingDic.Key] = buildingDic.Value;
                }
            }
            else
            {
                if (currentItem.currentLevel != 1)
                {
                    disableBuildings[buildingDic.Key] = buildingDic.Value;
                }
            }
        }
        shopMode = true;
    }


    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
