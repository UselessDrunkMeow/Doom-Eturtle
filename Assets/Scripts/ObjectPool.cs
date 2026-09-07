using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;

    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool = 10;
    public bool shouldExpand = true; // Optional: auto-expand pool if empty

    void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();

        // Populate pool initially
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        // Iterate over the actual list count (not hardcoded amountToPool)
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // 1. Clean up missing/destroyed references safely
            if (pooledObjects[i] == null)
            {
                pooledObjects.RemoveAt(i);
                i--; // Adjust index after removal
                continue;
            }

            // 2. Return an available inactive object
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // 3. Dynamically expand pool if enabled and all objects are active
        if (shouldExpand)
        {
            GameObject tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            return tmp;
        }

        return null;
    }
}