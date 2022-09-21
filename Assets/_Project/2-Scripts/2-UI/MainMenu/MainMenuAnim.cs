using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Axiom.UI.MainMenu
{
    public class MainMenuAnim : MonoBehaviour
    {
        public Transform spawnParent;
    
        public float radius = 5;
        public int spawnAmount = 100;
        public float rotationDuration = 5f;

        public ObjectAnimMode mode;
        public GameObject spawnObject;
        public Color redCol;
        public Color blueCol;

        private readonly List<GameObject> spawnedObjectsList = new();
        private float currentDegrees;
        private Vector3 initialCenterPoint;
        private Vector3 currentCenterPoint;
    
        private void Awake()
        {
            initialCenterPoint = transform.position;
            currentCenterPoint = initialCenterPoint;
        
            for (int ii = 0; ii < spawnAmount; ii++)
            {
                float currentRad = ii * (360f/spawnAmount) * Mathf.Deg2Rad;
                GameObject spawnedObject = Instantiate(spawnObject, spawnParent.transform);
                spawnedObject.transform.position = currentCenterPoint + new Vector3(Mathf.Sin(currentRad), Mathf.Cos(currentRad)) * radius;
                spawnedObjectsList.Add(spawnedObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int ii = 0; ii < spawnedObjectsList.Count; ii++)
            {
                float currentRad = (ii * (360f/spawnAmount) + Time.time * rotationDuration) * Mathf.Deg2Rad;
                float xVal = Mathf.Sin(currentRad);
                float yVal = Mathf.Cos(currentRad);
                if (xVal == 0) xVal = 0.001f;
                float zVal = Mathf.Tan(yVal/xVal);
                spawnedObjectsList[ii].transform.position = currentCenterPoint + new Vector3(xVal, yVal, zVal) * radius;
            }
        }

        public void ChangeColor()
        {
            foreach (var t in spawnedObjectsList)
            {
                Color targetCol = Random.Range(0, 2) == 0 ? redCol : blueCol;
                t.GetComponent<MeshRenderer>().material.DOColor(targetCol, Random.Range(0.5f, 1f));
                t.transform.DOLocalRotate(new Vector3(0, 0, 360), 2f, RotateMode.LocalAxisAdd).SetLoops(-1);
            }
        }

        private IEnumerator LerpEmission_CO(MeshRenderer mesh, Color targetCol)
        {
            Color initialCol = mesh.material.GetColor("_EmissionColor");
            float duration = 0f;

            while (duration < 0.5f)
            {
                duration += Time.deltaTime;
                Color lerpCol = Color.Lerp(initialCol, targetCol, duration / 0.5f);
                mesh.material.SetColor("_BaseColor", lerpCol);
                mesh.material.SetColor("_EmissionColor", lerpCol * 2);
                yield return null;
            }
        }

        public void ObjectRotation(Vector3 rotation, float duration, ObjectAnimMode animMode)
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

        private void ObjectRotation_Random(Vector3 rotation, float duration) => ORRandom_CO(rotation, duration);
    
        private void ORRandom_CO(Vector3 rotation, float duration)
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
}