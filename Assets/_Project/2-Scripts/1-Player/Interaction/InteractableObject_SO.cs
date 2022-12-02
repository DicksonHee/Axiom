using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactable_SO", fileName = "newInteractableSO")]
public class InteractableObject_SO : ScriptableObject
{
    public GameObject objectPrefab;
    [TextArea] public string objectName;
    [TextArea] public string objectDescription;
}
