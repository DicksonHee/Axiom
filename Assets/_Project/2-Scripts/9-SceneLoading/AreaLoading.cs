using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Axiom.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaLoading : MonoBehaviour
{
    public static AreaLoading current;

    public List<ScenePair> scenePairs;
    public List<SceneGroup> sceneGroups;
    public string defaultScene;

    private Dictionary<string, GameObject> pairDict = new();

    private void Start()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;

        foreach (ScenePair scenePair in scenePairs)
        {
            pairDict.Add(scenePair.sceneName, scenePair.gameObject);
        }
        
        Invoke(nameof(Init), 2f);
    }

    private void Init()
    {
        ChangeScene(defaultScene);
    }

    public void ChangeScene(string sceneToLoad)
    {
        foreach (SceneGroup group in sceneGroups)
        {
            if (group.thisScene == sceneToLoad)
            {
                PostProcessingActions.current.ChangeVolume(group.areaName);
                foreach (KeyValuePair<string, GameObject> entry in pairDict)
                {
                    entry.Value.SetActive(group.sceneNames.Contains(entry.Key));
                }
            }
        }
    }
}

[Serializable]
public class ScenePair
{
    public GameObject gameObject;
    public string sceneName;
}

[Serializable]
public class SceneGroup
{
    public string thisScene;
    public AreaName areaName;
    public List<string> sceneNames;
}