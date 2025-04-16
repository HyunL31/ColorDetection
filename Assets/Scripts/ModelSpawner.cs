using System;
using System.Collections;
using UnityEngine;

public class ModelSpawner : MonoBehaviour
{
    public event EventHandler OnModelSpawn;
    private GameObject modelInScene;
    public void SpawnModel(Vector3 spawnPosition, GameObject prefab){
        modelInScene = Instantiate(prefab,spawnPosition,Quaternion.identity);
        OnModelSpawn?.Invoke(this,EventArgs.Empty);
    }
}
