using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        GameObject go = GameObject.Find("PersistentStuff");
        if (go != gameObject) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
