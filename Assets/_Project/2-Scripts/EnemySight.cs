using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemySight : MonoBehaviour
{
    public float pointBlankRange;
    public float midRange;
    public float farRange;
    public Vector3 pointBlankCube;
    public LayerMask playerLayer;
    public Transform player;
    public float maxFov;
    public float distance;
    public bool inView = false;

    private bool susInsideBox;
    private static Transform playerT;
    bool inLOS = false;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        playerT = player;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Check if player is inside box
        susInsideBox = Physics.BoxCast(transform.position, pointBlankCube, transform.forward, out hit,
            transform.rotation, farRange, playerLayer);
        if (susInsideBox)
        {
            Vector3 direction = playerT.position - transform.position;

            // Check if line of sight is not blocked
            bool hitSomething;
            RaycastHit rh;
            hitSomething = Physics.Raycast(transform.position, direction, out rh, farRange);

            if (rh.collider.tag == "Player")
            {
                inLOS = true;
            }
            else
            {
                inLOS = false;
            }


            if (hitSomething)
            {
                Color c;
                //Check if player is within fov
                if (inLOS && InFOV(player))
                {
                    inView = true;
                    c = Color.red;
                }
                else
                {
                    inView = false;
                    c = Color.cyan;
                }

                //Debugging
                Vector3 newDir = rh.point - transform.position;
                distance = newDir.magnitude;
                Debug.DrawRay(transform.position, newDir, c);
            }
        }
        else
        {
            inView = false;
        }
        //TO DO Check if enemy dead body is within view

    }

    private bool InFOV(Transform inspectable)
    {
        Vector3 displacement = inspectable.position - transform.position;
        float LookAngleThreshold = maxFov / 2;
        return

            Vector3.Angle(displacement, transform.forward) <= LookAngleThreshold; // Within look angle threshold
        //&&
        //Vector3.Angle (-displacement, inspectable.forward) <= RelativeAngleThreshold; // Within relative angle S
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * farRange);
        //point blank range
        DrawBoxCastBox(transform.position, pointBlankCube, transform.rotation, transform.forward, pointBlankRange,
            Color.red);

        //mid range
        DrawBoxCastBox(transform.position + transform.forward * pointBlankRange, pointBlankCube, transform.rotation,
            transform.forward, midRange - pointBlankRange, Color.yellow);

        //far range
        DrawBoxCastBox(transform.position + transform.forward * midRange, pointBlankCube, transform.rotation,
            transform.forward, farRange - midRange, Color.green);
    }






    //NOTHING IMPORTANT DOWN BELOW
    ////////////////////////////////////////////////////////////////////////////////////////////////
    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction,
        float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction,
        float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }

    public static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }

        public Vector3 localBackTopLeft
        {
            get { return -localFrontBottomRight; }
        }

        public Vector3 localBackTopRight
        {
            get { return -localFrontBottomLeft; }
        }

        public Vector3 localBackBottomLeft
        {
            get { return -localFrontTopRight; }
        }

        public Vector3 localBackBottomRight
        {
            get { return -localFrontTopLeft; }
        }

        public Vector3 frontTopLeft
        {
            get { return localFrontTopLeft + origin; }
        }

        public Vector3 frontTopRight
        {
            get { return localFrontTopRight + origin; }
        }

        public Vector3 frontBottomLeft
        {
            get { return localFrontBottomLeft + origin; }
        }

        public Vector3 frontBottomRight
        {
            get { return localFrontBottomRight + origin; }
        }

        public Vector3 backTopLeft
        {
            get { return localBackTopLeft + origin; }
        }

        public Vector3 backTopRight
        {
            get { return localBackTopRight + origin; }
        }

        public Vector3 backBottomLeft
        {
            get { return localBackBottomLeft + origin; }
        }

        public Vector3 backBottomRight
        {
            get { return localBackBottomRight + origin; }
        }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }

        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }
}
