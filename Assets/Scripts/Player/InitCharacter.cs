using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitCharacter : MonoBehaviour
{
    public List<GameObject> avatars;
    // Start is called before the first frame update
    /// <summary>
    /// this function will set avatar of player from last exit
    /// </summary>
    void Start()
    {
        GameObject currentAvatar = transform.GetChild(0).gameObject;
        string lastAvatar = ES3.Load<string>("FPS avatar", "Player/FPS", currentAvatar.name);
        if (!string.Equals(currentAvatar.name, lastAvatar))
        {
            Quaternion faceTo = currentAvatar.transform.rotation;
            Destroy(currentAvatar);
            var character = avatars.Find(e => string.Equals(e.name, lastAvatar));
            if (character != null)
            {
                currentAvatar = Instantiate(character, this.transform);
            }
            else
            {   
                Debug.Log("** last avatar: " + lastAvatar);
                currentAvatar = Instantiate(Resources.Load<GameObject>("Achievement Resources/Small Tasks/NPCs/" + lastAvatar), this.transform);
                currentAvatar.transform.localScale = new Vector3(1f, 1f, 1f);
                Destroy(currentAvatar.GetComponent<NPCRandomMoving>());
                Destroy(currentAvatar.GetComponent<NavMeshAgent>());
            }
            currentAvatar.name = currentAvatar.name.Replace("(Clone)", "");
            currentAvatar.transform.rotation = faceTo;
        }
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        ES3.Save<string>("FPS avatar", transform.GetChild(0).name, "Player/FPS");
    }
}
