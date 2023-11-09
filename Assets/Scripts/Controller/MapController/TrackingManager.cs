using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrackingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Image CitizenMarkerImage;


    // Archivement system
    public Image NPCMarkerImage;
    public Image SubtaskNPCMarkerImage;
    public Image ObjectMarkerImage;
    

    public void AddBinnacleTrackedObjectScript(GameObject obj, MarkerType markerType)
    {
        if (obj.GetComponent<BinnacleTrackedObjectScript>() == null)
        {   
            Image radarMarkerImage = GetImage(markerType);
            obj.AddComponent<BinnacleTrackedObjectScript>().radarMarkerImage = radarMarkerImage;
        }
    }

    public Image GetImage(MarkerType markerType)
    {
        switch (markerType)
        {
            case MarkerType.NPC:
                return NPCMarkerImage;
            case MarkerType.Object:
                return ObjectMarkerImage;
            case MarkerType.SubTaskNPC:
                return SubtaskNPCMarkerImage;
            case MarkerType.Citizen:
                return CitizenMarkerImage;
            default:
                return null;
        }
    }

    public enum MarkerType
    {
        NPC,
        Object,
        SubTaskNPC,
        Citizen
    }
}
