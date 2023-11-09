using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CreateCitizen : MonoBehaviour
{
    private GameObject[] prefabs;
    public int desiredNum = 10;
    new private Camera camera;
    void Start()
    {
        prefabs = Resources.LoadAll<GameObject>("Achievement Resources/Small Tasks/NPCs");
        camera = Camera.main;
        create();
        gameObject.SetActive(false);
    }

    void create()
    {
        int createdNum = 0;
        int layerMask = 1 << 6; // only against Ground layer
        Ray ray;
        int index = 0;
        while (createdNum < desiredNum)
        {
            float x = Random.Range(0f, Screen.width);
            float y = Random.Range(0f, Screen.height);
            ray = camera.ScreenPointToRay(new Vector3(x, y, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, camera.farClipPlane, layerMask))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    NavMeshHit nmhit;
                    if (NavMesh.SamplePosition(hit.point, out nmhit, 0.05f, NavMesh.AllAreas))
                    {
                        index = Random.Range(0, prefabs.Length);
                        GameObject go = Instantiate(prefabs[index], nmhit.position, Quaternion.identity, this.transform);
                        go.GetComponent<NavMeshAgent>().baseOffset = -1.4f;
                        // SetNPCRandomState(go);
                        createdNum++;
                    }
                }
            }
        }
    }

    void SetNPCRandomState(GameObject go)
    {
        go.GetComponent<NavMeshAgent>().speed = RandomSpeed();
        if (go.GetComponent<NavMeshAgent>().speed > 0.05f)
        {
            go.GetComponent<Animation>().CrossFade("run");
        }
        else
        {
            go.GetComponent<Animation>().CrossFade("walk");
        }
        go.GetComponent<Animation>().wrapMode = WrapMode.Loop;
    }

    float RandomSpeed()
    {
        return Random.Range(0.01f, 0.09f);
    }
}
