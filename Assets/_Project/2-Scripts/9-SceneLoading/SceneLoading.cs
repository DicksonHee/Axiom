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
        // foreach (ScenePair pair in scenePairs)
        // {
        //     bool shouldLoadScene = false;
        //     foreach (string name in pair.sceneName)
        //     {
        //         if (name == sceneToLoad)
        //         {
        //             shouldLoadScene = true;
        //             break;
        //         }
        //     }
        //
        //     pair.gameObject.SetActive(shouldLoadScene);
        // }

        foreach (SceneGroup group in sceneGroups)
        {
            if (group.thisScene == sceneToLoad)
            {
                foreach (KeyValuePair<string, GameObject> entry in pairDict)
                {
                    if (group.sceneNames.Contains(entry.Key)) entry.Value.SetActive(true);
                    else entry.Value.SetActive(false);
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
    public List<string> sceneNames;
}