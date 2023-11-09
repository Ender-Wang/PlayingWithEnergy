using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPickerTrees : MonoBehaviour
{
    public GameObject positionsRoot;
    // Start is called before the first frame update
    void Start()
    {
        getPositions();
    }

    /// <summary>
    /// Get all positions of the children of the positionsRoot object. Used to get all the positions of the trees in the scene, which are then used as the spawn points for the NPC and objects in the game.
    /// </summary>
    void getPositions()
    {
        Transform[] positions = positionsRoot.GetComponentsInChildren<Transform>(true);
        List<string> positionsOutput = new List<string>();
        foreach (Transform position in positions)
        {
            positionsOutput.Add("{ \"location\": [" + position.position.x + ", " + position.position.y + ", " + position.position.z + "] },");
        }
        Debug.Log(string.Join("\n", positionsOutput.ToArray()));
    }
}
