using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ModelSpawner : MonoBehaviour
{
    public UnityEvent<GameObject> OnModelSpawn;
    private GameObject modelInScene;
    public void SpawnModel(Vector3 spawnPosition, GameObject prefab){
        modelInScene = Instantiate(prefab,spawnPosition,Quaternion.identity);
        OnModelSpawn.Invoke(modelInScene);
    }
}
