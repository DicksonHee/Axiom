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
        gameObjects[1].TryGetComponent(out Portal portal1);

        if (portal0 == null || portal1 == null) return;

        portal0.otherPortal = portal1;
        portal1.otherPortal = portal0;

        PrefabUtility.RecordPrefabInstancePropertyModifications(portal0);
        PrefabUtility.RecordPrefabInstancePropertyModifications(portal1);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }
}
