using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this script to detect the collision of the collider with the buildings
/// </summary>
public class ColisionDetection : MonoBehaviour
{
    public List<GameObject> collisionObjects { get; set; } // list of objects that are detected by the collider,
    public bool show; // flag to indicate whether the collider is shown
    bool detection; // flag to indicate whether the collision detection still need working
    int detectionCount; // number of collision objects
    int detectionCountTemp; // current number of collision objects before update (in physics update)

    void OnEnable()
    {
        detectionCount = 0;
        detection = true;
        collisionObjects = new List<GameObject>();
    }


    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(TriggerEnter(other));
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(TriggerExit(other));
    }

    private void OnTriggerStay(Collider other)
    {
        detectionCountTemp = detectionCount;
        if (detection)
        {
            StartCoroutine(TriggerStay(other));
        }
    }

    void Update()
    {
        // when not in the event of shopping
        if (MultiSelectController.Instance && !MultiSelectController.Instance.enabled && !SingleInstallController.Instance.enabled && detectionCountTemp == detectionCount) // the current number stop increasing, all the objects are detected
        {
            detection = false;
            if (!show)
                gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        collisionObjects.Clear();
    }

    IEnumerator TriggerEnter(Collider other)
    {
        if (other.transform.parent && other.transform.parent.CompareTag("Selectable") && !other.transform.CompareTag("ShopItem"))
        {
            string name = BuildingManager.Instance.GetBuildingName(other.gameObject);
            if (MultiSelectController.Instance && MultiSelectController.Instance.enabled)
            {
                if (!MultiSelectController.Instance.disableBuildings.ContainsKey(name))
                {
                    other.gameObject.GetComponent<ChangeShader>().ChangeColor(Settings.highlightColor);
                    MultiSelectController.Instance.Selecting(other.gameObject);
                    if (MultiSelectController.Instance.selectedBuildings.IndexOf(other.gameObject) == -1) MultiSelectController.Instance.selectedBuildings.Add(other.gameObject);
                }
            }
            if (SingleInstallController.Instance && SingleInstallController.Instance.enabled)
            {
                if (!SingleInstallController.Instance.disableBuildings.ContainsKey(name))
                {
                    other.gameObject.GetComponent<ChangeShader>().ChangeColor(Settings.highlightColor);
                    SingleInstallController.Instance.Selecting(other.gameObject);
                    if (SingleInstallController.Instance.selectedBuildings.IndexOf(other.gameObject) == -1) SingleInstallController.Instance.selectedBuildings.Add(other.gameObject);
                }
            }
        }
        yield return null;
    }

    IEnumerator TriggerExit(Collider other)
    {
        if (MultiSelectController.Instance && SingleInstallController.Instance && other.transform.parent && other.transform.parent.CompareTag("Selectable") && !other.transform.CompareTag("ShopItem"))
        {
            string name = BuildingManager.Instance.GetBuildingName(other.gameObject);
            if (MultiSelectController.Instance.enabled)
            {
                if (!MultiSelectController.Instance.disableBuildings.ContainsKey(name))
                {
                    other.gameObject.GetComponent<ChangeShader>().ChangeColor(Settings.availableColor);
                    if (MultiSelectController.Instance.selectedBuildings.IndexOf(other.gameObject) != -1) MultiSelectController.Instance.selectedBuildings.Remove(other.gameObject);
                }
            }


            if (SingleInstallController.Instance.enabled)
            {
                if (!SingleInstallController.Instance.disableBuildings.ContainsKey(name))
                {
                    other.gameObject.GetComponent<ChangeShader>().ChangeColor(Settings.availableColor);
                    if (SingleInstallController.Instance.selectedBuildings.IndexOf(other.gameObject) != -1) SingleInstallController.Instance.selectedBuildings.Remove(other.gameObject);
                }
            }
        }
        yield return null;
    }

    IEnumerator TriggerStay(Collider other)
    {
        if (SingleInstallController.Instance && MultiSelectController.Instance && !MultiSelectController.Instance.enabled && !SingleInstallController.Instance.enabled)
        {
            if (other.transform.parent && other.transform.parent.CompareTag("Selectable") && !other.transform.CompareTag("ShopItem") && collisionObjects.IndexOf(other.gameObject) == -1)
            {
                collisionObjects.Add(other.gameObject);
                ++detectionCount;
            }
        }
        yield return null;
    }
}
