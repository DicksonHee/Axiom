using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private float yRot;

    private void Awake()
    {
       yRot = Random.Range(0.01f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, yRot, 0 * Time.deltaTime, Space.World);
    }
}
