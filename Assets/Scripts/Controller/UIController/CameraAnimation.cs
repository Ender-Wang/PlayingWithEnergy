using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Link to the camera which needs to be moved and rotated.
/// </summary>
public class CameraAnimation : MonoBehaviour
{
    [SerializeField]
    protected Vector3 targetPosition;
    [SerializeField]
    public Vector3 rotation = Vector3.zero;
    [SerializeField]
    protected float duration;


    private Camera m_camera;
    private bool m_orthographic = false;
    private float m_orthographicSize;
    private Transform target;
    private float fieldOfView = 0;

    // Start is called before the first frame update
    void Start()
    {
        duration = 0f;
        m_camera = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// API: Start moving the camera to the target position and rotating to the target direction.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="faceTo"></param>
    /// <param name="duration"></param>
    /// <param name="orthographics"></param>
    /// <param name="orthographicSize"></param>
    public void CameraMove(Vector3 targetPosition, Vector3 rotation, float duration, bool orthographics = false, float orthographicSize = 10f)
    {
        m_camera = GetComponent<Camera>();
        this.targetPosition = targetPosition;
        this.rotation = rotation;
        this.duration = duration;
        m_orthographic = orthographics;
        m_orthographicSize = orthographicSize;
        StartCoroutine(Animate());
    }

    /// <summary>
    /// API: Start moving the camera to the target position and rotating to the target direction.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="faceTo"></param>
    /// <param name="duration"></param>
    /// <param name="fieldOfView"></param>
    public void CameraMove(Vector3 targetPosition, Vector3 rotation, float duration, float fieldOfView)
    {
        m_camera = GetComponent<Camera>();
        this.targetPosition = targetPosition;
        this.rotation = rotation;
        this.duration = duration;
        this.fieldOfView = fieldOfView;
        StartCoroutine(Animate());
    }

    /// <summary>
    /// API: Start moving the camera to the target position and rotating to face the target.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    /// <param name="fieldOfView"></param>
    public void CameraMove(Vector3 targetPosition, Transform target, float duration, float fieldOfView = 0)
    {
        m_camera = GetComponent<Camera>();
        this.targetPosition = targetPosition;
        this.duration = duration;
        this.target = target;
        this.fieldOfView = fieldOfView;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        var ratio = 0.0f;
        var multiplier = 1.0f / (duration / 2);

        if (!m_orthographic && m_camera.orthographic) // when camera is orthographic and we want to disable it
        {
            while (ratio < 0.5f)
            {
                ratio += Time.deltaTime * multiplier;
                m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, m_orthographicSize, ratio);
                yield return null;
            }
            m_camera.orthographic = false;
            m_camera.orthographicSize = m_orthographicSize;
        }
        ratio = 0.0f;
        while (ratio < 0.5f) //! Attention: lerp function problem (0.5 returns final position)
        {
            ratio += Time.deltaTime * multiplier;
            if (m_camera != null)
            {

                yield return null;
                m_camera.transform.position = Vector3.Lerp(m_camera.transform.position, targetPosition, ratio);
                if (fieldOfView != 0) m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, fieldOfView, ratio);
                if (rotation != Vector3.zero)
                    m_camera.transform.localEulerAngles = Vector3.Lerp(m_camera.transform.localEulerAngles, rotation, ratio);
                else
                {
                    m_camera.transform.forward = -target.forward;
                }
            }
            yield return null;
        }
        // avoid the camera doesn't reach the target position and rotation
        m_camera.transform.position = targetPosition;
        if (fieldOfView != 0) m_camera.fieldOfView = fieldOfView;
        if (rotation != Vector3.zero)
            m_camera.transform.localEulerAngles = rotation;

        if (m_orthographic)
        {
            m_camera.orthographic = true;
            m_camera.orthographicSize = m_orthographicSize;
        }
    }

}
