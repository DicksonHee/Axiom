using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UILineRenderer : MonoBehaviour
{
    public LineRenderer[] lineList;
    public Gradient initialGradient;
    
    private List<Vector3>[] positionsList; // Array of lists of vector3 positions of lines
    private List<float>[] segmentDurations; // Array of lists of time required to lerp between line segments

    private void Awake()
    {
        Initialise();
        
        InvokeRepeating(nameof(AnimateLine), 0, 0.35f);
    }

    public void StopAnimations()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
    
    private void Initialise()
    {
        segmentDurations = new List<float>[lineList.Length];
        positionsList = new List<Vector3>[lineList.Length];
        
        // Store all line renderer positions into positions list
        for (int ii = 0; ii < lineList.Length; ii++)
        {
            lineList[ii].gameObject.SetActive(false);
            positionsList[ii] = new List<Vector3>();
                
            for (int jj = 0; jj < lineList[ii].positionCount; jj++)
            {
                positionsList[ii].Add(lineList[ii].GetPosition(jj));
            }
        }
        
        // Iterate through each line in the list
        for (int jj = 0; jj < lineList.Length; jj++)
        {
            segmentDurations[jj] = new List<float>();
            float[] segmentLength = new float[lineList.Length - 1];
            float lineLength = 0f;

            // Get length of each line segment and find the total length of all segments
            for (int ii = 0; ii < lineList[jj].positionCount - 1; ii++)
            {
                segmentLength[ii] = Vector3.Distance(positionsList[jj][ii] , positionsList[jj][ii+1]);
                lineLength += segmentLength[ii];
            }
        
            // Find the time it takes for each segment to lerp evenly
            for (int ii = 0; ii < segmentLength.Length; ii++)
            {
                segmentDurations[jj].Add((segmentLength[ii] / lineLength) * 2f);
            }
        }
    }
    
    private void AnimateLine()
    {
        int randomIndex = Random.Range(0, lineList.Length);
        if(lineList[randomIndex] != null) StartCoroutine(AnimateLine_CO(randomIndex));
    }
    
    private IEnumerator AnimateLine_CO(int index)
    {
        LineRenderer line = lineList[index];
        List<Vector3> posList = positionsList[index];
        Gradient tempGradient = new Gradient();
        
        int positionCount = posList.Count;
        lineList[index] = null;
        tempGradient.SetKeys(initialGradient.colorKeys, initialGradient.alphaKeys);
        
        line.colorGradient = initialGradient;
        line.gameObject.SetActive(true);

        // Lerp the positions
        float startTime;
        for (int ii = 0; ii < positionCount - 1; ii++)
        {
            startTime = Time.time;

            Vector3 startPosition = positionsList[index][ii];
            Vector3 endPosition =  positionsList[index][ii + 1];

            Vector3 pos = startPosition;

            while (pos != endPosition)
            {
                float duration = (Time.time - startTime) / segmentDurations[index][ii];
                pos = Vector3.Lerp(startPosition, endPosition, duration);

                for (int jj = ii + 1; jj < positionCount; jj++)
                {
                    line.SetPosition(jj, pos);
                }

                yield return null;
            }
        }

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 0f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 1f;

        startTime = Time.time;
        while (Time.time - startTime < 0.5f)
        {
            alphaKeys[0].alpha = 1 - (Time.time - startTime) * 2f;
            alphaKeys[0].time = (Time.time - startTime) * 2f;
            tempGradient.SetKeys(tempGradient.colorKeys, alphaKeys);
            line.colorGradient = tempGradient;
            yield return null;
        }
        
        startTime = Time.time;
        while (Time.time - startTime < 0.5f)
        {
            alphaKeys[1].alpha = 1 - (Time.time - startTime) * 2f;
            tempGradient.SetKeys(tempGradient.colorKeys, alphaKeys);
            line.colorGradient = tempGradient;
            yield return null;
        }

        line.gameObject.SetActive(false);
        lineList[index] = line;
    }
}
