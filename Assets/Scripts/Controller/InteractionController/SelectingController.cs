using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UltimateClean;

public class SelectingController : MonoBehaviour
{
    public static SelectingController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);
    }


    // public get variables
    public static Vector3 position { get { return SelectingController.Instance.selection == null ? Vector3.zero : SelectingController.Instance.selection.GetComponent<BoxCollider>().center; } } // TODO: modify the returning zero after expand the map
    public static GameObject selectedBuilding { get { return SelectingController.Instance.selection == null ? null : SelectingController.Instance.selection.gameObject; } }


    public Transform highlight;
    public Transform selection;
    private RaycastHit raycastHit;

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            if (highlight.CompareTag("ShopItem"))
            { // hide the range that the shop item can influence
                ShopInstallManager.Instance.HideRange(ShopInstallManager.Instance.GetShopItem(highlight));
            }
            highlight.GetComponent<ChangeShader>().DisSelect();
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.parent && highlight.parent.CompareTag("Selectable") && highlight != selection) // TODO: change if add more tags
            {
                if (!highlight.GetComponent<ChangeShader>().ShaderIsOn())
                {
                    highlight.GetComponent<ChangeShader>().ChangeColor(Settings.highlightColor);
                    highlight.GetComponent<ChangeShader>().Select();
                    if (highlight.CompareTag("ShopItem"))
                    { // show the range that the shop item can influence
                        ShopInstallManager.Instance.ShowRange(ShopInstallManager.Instance.GetShopItem(highlight));
                    }
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {

            if (highlight)
            {
                if (highlight.CompareTag("ShopItem"))
                { // hide the range that the shop item can influence
                    ShopInstallManager.Instance.HideRange(ShopInstallManager.Instance.GetShopItem(highlight));
                }

                if (selection != null)
                {
                    selection.GetComponent<ChangeShader>().DisSelect();
                }
                selection = raycastHit.transform;
                selection.GetComponent<ChangeShader>().ChangeColor(Settings.selectedColor);
                selection.GetComponent<ChangeShader>().Select();
                if (selection.GetComponent<SingleItemUpgradeUIOpener>())
                {
                    selection.GetComponent<SingleItemUpgradeUIOpener>().OpenPopup(selection);
                }
                else
                    GetComponent<BuildingInfoPopupOpener>().OpenPopup(selection); // open the popup
                // BuildingManager.Instance.BuildingPrinter(BuildingManager.Instance.GetBuildingName(selection.gameObject));
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.GetComponent<ChangeShader>().DisSelect();
                    selection = null;
                }
            }
        }

    }



}
