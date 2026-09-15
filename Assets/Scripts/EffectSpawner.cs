using System;
using UnityEngine;
using UnityEngine.Pool;

public class EffectSpawner : MonoBehaviour
{
    private GameObject PooledEffect;
    public static void SpawnEffect(Vector3 transform, String EffectName)
    {
            GameObject PooledEffect =
                ObjectPool.SharedInstance.GetPooledObject(EffectName);
        
            if (PooledEffect != null)
                {
                    PooledEffect.transform.position = transform;
                    PooledEffect.SetActive(true);
                    PooledEffect.GetComponent<ParticleSystem>().Play();

                    if (PooledEffect.TryGetComponent<ParticleSystem>(out var ps))
            {
                ps.Play();
            }
                }
                
            else
                {
                    Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
                }

    }
}
