using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    // Initial number of cloned objects
    [SerializeField] private uint initPoolSize;
    public uint InitPoolSize => initPoolSize;

    // Prefab of the object to pool
    [SerializeField] private T objectToPool;

    // Stack to store the pooled objects
    private Stack<T> stack;

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

        stack = new Stack<T>();

        // Populate the pool
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = Instantiate(objectToPool);
            instance.gameObject.SetActive(false);
            stack.Push(instance);
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

        // If the pool is empty, instantiate a new object
        if (stack.Count == 0)
        {
            T newInstance = Instantiate(objectToPool);
            return newInstance;
        }

        // Otherwise, retrieve an object from the pool
        T nextInstance = stack.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    // Returns the object to the pool
    public void ReturnToPool(T pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}
