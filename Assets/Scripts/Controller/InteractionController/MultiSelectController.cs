using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;

public class MultiSelectController : MonoBehaviour
{
    public static MultiSelectController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);

    }

    public Color rectColor = new Color(1.0f, 0.33f, 0.64f, 1);
    public Shader rimShader;

    private Vector3 start = Vector3.zero;//save the position of the mouse
    private Material rectMat = null;//material of the rectangle drawn
    public bool drawRectangle = false;//the sign of the beginning of drawing
    private bool isSelected = false;//the sign of the end of drawing
    public bool shopMode = false; // when shopping begins, we need to outline the available buildings

    public GameObject cancelBtn;
    private Transform buttons;
    [SerializeField]
    public GameObject colliderPrefab;
    private GameObject colliderCube;
    private Transform colliderParent;
    private Vector3 cameraOldPosition; // save the original camera position
    private Vector3 cameraOldRotation; // save the camera look at position

    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> selectedBuildings = new List<GameObject>();
    public Dictionary<string, Building> disableBuildings = new Dictionary<string, Building>();

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
        if (colliderCube == null)
            colliderCube = Instantiate(colliderPrefab, colliderParent, false);
        else
            colliderCube.SetActive(true);
        colliderCube.transform.localScale = Vector3.zero;
    }

    // Use this for initialization
    void Start()
    {
        // initialize the material of the rectangle
        rectMat = new Material(rimShader);
        rectMat.hideFlags = HideFlags.HideAndDontSave;
        rectMat.shader.hideFlags = HideFlags.HideAndDontSave;

        // initialize the building list
        foreach (KeyValuePair<Building, GameObject> buildingDic in BuildingManager.Instance.buildingObjects)
        {
            buildings.Add(buildingDic.Value);
        }
        colliderParent = buildings[0].transform.parent.parent.parent.parent;
    }

    void Update()
    {
        if (shopMode)
        {
            StartCoroutine(OutlineAvailable());
            shopMode = false;
        }
        // change the shader color after the buildings have been selected
        if (isSelected) StartCoroutine(ChangeSelectionsShader(Settings.selectedColor));

        if (Input.GetMouseButtonDown(0))
        {
            // StartCoroutine(DisSelectBuildings()); // clear the buildings selected before
            drawRectangle = true;//if left click, start drawing
            start = Input.mousePosition;//save the current position of the mouse
            // set the position of the collider cube
            Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(new Vector3(start.x, start.y, 0));
            // Vector3 offset = colliderCube.transform.position - mousePosInWorld;
            colliderCube.transform.position = mousePosInWorld - new Vector3(colliderCube.transform.localScale.x / 2, 0, colliderCube.transform.localScale.z / 2);
            // Debug.Log("colliderCube.transform.position: " + colliderCube.transform.position + " mousePosInWorld: " + mousePosInWorld);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drawRectangle = false;//end drawing
            isSelected = true;// the buildings have been selected
        }
        if (drawRectangle) StartCoroutine(CheckSelection(start, Input.mousePosition));//start drawing and selecting the buildings
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

    /// <summary>
    /// Draw the rectangle
    /// </summary>
    void OnPostRender()
    {
        //drawing is recommended to put in OnPostRender()
        if (drawRectangle)
        {
            Vector3 end = Input.mousePosition;//current position of the mouse
            GL.PushMatrix();//save the rotation matrix of the camera

            if (!rectMat)
                return;

            rectMat.SetPass(0);
            GL.LoadPixelMatrix();//set drawing in the pixel coordinate

            GL.Begin(GL.QUADS);
            GL.Color(new Color(rectColor.r, rectColor.g, rectColor.b, 0.1f));//set the color of the rectangle
            GL.Vertex3(start.x, start.y, 1);
            GL.Vertex3(end.x, start.y, 1);
            GL.Vertex3(end.x, end.y, 1);
            GL.Vertex3(start.x, end.y, 1);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(rectColor);//the lines of the rectangle
            GL.Vertex3(start.x, start.y, 1);
            GL.Vertex3(end.x, start.y, 1);
            GL.Vertex3(end.x, start.y, 1);
            GL.Vertex3(end.x, end.y, 1);
            GL.Vertex3(end.x, end.y, 1);
            GL.Vertex3(start.x, end.y, 1);
            GL.Vertex3(start.x, end.y, 1);
            GL.Vertex3(start.x, start.y, 1);
            GL.End();

            GL.PopMatrix();//recover the rotation matrix
        }
    }

    /// <summary>
    /// Check if the buildings are in the rectangle
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    IEnumerator CheckSelection(Vector3 start, Vector3 end)
    {
        Vector3 p1 = Vector3.zero;
        Vector3 p2 = Vector3.zero;
        if (start.x > end.x)
        {
            //make sure that p1 < p2, because drawing may not start in the direction of right down
            p1.x = end.x;
            p2.x = start.x;
        }
        else
        {
            p1.x = start.x;
            p2.x = end.x;
        }
        if (start.y > end.y)
        {
            p1.y = end.y;
            p2.y = start.y;
        }
        else
        {
            p1.y = start.y;
            p2.y = end.y;
        }

        Vector3 location1 = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(p1.x, p1.y, 0));
        Vector3 location2 = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(p2.x, p2.y, 0));
        Vector3 oldScale = colliderCube.transform.localScale;
        colliderCube.transform.localScale = new Vector3(Mathf.Abs(location1.x - location2.x), 10000, Mathf.Abs(location1.z - location2.z));
        if (oldScale != colliderCube.transform.localScale)
            colliderCube.transform.position = location1 - new Vector3(colliderCube.transform.localScale.x / 2, 0, colliderCube.transform.localScale.z / 2);
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
        cancelBtn.SetActive(false);
        cancelBtn.GetComponent<ButtonWithSound>().onClick.RemoveAllListeners();
        SelectingController.Instance.enabled = true;
        this.enabled = false;
    }

    void OnDisable()
    {
        // move back camera
        GetComponent<CameraAnimation>().CameraMove(cameraOldPosition, cameraOldRotation, 3f);
        colliderCube.SetActive(false);
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
