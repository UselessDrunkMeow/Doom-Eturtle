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
    public List<BoxCollider> colliders = new List<BoxCollider>();
    public List<Vector3> cornerOne = new List<Vector3>();
    public List<Vector3> cornerTwo = new List<Vector3>();  
    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);
    public Vector3 corner1;
    public Vector3 corner2;
    public float distance;
    public float maxdistance;
    public GameObject Player;
    
    void Start()
    {
        colliders.AddRange(Object.FindObjectsByType<BoxCollider>());

        foreach (BoxCollider collider in colliders)
        {
            Vector3 halfSize = collider.size * 0.5f;

            Vector3 corner1 = collider.transform.TransformPoint(
                collider.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)
            );

            Vector3 corner2 = collider.transform.TransformPoint(
                collider.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z)
            );

            cornerOne.Add(corner1);
            cornerTwo.Add(corner2);
        }
    }
    void SpawnWave()
    {
        while(SpawnCredit != 0)
        {
            int Col = UnityEngine.Random.Range(0, colliders.Count);
            GameObject PooledEnemy = ObjectPool.SharedInstance.GetPooledObject(EnemyToSpawn); 
            Vector3 position = new Vector3(
                UnityEngine.Random.Range(cornerOne[Col].x, cornerTwo[Col].x),
                0,
                UnityEngine.Random.Range(cornerOne[Col].z, cornerTwo[Col].z)
            );
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
