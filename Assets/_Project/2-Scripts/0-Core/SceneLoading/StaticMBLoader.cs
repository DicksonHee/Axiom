using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMBLoader
{
    public class StaticMB : MonoBehaviour
    {
    }

    public static StaticMB _staticMb;

    public static void Init_StaticMB()
    {
        if (_staticMb != null) return;

        GameObject gameObject = new GameObject("StaticMB");
        _staticMb = gameObject.AddComponent<StaticMB>();
        GameObject.DontDestroyOnLoad(_staticMb);
    }
}