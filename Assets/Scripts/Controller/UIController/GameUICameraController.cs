using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUICameraController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float zoomSpeed = 0.5f;
    public float resetDuration = 2f;

    private Vector3 rotationCenter;
    private Vector3 initialPosition; // Initial position of the camera
    private Quaternion initialRotation; // Initial rotation of the camera
    private bool resetting = false; // Flag to indicate if camera is resetting
    private float resetStartTime; // Time when camera reset started
    private Camera m_camera;
    private float lowerLimitOrthographicSize = 0.5f;
    private float upperLimitOrthographicSize = 15f;

    private void Start()
    {
        // Set the initial rotation center to the camera's current position
        rotationCenter = transform.position;
        m_camera = GetComponent<Camera>();

        // Store the initial position and rotation of the camera, used for resetting
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // If camera is in orthographic mode (used in shop mode) <-> camera is in perspective mode (used in game home mode)
        if (m_camera.orthographic)
        {
            LimitCameraPosition(0, 20, 6, 20, 0, 25);
            if (Input.GetMouseButton(1))
            {
                mouseTranslateCamera();
            }
            ZoomCamera();
            LimitOrthographicSize();
        }
        else
        {
            if (!resetting)
            {
                LimitCameraPosition(0, 20, 6, 20, 0, 25);
                ZoomCamera();

                // Hold Right mouse button and move mouse to move camera
                if (Input.GetMouseButton(1))
                {
                    mouseTranslateCamera();
                }

                // Check if camera reset button is pressed
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ResetCamera();
                }
            }
            else
            {
                AnimateCameraReset();
            }
        }
    }

    /// <summary>
    /// Limit the camera position to the map, based on camera position on the x, y, z axis
    /// </summary>
    /// <param name="xMin">Minimum x position</param>
    /// <param name="xMax">Maximum x position</param>
    /// <param name="yMin">Minimum y position</param>
    /// <param name="yMax">Maximum y position</param>
    /// <param name="zMin">Minimum z position</param>
    /// <param name="zMax">Maximum z position</param>
    private void LimitCameraPosition(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
    {
        //Parameters used in func LimitCameraPosition()
        bool lowerLimitPositionXFlag = false; //0
        bool upperLimitPositionXFlag = false; //20
        bool lowerLimitPositionYFlag = false; //6
        bool upperLimitPositionYFlag = false; //20
        bool lowerLimitPositionZFlag = false; //0
        bool upperLimitPositionZFlag = false; //25
        Vector3 lowerLimitPosition = new Vector3();
        Vector3 upperLimitPosition = new Vector3();
        int lowerLimitFieldOfView = 0;
        int upperLimitFieldOfView = 0;

        // Note down camera position and field of view when camera reaches the lower/upper limit
        // X: 0 <-> 20
        if (!lowerLimitPositionXFlag && transform.position.x < xMin)
        {
            lowerLimitPosition = transform.position;
            lowerLimitFieldOfView = (int)m_camera.fieldOfView;
            lowerLimitPositionXFlag = true;
        }
        if (!upperLimitPositionXFlag && transform.position.x > xMax)
        {
            upperLimitPosition = transform.position;
            upperLimitFieldOfView = (int)m_camera.fieldOfView;
            upperLimitPositionXFlag = true;
        }
        if (lowerLimitPositionXFlag || upperLimitPositionXFlag)
        {
            transform.position = new Vector3(lowerLimitPositionXFlag ? xMin : xMax, transform.position.y, transform.position.z);
            lowerLimitPositionXFlag = false;
            upperLimitPositionXFlag = false;
        }

        // Y: 6 <-> 20
        if (!lowerLimitPositionYFlag && transform.position.y < yMin)
        {
            lowerLimitPosition = transform.position;
            lowerLimitFieldOfView = (int)m_camera.fieldOfView;
            lowerLimitPositionYFlag = true;
        }
        if (!upperLimitPositionYFlag && transform.position.y > yMax)
        {
            upperLimitPosition = transform.position;
            upperLimitFieldOfView = (int)m_camera.fieldOfView;
            upperLimitPositionYFlag = true;
        }
        if (lowerLimitPositionYFlag || upperLimitPositionYFlag)
        {
            transform.position = new Vector3(transform.position.x, lowerLimitPositionYFlag ? yMin : yMax, transform.position.z);
            lowerLimitPositionYFlag = false;
            upperLimitPositionYFlag = false;
        }

        // Z: 0 <-> 25
        if (!lowerLimitPositionZFlag && transform.position.z < zMin)
        {
            lowerLimitPosition = transform.position;
            lowerLimitFieldOfView = (int)m_camera.fieldOfView;
            lowerLimitPositionZFlag = true;
        }
        if (!upperLimitPositionZFlag && transform.position.z > zMax)
        {
            upperLimitPosition = transform.position;
            upperLimitFieldOfView = (int)m_camera.fieldOfView;
            upperLimitPositionZFlag = true;
        }
        if (lowerLimitPositionZFlag || upperLimitPositionZFlag)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, lowerLimitPositionZFlag ? zMin : zMax);
            lowerLimitPositionZFlag = false;
            upperLimitPositionZFlag = false;
        }

        // Reset the flag when camera moves away from the limit
        if (transform.position.x >= xMin && transform.position.x < xMax)
        {
            lowerLimitPositionXFlag = false;
            upperLimitPositionXFlag = false;
        }
        if (transform.position.y >= yMin && transform.position.y < yMax)
        {
            lowerLimitPositionYFlag = false;
            upperLimitPositionYFlag = false;
        }
        if (transform.position.z >= zMin && transform.position.z < zMax)
        {
            lowerLimitPositionZFlag = false;
            upperLimitPositionZFlag = false;
        }
    }

    private void LimitOrthographicSize()
    {
        if (m_camera.orthographic && m_camera.orthographicSize < lowerLimitOrthographicSize)
        {
            m_camera.orthographicSize = lowerLimitOrthographicSize;
        }
        if (m_camera.orthographic && m_camera.orthographicSize > upperLimitOrthographicSize)
        {
            m_camera.orthographicSize = upperLimitOrthographicSize;
        }
    }

    /// <summary>
    /// Zoom the camera in/out based on mouse wheel movement.
    /// </summary>
    private void ZoomCamera()
    {
        // Get mouse wheel movement
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the zoom center position in the world space (current cursor location)
        Vector3 zoomCenter = GetCursorPosition();

        // Zoom in/out based on mouse wheel movement with zoom center as the pivot
        float zoomAmount = scroll * zoomSpeed;
        if (m_camera.orthographic)
        {
            m_camera.orthographicSize += zoomAmount * 100;
        }
        else if (m_camera.fieldOfView + zoomAmount > 0)
        {
            // Adjust camera position to maintain the zoom center
            Vector3 toZoomCenter = transform.position - zoomCenter;
            transform.Translate(toZoomCenter * zoomAmount, Space.World);
            m_camera.fieldOfView += zoomAmount;
        }
    }

    /// <summary>
    /// Get the cursor position in world space.
    /// </summary>
    /// <returns>The cursor position in world space.</returns>
    private Vector3 GetCursorPosition()
    {
        // Cast a ray from the camera to the cursor position
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast and check if it hits anything
        if (Physics.Raycast(ray, out hit))
        {
            // Return the hit point as the cursor position
            return hit.point;
        }

        // If the raycast doesn't hit anything, return the current mouse position in world space
        return m_camera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Get the rotation center position, which is the cursor position on the map.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRotationCenter()
    {
        // Cast a ray from the camera to the cursor position
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast and check if it hits anything
        if (Physics.Raycast(ray, out hit))
        {
            // Return the hit point as the rotation center
            return hit.point;
        }

        // If the raycast doesn't hit anything, return the current rotation center position
        return rotationCenter;
    }

    /// <summary>
    /// Reset the camera to its initial position and rotation.
    /// </summary>
    private void RotateCamera()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate the rotation center position in the world space
        rotationCenter = GetRotationCenter();

        // Rotate the camera based on mouse movement around the rotation center
        transform.RotateAround(rotationCenter, Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
        transform.RotateAround(rotationCenter, transform.right, -mouseY * rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Move camera XY plane in shop mode.
    /// </summary>
    private void mouseTranslateCamera()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Translate(mouseX * rotationSpeed * 10 * Time.deltaTime, 0, mouseY * rotationSpeed * 10 * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Reset the camera to its initial position and rotation.
    /// </summary>
    private void ResetCamera()
    {
        resetting = true;
        resetStartTime = Time.time;
    }

    /// <summary>
    /// Animate the camera reset.
    /// </summary>
    private void AnimateCameraReset()
    {
        // Calculate the progress of the reset animation
        float progress = (Time.time - resetStartTime) / resetDuration;

        // Clamp the progress between 0 and 1
        progress = Mathf.Clamp01(progress);

        // Calculate the new position and rotation of the camera
        Vector3 newPosition = Vector3.Lerp(transform.position, initialPosition, progress);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, initialRotation, progress);

        // Set the camera position and rotation
        transform.position = newPosition;
        transform.rotation = newRotation;

        // Check if the reset animation is complete
        if (progress >= 1f)
        {
            resetting = false;
        }
    }
}
