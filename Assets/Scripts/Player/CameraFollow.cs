using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // target should have a PlayerController component
    public Transform target;

    [Header("the speed of moving camera")]
    public float smoothSpeed = 10f;

    [Header("the speed of zooming with mouse scroll wheel")]
    public float zoomSpeed = 1.25f;

    public float maxDistanceRatio = 1.2f;
    public float minDistanceRatio = 0.8f;

    private Vector3 offset; // the offset between camera and target
    public Vector3 player {get {return target.position;}}
    private float distance = 1f;
    private Vector3 initOffset = new Vector3(0f, 0.1f, -0.12f);

    private float rotationX = 0f;
    private float rotationY = 0f;
    void Start()
    {
        offset = initOffset;
    }

    void OnEnable()
    {
        target.GetComponent<PlayerController>().IsInFPS(true);
    }

    void OnDisable()
    {
        target.GetComponent<PlayerController>().IsInFPS(false);
    }

    void LateUpdate()
    {
        // if the right mouse button is pressed, the camera will rotate around the player
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            rotationX += mouseX * 5f;
            rotationY -= mouseY * 5f;
            rotationY = Mathf.Clamp(rotationY, -55f, 55f);
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
            offset = Matrix4x4.Rotate(rotation).MultiplyVector(initOffset) * distance;
        }

        // if Mouse ScrollWheel is pressed, the camera will zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {   
            distance += -scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistanceRatio, maxDistanceRatio);
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
            offset = Matrix4x4.Rotate(rotation).MultiplyVector(initOffset) * distance;
        }

        Vector3 desiredPosition = player + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.position = desiredPosition;
        transform.LookAt(player + new Vector3(0f,0.05f,0f));
    }
}
