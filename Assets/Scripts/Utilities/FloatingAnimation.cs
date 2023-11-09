using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{

    float floatingHeight = 0.02f;
    float floatingSpeed = 0.5f;
    float rotationSpeed = 40f;
    float originalY;
    float maxY;

    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
        maxY = originalY + floatingHeight;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // change direction
        if (transform.position.y >= maxY)
        {
            floatingSpeed = -floatingSpeed;
        }
        else if (transform.position.y <= originalY)
        {
            floatingSpeed = -floatingSpeed;
        }

        // change height
        transform.position = new Vector3(transform.position.x, originalY + floatingHeight * Mathf.PingPong(Time.time * floatingSpeed, 1f), transform.position.z);
        // change rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationSpeed * Time.time, transform.rotation.eulerAngles.z);

    }
}
