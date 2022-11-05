using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class FilterNonStaticObjects : EditorWindow
{
    [MenuItem( "Custom/Select Non-Static" )]
    static void Init()
    {
        Transform parent = Selection.activeGameObject.transform;
        List<GameObject> goList = new();

        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags( child.gameObject );
            if ( ( flags & StaticEditorFlags.ContributeGI ) == 0 )
            {
                goList.Add(child.gameObject);
            }
        }

        GameObject[] goArray = new GameObject[goList.Count];
        for (int ii = 0; ii < goList.Count; ii++)
        {
            goArray[ii] = goList[ii];
        }

        Selection.objects = goArray;

        // Object[] gameObjects = FindObjectsOfType( typeof ( GameObject ) );
        // GameObject[] gameObjectArray;
        // gameObjectArray = new GameObject[ gameObjects.Length ];
        // int arrayPointer = 0;
        // foreach (GameObject gameObject in gameObjects )
        // {
        //     StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags( gameObject );
        //     if ( ( flags & StaticEditorFlags.ContributeGI ) == 0 )
        //     {
        //         gameObjectArray[ arrayPointer ] = gameObject;
        //         arrayPointer += 1;
        //     }
        // }
        // Selection.objects = gameObjectArray;
    }
}