using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTriggers : MonoBehaviour
{
    public string sceneToUnload;
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneLoading.current.EnableScene(sceneToLoad);
            SceneLoading.current.DisableScene(sceneToUnload);
        }
    }
}
