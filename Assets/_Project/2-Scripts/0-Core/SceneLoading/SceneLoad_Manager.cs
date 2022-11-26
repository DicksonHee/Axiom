using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoad_Manager
{
    public static bool Busy => scenesLoading.Count > 0 || scenesUnloading.Count > 0;

    private static List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    private static List<AsyncOperation> scenesUnloading = new List<AsyncOperation>();

    public static void LoadSpecificScene(string sceneName)
    {
        StaticMBLoader.Init_StaticMB();
        List<string> scenesToLoad = new List<string>();
        List<string> scenesToUnload = new List<string>();

        scenesToLoad.Add(sceneName);
        for (int ii = 0; ii < SceneManager.sceneCount; ii++)
        {
            if (SceneManager.GetSceneAt(ii).name != "Load_Scene")
            {
                scenesToUnload.Add(SceneManager.GetSceneAt(ii).name);
            }
        }
        //scenesToUnload.Add(SceneManager.GetActiveScene().name);

        StaticMBLoader._staticMb.StartCoroutine(ScenesLoading(sceneName, scenesToLoad, scenesToUnload));
    }

    private static void LoadScene(string sceneName)
    {
        scenesLoading.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    private static void UnloadScene(string sceneName)
    {
        scenesUnloading.Add(SceneManager.UnloadSceneAsync(sceneName));
    }

    private static IEnumerator ScenesLoading(string newActiveScene, List<string> scenesToLoad, List<string> scenesToUnload)
    {
        if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
        while(!SceneManager.GetSceneByName("Load_Scene").isLoaded)
        {
            yield return null;
        }
        if(scenesLoading.Count > 0) scenesLoading.Clear();

        LoadScreen.current.SetWhite();
        yield return new WaitForSeconds(1f);

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
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newActiveScene));
        
        LoadScreen.current.SetClear();
    }
}