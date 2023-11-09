using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCRandomMoving : MonoBehaviour
{
    private Vector3 initPos;
    public float movingRadius = 5f;
    bool inConversation = false;
    private NavMeshAgent agent;
    private Vector3 velocity;

    private void OnEnable() {
        agent = GetComponent<NavMeshAgent>();
        initPos = transform.position;
        RandomMove();
    }

    void RandomMove()
    {
        StartCoroutine(moveTo());
    }

    IEnumerator moveTo()
    {
        float x = Random.Range(-movingRadius, movingRadius);
        float z = Random.Range(-movingRadius, movingRadius);
        Vector3 targetPos = initPos + new Vector3(x, 0, z);
        NavMeshPath path = new NavMeshPath();
        while (!agent.CalculatePath(targetPos, path))
        {
            x = Random.Range(-movingRadius, movingRadius);
            z = Random.Range(-movingRadius, movingRadius);
            targetPos = initPos + new Vector3(x, 0, z);
            yield return null;
        }

        transform.LookAt(targetPos);
        agent.SetPath(path);
        SetNPCRandomState();

        while (agent.remainingDistance != 0)
        {
            yield return null;
        }
        RandomMove();
    }

    void SetNPCRandomState()
    {
        agent.speed = Random.Range(0.01f, 0.09f);
        if (agent.speed > 0.05f)
        {
            GetComponent<Animation>().CrossFade("run");
        }
        else
        {
            GetComponent<Animation>().CrossFade("walk");
        }
        GetComponent<Animation>().wrapMode = WrapMode.Loop;
    }

    public void EnterConversation()
    {
        StopAllCoroutines();
        inConversation = true;
        agent.isStopped = true;
        velocity = agent.velocity;
        agent.velocity = Vector3.zero;
        GetComponent<Animation>().CrossFade("dialog");
    }

    public void LeaveConversation()
    {
        inConversation = false;
        agent.isStopped = false;
        agent.velocity = velocity;
        RandomMove();
    }

    IEnumerator WaitForDialogFinish()
    {
        while (inConversation)
        {
            if (!agent.isStopped)
                agent.ResetPath();
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}