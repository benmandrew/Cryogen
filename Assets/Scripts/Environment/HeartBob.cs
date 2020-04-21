using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBob : MonoBehaviour
{
    private float minHeight;
    private float maxHeight;

    void Start() {
        minHeight = 1.9f;
        maxHeight = 2.1f;
    }

    // Update is called once per frame
    void FixedUpdate() {
        float c = Mathf.Cos(Time.time);
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Lerp(minHeight, maxHeight, c * c),
            transform.position.z);
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            Time.time * 100.0f,
            transform.eulerAngles.z);
    }
}
