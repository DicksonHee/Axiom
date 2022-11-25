using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class SimpleAnimations : MonoBehaviour
{
    public List<GameObject> objectsToRotate;
    public List<Vector3> directionToRotate;
    public List<SpinDirection> spinDirections;
    public List<float> duration;

    public List<GameObject> objectsToScale;
    public List<GameObject> objectsToTransform;

    private void Start()
    {
        Scale();

        for (int ii = 0; ii < objectsToRotate.Count; ii++)
        {
            GameObject gameObject = objectsToRotate[ii];

            if (ii < directionToRotate.Count && directionToRotate[ii] != Vector3.zero)
            {
                gameObject.transform
                    .DOLocalRotate(directionToRotate[ii], duration[ii], RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
            else if(ii < spinDirections.Count && spinDirections[ii] != null)
            {
                Vector3 rotateVec = new Vector3(spinDirections[ii].x ? 360f : 0f, spinDirections[ii].y ? 360f : 0f, spinDirections[ii].z ? 360f : 0f);
                rotateVec *= Random.Range(0,2) == 0 ? -1f : 1f;
                float rotateDuration = duration[ii] * Random.Range(1f, 2f);
                gameObject.transform
                    .DOLocalRotate(rotateVec, rotateDuration, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    gameObject.transform
                        .DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), Random.Range(40f, 50f), RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                }
                else
                {
                    gameObject.transform
                        .DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)) * -1, Random.Range(40f, 50f), RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                }
            }
        }
    }

    public void Scale()
    {
        foreach(GameObject go in objectsToTransform)
        {
            go.transform.DOLocalMove(new Vector3(Random.Range(0f, 0.1f), Random.Range(0f, 0.1f), Random.Range(0f, 0.1f)), Random.Range(1f, 5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutElastic);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) StopTweens();
    }

    public void StopTweens()
    {
        foreach(GameObject go in objectsToRotate)
        {
            go.transform.DOKill();
        }
    }

    [Serializable]
    public class SpinDirection
    {
        public bool x;
        public bool y;
        public bool z;
    }
}