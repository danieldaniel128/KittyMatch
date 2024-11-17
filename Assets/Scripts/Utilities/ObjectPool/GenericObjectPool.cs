using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    // Initial number of cloned objects
    [SerializeField] private uint initPoolSize;
    public uint InitPoolSize => initPoolSize;

    // Prefab of the object to pool
    [SerializeField] private T objectToPool;
    [SerializeField] List<T> objectsToPool;

    private void Start()
    {
        SetupPool();
    }

    // Creates the pool (invoke when the lag is not noticeable)
    private void SetupPool()
    {
        // Check if the objectToPool field is missing
        if (objectToPool == null)
        {
            Debug.LogError("ObjectToPool prefab is not assigned.");
            return;
        }

        objectsToPool= new List<T>();
        // Populate the pool
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = Instantiate(objectToPool,transform);
            instance.gameObject.SetActive(false);
            objectsToPool.Add(instance);
        }
    }

    // Returns an active object from the pool
    public T GetPooledObject()
    {
        // Check if the objectToPool field is missing
        if (objectToPool == null)
        {
            Debug.LogError("ObjectToPool prefab is not assigned.");
            return null;
        }
        foreach (T instance in objectsToPool)
            if(!instance.gameObject.activeSelf)
            {
                instance.gameObject.SetActive(true);
                objectsToPool.Remove(instance);
                Debug.Log("removed from pool");
                return instance;
            }
        Debug.Log("new pool item");
        T newInstance = Instantiate(objectToPool, transform);
        return newInstance;
    }

    // Returns the object to the pool
    public void ReturnToPool(T pooledObject)
    {
        pooledObject.transform.SetParent(transform);
        pooledObject.gameObject.SetActive(false);
        Debug.Log("return to pool");
        objectsToPool.Add(pooledObject);
    }
}
