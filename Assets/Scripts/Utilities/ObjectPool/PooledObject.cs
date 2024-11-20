using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example how to use.
public class PooledObject : MonoBehaviour
{
    private GenericObjectPool<PooledObject> pool;
    public GenericObjectPool<PooledObject> Pool { get => pool; set => pool = value; }

    public void ReleaseToPool()
    {
        pool.ReturnToPool(this);
    }
}
