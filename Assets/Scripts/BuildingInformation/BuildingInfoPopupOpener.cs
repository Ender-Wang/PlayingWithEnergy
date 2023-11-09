using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class BuildingInfoPopupOpener : PopupOpener
    {
        Dictionary<string, Dictionary<string, string>> buildingInfo;

        // do not use this function directly
        public override void OpenPopup()
        {
            base.OpenPopup();
        }

        // open the popup with information of the selected building
        public void OpenPopup(Transform selectedBuilding)
        {
            OpenPopup();
            buildingInfo = BuildingLoader.Instance.getBuildingInfo(selectedBuilding.gameObject);
            string buildingName = BuildingManager.Instance.GetBuildingName(selectedBuilding.gameObject);
            Building building = BuildingManager.Instance.GetBuildingState(buildingName);
            m_popup.GetComponent<BuildingInfoPopup>().SetBuildingInfoPopup(buildingInfo, building); // set up the information of the building into the popup
        }
    }
}
