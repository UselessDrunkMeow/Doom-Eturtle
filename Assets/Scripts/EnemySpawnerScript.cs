using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EnemySpawnerScript : MonoBehaviour
{
    public string EnemyToSpawn = "Æpfft-steyghnn";
    public int enemyOneCost = 1;
    public int enemyTwoCost = 20;
    public BoxCollider boxCollider;
    public int SpawnCredit = 5;
    public int wavecount = 0;
    public List<GameObject> Enemies;

    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);
    public Vector3 corner1;
    public Vector3 corner2;
    public float distance;
    public float maxdistance;
    public GameObject Player;
    
    void Start()
    {
        Vector3 halfSize = boxCollider.size * 0.5f;
        corner1 = boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
        corner2 = boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
    }
    void SpawnWave()
    {
        while(SpawnCredit != 0)
        {
            GameObject PooledEnemy = ObjectPool.SharedInstance.GetPooledObject(EnemyToSpawn); 
            Vector3 position = new Vector3(UnityEngine.Random.Range(corner1.x, corner2.x), 0, UnityEngine.Random.Range(corner1.z, corner2.z));
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
        SpawnCredit = wavecount + 5;
    }
    void CleanEnemyList()
    {
        Enemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);
    }
    void Update()
    {
        distance = Vector3.Distance(Player.transform.position, boxCollider.transform.position);
        CleanEnemyList();
        if (Enemies.Count == 0 && distance >= maxdistance)
        {
            Debug.Log("spawn wave");
            SpawnWave();
        }
    }
}
