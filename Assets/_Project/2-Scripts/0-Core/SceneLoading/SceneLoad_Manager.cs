using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoad_Manager
{
    private static List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    private static List<AsyncOperation> scenesUnloading = new List<AsyncOperation>();

    public static void LoadSpecificScene(string sceneName)
    {
        StaticMBLoader.Init_StaticMB();
        List<string> scenesToLoad = new List<string>();
        List<string> scenesToUnload = new List<string>();

        scenesToLoad.Add(sceneName);
        scenesToUnload.Add(SceneManager.GetActiveScene().name);

        StaticMBLoader._staticMb.StartCoroutine(ScenesLoading(scenesToLoad, scenesToUnload));
    }

    private static void LoadScene(string sceneName)
    {
        scenesLoading.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    private static void UnloadScene(string sceneName)
    {
        scenesUnloading.Add(SceneManager.UnloadSceneAsync(sceneName));
    }

    private static IEnumerator ScenesLoading(List<string> scenesToLoad, List<string> scenesToUnload)
    {
        if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
        while(!SceneManager.GetSceneByName("Load_Scene").isLoaded)
        {
            yield return null;
        }
        if(scenesLoading.Count > 0) scenesLoading.Clear();

        yield return new WaitForSeconds(1f);
        LoadScreen.current.SetWhite();

        foreach (string sceneName in scenesToUnload) UnloadScene(sceneName);
        
        bool allUnloaded = false;
        while (!allUnloaded)
        {
            allUnloaded = true;
            foreach (AsyncOperation op in scenesUnloading)
            {
                if (!op.isDone) allUnloaded = false;
            }
            yield return null;
        }
        scenesUnloading.Clear();
        
        foreach (string sceneName in scenesToLoad) LoadScene(sceneName);
        bool allLoaded = false;
        while (!allLoaded)
        {
            allLoaded = true;
            foreach (AsyncOperation op in scenesLoading)
            {
                if (!op.isDone) allLoaded = false;
            }
            yield return null;
        }
        scenesLoading.Clear();
        
        LoadScreen.current.SetClear();
    }
}