using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingTriggers : MonoBehaviour
{
    public string sceneToLoad;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0)) LoadScene();
    }

    public void LoadScene()
    {
        SceneLoad_Manager.LoadSpecificScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }
}