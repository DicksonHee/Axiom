using System;
using System.Collections;
using System.Collections.Generic;
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
        foreach (ScenePair pair in scenePairs)
        {
            if (pair.sceneName != defaultScene) pair.gameObject.SetActive(false);
            else pair.gameObject.SetActive(true);
        }
    }

    public void DisableScene(string sceneName)
    {
        foreach(ScenePair pair in scenePairs)
        {
            if (pair.sceneName == sceneName) pair.gameObject.SetActive(false);
        }
    }

    public void EnableScene(string sceneName)
    {
        foreach (ScenePair pair in scenePairs)
        {
            if (pair.sceneName == sceneName) pair.gameObject.SetActive(true);
        }
    }
}

[Serializable]
public class ScenePair
{
    public string sceneName;
    public GameObject gameObject;
}