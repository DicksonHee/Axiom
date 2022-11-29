using Axiom.Player.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRenderer : MonoBehaviour
{
    public Transform spawnPosition;

    private GameObject spawnedGameObject;

    private void OnEnable()
    {
        PlayerInteract.OnStartInteract += SpawnObject;
    }

    private void SpawnObject(InteractableObject_SO objectSO)
    {
        if(spawnedGameObject != null) Destroy(spawnedGameObject);

        spawnedGameObject = Instantiate(objectSO.objectPrefab, spawnPosition);
    }
}
