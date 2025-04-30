using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ModelSpawner : MonoBehaviour
{
    public UnityEvent<GameObject> OnModelSpawn; // Unity event about finishing spawn model.
    private GameObject modelInScene; //variable for return to GameManager
    
    /// <summary>
    /// After Get prefab model and position, spawn model to position.
    /// </summary>
    /// <param name="spawnPosition"></param>
    /// <param name="prefab"></param>
    public void SpawnModel(Vector3 spawnPosition, GameObject prefab){
        modelInScene = Instantiate(prefab,spawnPosition,Quaternion.identity);
        OnModelSpawn.Invoke(modelInScene);
    }
}
