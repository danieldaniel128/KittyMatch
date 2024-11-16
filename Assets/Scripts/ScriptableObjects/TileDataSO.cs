using UnityEngine;

[CreateAssetMenu(fileName = "NewTileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileDataSO : ScriptableObject
{
    public string TileName;
    public string TileType;
    public Texture2D TileIcon;
    public bool IsRenderered;

    // Additional properties for special effects or scoring could be added here
}

