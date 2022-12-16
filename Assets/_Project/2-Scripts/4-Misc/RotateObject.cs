using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private float axisRot;

    [SerializeField] private bool xAxis = false;
    [SerializeField] private bool yAxis = false;
    [SerializeField] private bool zAxis = false;

    private void Awake()
    {
        axisRot = Random.Range(0.01f, 0.05f);

        if (!xAxis && !yAxis && !zAxis)
        {
            yAxis = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (xAxis)
        {
            gameObject.transform.Rotate(axisRot, 0, 0 * Time.deltaTime, Space.World);
        }
        else if (yAxis)
        {
            gameObject.transform.Rotate(0, axisRot, 0 * Time.deltaTime, Space.World);
        }
        else if (zAxis)
        {
            gameObject.transform.Rotate(0, 0, axisRot * Time.deltaTime, Space.World);
        }
    }
}
