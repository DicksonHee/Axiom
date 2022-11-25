using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLoadTriggers : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AreaLoading.current.ChangeScene(sceneToLoad);
        }
    }
}
