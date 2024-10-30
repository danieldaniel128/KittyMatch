using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour , IPointerDownHandler
{
    public abstract void Activate();

    public virtual bool CanMatchWith(Tile otherTile)
    {
        // Default match logic for all tiles (can be overridden)
        return false;
    }

    public void DestroyTile()
    {
        Destroy(gameObject); // Destroy tile GameObject in Unity
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("tile clicked");
    }

}
