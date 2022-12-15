using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAScene : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        SceneLoad_Manager.LoadSpecificScene(sceneToLoad);
    }
}
