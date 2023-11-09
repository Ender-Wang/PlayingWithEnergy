using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingAnimation : MonoBehaviour
{
    public float scaleDuration = 0.5f;
    public float moveDuration = 1f;
    public Vector3 targetPosition;
    public AnimationCurve scaleCurve;

    private Vector3 initialScale;
    Vector3 initialPosition;
    float time;
    float moveTimer;

    private void Start()
    {
        initialScale = transform.localScale;
        time = 0;
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        time += Time.deltaTime;
        float t = Mathf.PingPong(Time.time, scaleDuration) / scaleDuration;
        float scaleValue = scaleCurve.Evaluate(t);
        transform.localScale = initialScale * scaleValue;

        if (time >= 1f)
        {
            MoveToTargetPosition();
        }
    }

    private void MoveToTargetPosition()
    {
        moveTimer += Time.deltaTime / moveDuration;
        transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, moveTimer);

        if (moveTimer >= 1f)
        {
            Destroy(gameObject);
        }
    }

}
