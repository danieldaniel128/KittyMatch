using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicTile : Tile
{
    public Color TileColor { get; private set; }

    public void Initialize(Color color)
    {
        TileColor = color;
    }

    public override void Activate()
    {
        // No special behavior for basic tiles
    }

    public override bool CanMatchWith(Tile otherTile)
    {
        if (otherTile is BasicTile otherBasicTile)
        {
            return otherBasicTile.TileColor == TileColor;
        }
        return false;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
}
