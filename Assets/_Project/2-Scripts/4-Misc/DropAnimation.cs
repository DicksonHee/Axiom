using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAnimation : MonoBehaviour
{
    public List<GameObject> objects;

    public void StartDrop()
    {
        foreach(GameObject obj in objects)
        {
            if (obj == null) continue;
            obj.transform.DOMoveY(-1000f, Random.Range(15f, 25f)).SetDelay(Random.Range(0f, 5f)).SetEase(Ease.InOutCubic);
        }
    }
}
