using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EnemySpawnerScript : MonoBehaviour
{
    public int enemyOneCost = 1;
    public int SpawnCredit = 5;
    public int wavecount = 0;
    public List<GameObject> Enemies;
    public float SpawnRadius = 10;
    public GameObject prefab;
    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);
    
    void SpawnWave()
    {
        while(SpawnCredit != 0)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), 0, UnityEngine.Random.Range(-SpawnRadius, SpawnRadius));
            Enemies.Add(Instantiate(prefab, position, SpawnRotation));
            SpawnCredit = SpawnCredit-enemyOneCost;
        }
        wavecount++;
        SpawnCredit = wavecount + 5;
    }
    void CleanEnemyList()
    {
        Enemies.RemoveAll(enemy => enemy == null);
    }

    void FixedUpdate()
    {
        
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
        
    }
    void Update()
    {
        CleanEnemyList();
    }
}