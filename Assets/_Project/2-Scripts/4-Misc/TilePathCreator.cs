using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TilePathCreator : MonoBehaviour
{
    public List<GameObject> tileObjects;
    public Vector3 spacing;
    public PathCreator pathCreator;
    public int tileWidthCount;
    public int tileLengthCount;

    public void CreatePath()
    {
        for(int ii = 0; ii < tileLengthCount; ii++)
        {
            Vector3 currentBezPathPoint = pathCreator.path.GetPointAtTime(ii / tileLengthCount);

            for (int jj = 0; jj < tileWidthCount; jj++)
            {
                GameObject randomGO = tileObjects[Random.Range(0, tileObjects.Count)];

            }
        }
    }
    
}
