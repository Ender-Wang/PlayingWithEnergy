using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using DG.Tweening;

public class ObjectWithUI : MonoBehaviour
{
    protected GameObject canvas;
    protected bool isShowing = false;
    public Transform m_camera;
    public GameObject followingUIChain { get; set; }
    public FollowingUI currentUI { get; set; }
    public FollowingUI oldUI { get; set; }

    protected Transform playerTransform;
    protected float detectionDistance = 0.15f;
    protected bool hasInteraction = false; // if the object has interaction, need to be overrided in the child class
    protected bool inInteraction = false; // if the object is in interaction

    private bool inDistance = false; // player is in the detection distance
    private bool trigger = false; // player has entered in the detection distance

    // Start is called before the first frame update
    void Awake()
    {
        InitUI();
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        Show(FollowingType.Name, gameObject.name);
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    protected void Update()
    {
        if (hasInteraction)
            DetectDistance(); // detect the distance between player and object
    }

    void LateUpdate()
    {
        if (isShowing)
        {
            if (m_camera != Camera.main.transform)
                m_camera = Camera.main.transform;
            canvas.transform.forward = m_camera.forward; // keep the canvas always facing the camera
        }

    }

    void InitUI()
    {
        // ---------- Init canvas
        canvas = new GameObject("Object UI");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        canvas.transform.SetParent(transform);
        float offset = 7f;
        if (GetComponent<BoxCollider>() != null)
            offset = GetComponent<BoxCollider>().size.y + 1;
        canvas.transform.localPosition = new Vector3(0, offset, 0);
        canvas.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        canvas.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1280);
        // ----------
        InitializeUIChain(); // initialize the UI chain
    }

    /// <summary>
    /// show the UI in the screen
    /// </summary>
    /// <param name="type"></param>
    /// <param name="request"></param>
    public void Show(FollowingType type, string request = "")
    {
        var followingUI = followingUIChain.GetComponent<NameUI>().Create(type, request); // call the responsiblity chain
        if (followingUI)
        {
            followingUI.UI.transform.SetParent(canvas.transform);
            followingUI.UI.transform.localPosition = Vector3.zero;
            followingUI.UI.transform.localScale = Vector3.one;
            followingUI.UI.transform.localRotation = Quaternion.identity;
            ChangeToUI(followingUI);
        }
        isShowing = true;
        m_camera = Camera.main.transform;
    }

    // Change the current showing UI to target UI
    void ChangeToUI(FollowingUI target)
    {
        target.UI.SetActive(true);
        if (currentUI)
        {
            oldUI = currentUI;
            if (currentUI != target)
            {
                currentUI.StopAllCoroutines(); // stop all animations
                currentUI.UI.transform.localScale = Vector3.one; // reset the scale
                currentUI.UI.SetActive(false);
            }
        }
        currentUI = target;
    }

    /// <summary>
    /// Change the original UI to target UI
    /// </summary>
    /// <param name="target"></param>
    protected void ChangeOriginUI(FollowingType type, string request = "")
    {
        oldUI = followingUIChain.GetComponent<NameUI>().Create(type, request);
        currentUI = oldUI;
    }

    protected void ChangeBackUI()
    {
        if (oldUI)
        {
            Show(oldUI.type);
            oldUI.UI.transform.localScale = Vector3.one; // reset the scale
        }
    }

    protected void HideUI()
    {
        if (currentUI)
        {
            currentUI.UI.SetActive(false);
        }
    }

    /// <summary>
    /// Add the following UIs to the chain
    /// </summary>
    void InitializeUIChain()
    {
        followingUIChain = new GameObject("FollowingUIChain");
        followingUIChain.transform.SetParent(canvas.transform);

        // TODO: add more UIs, to be continued...
        FollowingUI NameUI = followingUIChain.AddComponent<NameUI>();
        FollowingUI MarkUI = followingUIChain.AddComponent<MarkUI>();
        FollowingUI KeyBoardUI = followingUIChain.AddComponent<KeyBoardUI>();
        NameUI.Add(MarkUI).Add(KeyBoardUI);
    }

    /// <summary>
    /// Detect the distance between player and object, if player is near the object, do the interaction
    /// </summary>
    protected virtual void DetectDistance()
    {
        // change the UI when player is near the NPC
        float distanceSqr = (transform.position - playerTransform.position).sqrMagnitude;

        if (distanceSqr < detectionDistance * detectionDistance)
        {
            if (!inDistance)
            {
                Show(FollowingType.Keyboard, "F"); // when player is near the NPC, show the conversation button
                inDistance = true;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                HideUI(); // hide the UI
                if (!inInteraction)
                {
                    Interact();
                }
            }
            trigger = true;
        }
        else
        {
            inDistance = false;
            if (trigger)
            {
                ChangeBackUI(); // change back to show previews UI
                trigger = false;
            }
        }
    }

    /// <summary>
    /// Virtual function, to be overrided, implement the interaction action when player press the interact button (F)
    /// </summary>
    protected virtual void Interact() { }

    protected void ChangeOffset(float height)
    {
        canvas.transform.localPosition = new Vector3(0, height, 0);
    }

    protected void ChangeScale(float scale)
    {
        canvas.transform.localScale = new Vector3(scale, scale, scale);
    }
}
