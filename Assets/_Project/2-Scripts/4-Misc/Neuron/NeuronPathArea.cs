using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronPathArea : MonoBehaviour
{
    public Bezier.Curve Curve => curve;

    [SerializeField] private Bezier.Curve curve;
    [SerializeField] private Vector3 areaSize;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color boxColour = Color.white;

    private Vector3 origin, maxPoint, minPoint;

    private void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        origin = transform.position + offset;
        maxPoint = origin + areaSize / 2;
        minPoint = origin - areaSize / 2;
    }

    public bool PointInArea(Vector3 point) =>
        point.x >= minPoint.x &&
        point.x <= maxPoint.x &&
        point.y >= minPoint.y &&
        point.y <= maxPoint.y &&
        point.z <= maxPoint.z &&
        point.z <= maxPoint.z;

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Transform active = UnityEditor.Selection.activeTransform;
        if (active != null && active != transform && !transform.IsChildOf(active))
            return;

        Initialise();

        Gizmos.color = boxColour;
        Gizmos.DrawWireCube(origin, areaSize);
    }
    #endif
}
