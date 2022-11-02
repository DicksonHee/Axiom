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
    public List<float> duration;

    private void Start()
    {
        for (int ii = 0; ii < objectsToRotate.Count; ii++)
        {
            GameObject gameObject = objectsToRotate[ii];

            if (ii < directionToRotate.Count && directionToRotate[ii] != Vector3.zero)
            {
                gameObject.transform
                    .DOLocalRotate(directionToRotate[ii], duration[ii], RotateMode.FastBeyond360)
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
}
