using Mono.Cecil.Cil;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnerScript : MonoBehaviour
{
    public List<GameObject> Enemies;
    public float SpawnRadius = 10;
    public GameObject prefab;
    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);
    void SpawnWave()
    {
        int i = 5;
        
        while(i != 0)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), UnityEngine.Random.Range(-SpawnRadius, SpawnRadius));
            Enemies.Add(Instantiate(prefab, position, SpawnRotation));
            i = i-1;
        }
        
    }

    void FixedUpdate()
    {
        
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
        
    }
}