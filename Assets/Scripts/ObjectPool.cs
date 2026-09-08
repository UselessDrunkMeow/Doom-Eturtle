using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;

    [System.Serializable]
    public class PoolItem
    {
        public string poolTag;          // Identifier string (e.g., "Bullet", "Enemy")
        public GameObject objectToPool; // Prefab to instantiate
        public int amountToPool = 10;   // Initial size
        public bool shouldExpand = true;// Expand if empty
        
        [HideInInspector] 
        public List<GameObject> pooledObjects = new List<GameObject>();
    }
    [Header("Pool Definitions")]
    public List<PoolItem> itemsToPool;

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
        foreach (PoolItem item in itemsToPool)
        {
            item.pooledObjects = new List<GameObject>();

            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject tmp = Instantiate(item.objectToPool);
                tmp.SetActive(false);
                item.pooledObjects.Add(tmp);
            }
        }
    }
    /// <summary>
    /// Fetches an available pooled object by its string tag.
    /// </summary>
    public GameObject GetPooledObject(string tag)
    {
        // Find the pool item matching the requested tag
        PoolItem item = itemsToPool.Find(p => p.poolTag == tag);

        if (item == null)
        {
            Debug.LogWarning($"ObjectPool: Pool with tag '{tag}' was not found!");
            return null;
        }

        // 1. Iterate through existing items in this specific pool
        for (int i = 0; i < item.pooledObjects.Count; i++)
        {
            // Clean up missing/destroyed references
            if (item.pooledObjects[i] == null)
            {
                item.pooledObjects.RemoveAt(i);
                i--;
                continue;
            }

            // Return an available inactive object
            if (!item.pooledObjects[i].activeInHierarchy)
            {
                return item.pooledObjects[i];
            }
        }

        // 2. Expand pool dynamically if enabled for this item
        if (item.shouldExpand)
        {
            GameObject tmp = Instantiate(item.objectToPool);
            tmp.SetActive(false);
            item.pooledObjects.Add(tmp);
            return tmp;
        }

        return null;
    }
}