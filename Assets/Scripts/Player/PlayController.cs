using UnityEngine;
using UnityEngine.AI;
using MalbersAnimations.Selector;

public class PlayerController : MonoBehaviour
{
    // Note: this script is attached to the player, the player is controlled by the keyboard
    //  you can set the speed and jump height in the inspector
    private NavMeshAgent agent;
    new private Animation animation;
    private CharacterController controller;
    public float moveSpeed = 15f;
    public float jumpHeight = 2f;

    public Camera PlayerCamera;

    private bool isInFPS = false;

    void Start()
    {
        // set player position and orientation from last exit
        Vector3 initLocation = ES3.Load<Vector3>("FPS location", "Player/FPS", Vector3.zero);
        if (initLocation != Vector3.zero)
        {
            this.transform.position = initLocation;
            transform.forward = ES3.Load<Vector3>("FPS faceTo", "Player/FPS", Vector3.zero);
        }

        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
    }

    // save player position and orientation before exit
    void OnDestroy()
    {
        ES3.Save<Vector3>("FPS location", transform.position, "Player/FPS");
        ES3.Save<Vector3>("FPS faceTo", transform.forward, "Player/FPS");
    }

    void Update()
    {
        if (animation == null)
        {
            // animation is from the child object "Character"
            animation = transform.Find("Character").GetChild(0).GetComponent<Animation>();
        }

        if (isInFPS)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;

            Vector3 cameraAngle = PlayerCamera.transform.rotation.eulerAngles;
            if (horizontal == 0 && vertical == 0)
            {
                animation.CrossFade("idle");
                transform.rotation = Quaternion.Euler(0f, cameraAngle.y, 0f);
            }
            if (vertical != 0 && horizontal == 0)
            {
                transform.rotation = Quaternion.Euler(0f, cameraAngle.y, 0f);
            }

            if (horizontal != 0f)
            {
                if (vertical > 0f)
                    transform.rotation = Quaternion.Euler(0f, cameraAngle.y + 45f * horizontal, 0f);
                else if (vertical < 0f)
                    transform.rotation = Quaternion.Euler(0f, cameraAngle.y - 45f * horizontal, 0f);
                else
                {
                    transform.rotation = Quaternion.Euler(0f, cameraAngle.y + 90f * horizontal, 0f);
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
                    animation.CrossFade("run");
                }
            }
            if (vertical > 0f)
            {
                transform.Translate(Vector3.forward * vertical, Space.Self);
                animation.CrossFade("run");
            }
            if (vertical < 0f)
            {
                transform.Translate(Vector3.forward * vertical / 2, Space.Self);
                animation.CrossFade("walk");
            }
        }
    }

    public void IsInFPS(bool isInFPS)
    {
        this.isInFPS = isInFPS;
    }


    public void StandStill(Vector3 position)
    {
        transform.position = position;
        animation.CrossFade("idle");
    }

}
