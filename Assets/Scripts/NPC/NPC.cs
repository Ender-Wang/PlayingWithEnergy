using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC : ObjectWithUI
{
    public Task task;
    Vector3 playerPosition;
    Vector3 NPCPosition;
    GameObject dialogCamera;
    public bool inConversation = false;
    public bool inDistance = false; // player is in the detection distance
    public bool trigger = false; // player has entered in the detection distance

    // public int round = 0;
    // public bool finishTaskOnDialogFinish = false;


    void OnEnable()
    {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        hasInteraction = true;
        dialogCamera = GameObject.Find("Dialog Camera");
    }

    private void Update()
    {
        base.Update();
        // RotationFix(); // fix rotation, avoid NPC fall down or fly away
        if (playerTransform == null)
        {
            return;
        }

    }

    /// <summary>
    /// Start the conversation, set up the scene and characters
    /// </summary>
    protected override void Interact()
    {
        base.Interact();
        inInteraction = true; // start conversation
        playerPosition = playerTransform.position; // record player position
        InitRotation = transform.forward;
        GetComponent<Animation>().CrossFade("dialog");
        GetComponent<Animation>().wrapMode = WrapMode.Loop;
        GetComponent<CharacterDialogSystem>()?.DialogStart();
        DialogCamera(); // create dialog camera
        StartCoroutine(WaitForDialogFinish()); // Wait for dialog finish
    }
    private Vector3 InitRotation;
    IEnumerator WaitForDialogFinish()
    {   
        while (!GetComponent<CharacterDialogSystem>().dialog.GetComponent<Dialog>().dialogFinished)
        {
            transform.LookAt(playerTransform);
            playerTransform.LookAt(transform);
            // stop player moving
            playerTransform.GetComponent<PlayerController>().StandStill(playerPosition);
            yield return null;
        }
        transform.forward = InitRotation;
        inInteraction = false;

        // if NPC is a task NPC, finish the task
        if (GetComponent<Task>() != null)
        {
            if (GetComponent<CharacterDialogSystem>().isDialogsFinished())
            {
                GetComponent<Task>().FinishTask(true);
            }
            else
            {
                ChangeOriginUI(FollowingType.Name, gameObject.name);
                GetComponent<Task>().FinishTask(false);
            }
        }
        
        StartCoroutine(DisableDialogCamera()); // destroy dialog camera
        GetComponent<Animation>().CrossFade("idle");
    }

    void RotationFix()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void DialogCamera()
    {   
        transform.LookAt(playerTransform);
        dialogCamera.GetComponent<Camera>().enabled = true;
        dialogCamera.transform.position = Camera.main.transform.position;
        dialogCamera.transform.rotation = Camera.main.transform.rotation;
        dialogCamera.GetComponent<CameraAnimation>().CameraMove(transform.position + transform.forward * 0.02f + new Vector3(0, 0.03f, 0), transform, 1f); // move camera to NPC
    }

    IEnumerator DisableDialogCamera()
    {
        dialogCamera.GetComponent<CameraAnimation>().CameraMove(Camera.main.transform.position, Camera.main.transform.rotation.eulerAngles, 1f, Camera.main.fieldOfView); // move camera back to player
        yield return new WaitForSeconds(1f);
        dialogCamera.GetComponent<CameraAnimation>().rotation = Vector3.zero;
        dialogCamera.GetComponent<Camera>().enabled = false;
        StopAllCoroutines();
    }
}
