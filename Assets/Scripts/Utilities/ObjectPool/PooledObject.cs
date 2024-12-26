using System.Collections;
using UnityEngine;

//example how to use.
public class PooledObject : MonoBehaviour
{
    public TilePool Pool { get; protected set; }
    public virtual void ResetPooledObject()
    {

    }
    public void AttachPool(TilePool pool)
    {
        Pool = pool;
    }
}
