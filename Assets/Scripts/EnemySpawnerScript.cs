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

    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);
    
    void SpawnWave()
    {
        
        while(SpawnCredit != 0)
        {
            GameObject PooledEnemy = ObjectPool.SharedInstance.GetPooledObject(); 
            Vector3 position = new Vector3(UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), 0, UnityEngine.Random.Range(-SpawnRadius, SpawnRadius));
            if (PooledEnemy != null) {
                PooledEnemy.transform.position = position;
                PooledEnemy.SetActive(true);
                Enemies.Add(PooledEnemy);
                SpawnCredit -= enemyOneCost;
            }
            else
            {
                // Pool ran out of available objects
                Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
                break; 
            }
        }
        wavecount++;
        SpawnCredit = wavecount + SpawnCredit;
    }
    void CleanEnemyList()
    {
        Enemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);
    }
    void Update()
    {
        CleanEnemyList();
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }
}
