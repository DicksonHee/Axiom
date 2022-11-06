using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public static SceneLoading current;

    public List<ScenePair> scenePairs;
    public string defaultScene;

    private void Start()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;

        Invoke(nameof(Init), 2f);
    }

    private void Init()
    {
        ChangeScene(defaultScene);
    }

    public void ChangeScene(string sceneToLoad)
    {
        foreach (ScenePair pair in scenePairs)
        {
            bool shouldLoadScene = false;
            foreach (string name in pair.sceneName)
            {
                if (name == sceneToLoad)
                {
                    shouldLoadScene = true;
                    break;
                }
            }

            pair.gameObject.SetActive(shouldLoadScene);
        }
    }

    // public void DisableScene(string sceneName)
    // {
    //     foreach(ScenePair pair in scenePairs)
    //     {
    //         if (pair.sceneName == sceneName) pair.gameObject.SetActive(false);
    //     }
    // }
    //
    // public void EnableScene(string sceneName)
    // {
    //     foreach (ScenePair pair in scenePairs)
    //     {
    //         if (pair.sceneName == sceneName) pair.gameObject.SetActive(true);
    //     }
    // }
}

[Serializable]
public class ScenePair
{
    public GameObject gameObject;
    public List<string> sceneName;
}