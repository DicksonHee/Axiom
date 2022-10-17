using System.Collections;
using System.Collections.Generic;
using Axiom.NonEuclidean;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLinker
{
    [Shortcut("Link Portals", KeyCode.P, ShortcutModifiers.Control)]

    public static void LinkPortals()
    {
        GameObject[] gameObjects = Selection.gameObjects;

        gameObjects[0].TryGetComponent(out Portal portal0);
        gameObjects[0].TryGetComponent(out BoxCollider collider0);
        Transform screen0 = gameObjects[0].transform.Find("Screen");

        gameObjects[1].TryGetComponent(out Portal portal1);
        gameObjects[1].TryGetComponent(out BoxCollider collider1);
        Transform screen1 = gameObjects[1].transform.Find("Screen");

        if (portal0 == null || portal1 == null) return;

        portal0.otherPortal = portal1;
        portal1.otherPortal = portal0;

        collider1.size = collider0.size;
        collider1.center = collider1.center;
        screen1.localPosition = screen0.localPosition;
        screen1.localScale = screen0.localScale;
        screen1.localRotation = Quaternion.identity;
        screen0.localRotation = Quaternion.identity;

        PrefabUtility.RecordPrefabInstancePropertyModifications(portal0);
        PrefabUtility.RecordPrefabInstancePropertyModifications(portal1);
        PrefabUtility.RecordPrefabInstancePropertyModifications(collider0);
        PrefabUtility.RecordPrefabInstancePropertyModifications(collider1);
        PrefabUtility.RecordPrefabInstancePropertyModifications(screen0);
        PrefabUtility.RecordPrefabInstancePropertyModifications(screen1);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }
}
