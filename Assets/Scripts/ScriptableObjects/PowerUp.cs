using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "PowerUps")]
public class PowerUp : ScriptableObject
{
    public string specialName;
    public Sprite icon;
    public System.Action<TileController> effect;

    public void ApplyEffect(TileController tile)
    {
        effect?.Invoke(tile);
    }
}
