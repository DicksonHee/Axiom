using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentStuffInitialiser : MonoBehaviour
{
    public GameObject persistentStuff;

    private void Awake()
    {
        if (!GameObject.Find("PersistentStuff")) return;

        Instantiate(persistentStuff);
    }
}
