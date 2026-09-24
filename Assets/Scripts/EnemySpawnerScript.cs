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
    public int InitialSpawnCredit = 5;
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
    public float BossWave;
    public List<BoxCollider> Spawnable;
    public HealthManager Jheffreighy;
    public HealthManager Æpfftsteyghnn;
    public HealthManager PlayerHealth;
    public GameObject Boss;
    public float DifficultyFactor = 1.1f;

    void Start()
    {
        colliders.AddRange(GetComponents<BoxCollider>());

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
        InitialSpawnCredit = SpawnCredit;
    }
    void WaveHarder()
    {
        Jheffreighy._MaxHealth = Jheffreighy._MaxHealth * DifficultyFactor;
        Æpfftsteyghnn._MaxHealth = Æpfftsteyghnn._MaxHealth * DifficultyFactor;
        if (wavecount >= BossWave)
        {
            Boss.GetComponent<HealthManager>()._MaxHealth = Boss.GetComponent<HealthManager>()._MaxHealth * DifficultyFactor;
        }
    }

    void SpawnWave()
    {
        //WaveHarder();
        Debug.LogWarning("wavecount = " + wavecount + "Bosswave =" + BossWave);
        if (wavecount == BossWave)
        {
            Debug.LogWarning("WHAT THE FUCK WHY DO U AAAAAAAAA");
            Boss.gameObject.SetActive(true);
            Enemies.Add(Boss.gameObject);
            Boss.GetComponent<HealthManager>()._CurrentHealth = Boss.GetComponent<HealthManager>()._MaxHealth;
            Player.GetComponent<HealthManager>()._MaxHealth = Player.GetComponent<HealthManager>()._MaxHealth + 1;
            BossWave = BossWave + 10;
        }
        else
        {
            Debug.LogWarning("GVdasfvsddsgjydsguykdsghfegtr5jhtrtr987tr987jtr89j98jyr98j7474jjtr464jr654j546789657%$^&^&^&^%&*^&^%&*");
            while (SpawnCredit != 0)
            {
                if (Spawnable.Count == 0)
                {
                    Debug.Log("No spawnable colliders! Player too close!");
                    break;
                }

                int spawnableIndex = UnityEngine.Random.Range(0, Spawnable.Count);

                WhatEnemyToSpawn();

                BoxCollider selectedCollider = Spawnable[spawnableIndex];

                int colliderIndex = colliders.IndexOf(selectedCollider);

                GameObject PooledEnemy =
                    ObjectPool.SharedInstance.GetPooledObject(EnemyToSpawn);

                if (PooledEnemy != null)
                {
                    Vector3 position = new Vector3(
                        UnityEngine.Random.Range(
                            cornerOne[colliderIndex].x,
                            cornerTwo[colliderIndex].x
                        ),
                        0,
                        UnityEngine.Random.Range(
                            cornerOne[colliderIndex].z,
                            cornerTwo[colliderIndex].z
                        )
                    );

                    PooledEnemy.transform.position = position;
                    PooledEnemy.SetActive(true);
                    Enemies.Add(PooledEnemy);
                }

                else
                {
                    Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
                    break;
                }
            }
        }


        SpawnCredit = InitialSpawnCredit + wavecount + 3;
        wavecount++;
        PlayerHealth._CurrentHealth = PlayerHealth._MaxHealth;
    }

    void WhatEnemyToSpawn()
    {
        if (SpawnCredit >= enemyTwoCost)
        {
            int EnemyType = UnityEngine.Random.Range(0, 2);
            print(EnemyType);
            switch (EnemyType)
            {
                case 0:
                    EnemyToSpawn = "Jheffreighy";
                    print("Jheffreighy IN THE CASE SPAWNERD");
                    SpawnCredit = SpawnCredit - enemyOneCost;
                    break;
                case 1:
                    EnemyToSpawn = "Æpfft-steyghnn";
                    print("Æpfft-steyghnn SPAWNERD");
                    SpawnCredit = SpawnCredit - enemyTwoCost;
                    break;
            }
        }

        else if (SpawnCredit < enemyTwoCost)
        {
            EnemyToSpawn = "Jheffreighy";
            print("Jheffreighy SPAWNERD");
            SpawnCredit = SpawnCredit - enemyOneCost;
        }
    }

    void CleanEnemyList()
    {
        Enemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);
    }

    void Update()
    {
        foreach (BoxCollider collider in colliders)
        {
            Vector3 colliderCenter =
                collider.transform.TransformPoint(collider.center);

            float distanceToCollider = Vector3.Distance(
                Player.transform.position,
                colliderCenter
            );

            if (distanceToCollider < maxdistance)
            {
                Spawnable.Remove(collider);
            }
            else
            {
                if (!Spawnable.Contains(collider))
                {
                    Spawnable.Add(collider);
                }
            }
        }

        CleanEnemyList();

        if (Enemies.Count <= 0 && Spawnable.Count > 0)
        {
            Debug.Log("spawn wave");
            SpawnWave();
        }
    }
}
