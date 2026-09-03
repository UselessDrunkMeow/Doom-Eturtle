using Mono.Cecil.Cil;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public float SpawnRadius = 10;
    public GameObject prefab;
    public quaternion SpawnRotation = new quaternion(0, 0, 0, 0);

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), UnityEngine.Random.Range(-SpawnRadius, SpawnRadius), UnityEngine.Random.Range(-SpawnRadius, SpawnRadius));
        Instantiate(prefab, position, SpawnRotation);
    }
}
