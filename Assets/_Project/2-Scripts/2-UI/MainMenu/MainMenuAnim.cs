using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuAnim : MonoBehaviour
{
    public Transform centerPoint;
    public float radius;
    public int spawnAmount;
    public float rotationSpeed;
    public ObjectAnimMode mode;
    public AnimationCurve curve;
    
    public GameObject spawnObject;

    private List<GameObject> spawnedObjectsList = new();
    private float currentDegrees;

    private void Awake()
    {
        for (int ii = 0; ii < spawnAmount; ii++)
        {
            spawnedObjectsList.Add(Instantiate(spawnObject));
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShapeRotation();
        
        if(Input.GetKeyDown(KeyCode.Space)) ObjectRotation_CO(new Vector3(360f, 0,360f), 2f, mode);

        currentDegrees += Time.deltaTime * rotationSpeed;
    }

    private void ShapeRotation()
    {
        for (int ii = 0; ii < spawnedObjectsList.Count; ii++)
        {
            float currentRad = (ii * (360f/spawnedObjectsList.Count) + currentDegrees) * Mathf.Deg2Rad;
            spawnedObjectsList[ii].transform.position = centerPoint.position + new Vector3(Mathf.Sin(currentRad), Mathf.Cos(currentRad)) * (radius);
        }
    }

    private void ObjectRotation_CO(Vector3 rotation, float duration, ObjectAnimMode animMode)
    {
        switch (animMode)
        {
            case ObjectAnimMode.StartToEnd:
                ObjectRotation_StartToEnd(rotation, duration);
                break;
            case ObjectAnimMode.EndToStart:
                ObjectRotation_EndToStart(rotation, duration);
                break;
            case ObjectAnimMode.StartAndEnd:
                ObjectRotation_StartAndEnd(rotation, duration);
                break;
            case ObjectAnimMode.All:
                ObjectRotation_All(rotation, duration);
                break;
            case ObjectAnimMode.Random:
                ObjectRotation_Random(rotation, duration);
                break;
        }
    }

    private void ObjectRotation_StartToEnd(Vector3 rotation, float duration) => StartCoroutine(ORStartToEnd_CO(rotation, duration));

    private IEnumerator ORStartToEnd_CO(Vector3 rotation, float duration)
    {
        foreach (GameObject go in spawnedObjectsList)
        {
            yield return new WaitForSeconds(0.01f);
            go.transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
        }
    }
    
    private void ObjectRotation_EndToStart(Vector3 rotation, float duration) => StartCoroutine(OREndToStart_CO(rotation, duration));

    private IEnumerator OREndToStart_CO(Vector3 rotation, float duration)
    {
        for (int ii = spawnedObjectsList.Count - 1; ii >= 0; ii--)
        {
            yield return new WaitForSeconds(0.01f);
            spawnedObjectsList[ii].transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
        }
    }
    
    private void ObjectRotation_StartAndEnd(Vector3 rotation, float duration) => StartCoroutine(ORStartAndEnd_CO(rotation, duration));

    private IEnumerator ORStartAndEnd_CO(Vector3 rotation, float duration)
    {
        for (int ii = 0; ii < spawnedObjectsList.Count/2; ii++)
        {
            yield return new WaitForSeconds(0.01f);
            spawnedObjectsList[ii].transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
            spawnedObjectsList[spawnedObjectsList.Count - 1 - ii].transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
        }
    }

    private void ObjectRotation_All(Vector3 rotation, float duration)
    {
        foreach (var t in spawnedObjectsList)
        {
            t.transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
        }
    }

    private void ObjectRotation_Random(Vector3 rotation, float duration) => StartCoroutine(ORRandom_CO(rotation, duration));
    
    private IEnumerator ORRandom_CO(Vector3 rotation, float duration)
    {
        List<int> randList = new();
        for(int ii = 0; ii < spawnedObjectsList.Count; ii++) randList.Add(ii);

        for (int ii = 0; ii < randList.Count; ii++)
        {
            int temp = randList[ii];
            int randomIndex = Random.Range(ii, spawnedObjectsList.Count);
            randList[ii] = randList[randomIndex];
            randList[randomIndex] = temp;
        }

        foreach (int num in randList)
        {
            yield return new WaitForSeconds(0.01f);
            spawnedObjectsList[num].transform.DORotate(rotation, duration, RotateMode.FastBeyond360);
        }
    }
}

public enum ObjectAnimMode
{
    StartToEnd,
    EndToStart,
    StartAndEnd,
    All,
    Random
}