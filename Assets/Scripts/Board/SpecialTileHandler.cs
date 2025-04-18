using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialTileHandler
{
	private Dictionary<Vector2Int, TileController> _tiles;
	private SpecialTileDataSO[] _specialTiles;
	private TileDataSO _emptyType;

	public SpecialTileHandler(Dictionary<Vector2Int, TileController> tiles, SpecialTileDataSO[] specialTiles, TileDataSO emptyType)
	{
		_tiles = tiles;
		_specialTiles = specialTiles;
		_emptyType = emptyType;
	}

	public void ApplySpecialEffect(TileController specialTile)
	{
		string type = specialTile.GetModelTileType();

		if (type == "TShape")
		{
			var center = specialTile.TileIndex;
			for (int x = center.x - 1; x <= center.x + 1; x++)
				for (int y = center.y - 1; y <= center.y + 1; y++)
					PopIfInBounds(new Vector2Int(x, y));
		}
		else if (type == "4Row")
		{
			foreach (var kvp in _tiles.Where(t => t.Key.y == specialTile.Y))
				PopIfInBounds(kvp.Key);
		}
		else if (type == "4Column")
		{
			foreach (var kvp in _tiles.Where(t => t.Key.x == specialTile.X))
				PopIfInBounds(kvp.Key);
		}
	}

	private void PopIfInBounds(Vector2Int index)
	{
		if (_tiles.TryGetValue(index, out var tile))
		{
			tile.ChangeIcon(null);
			tile.Initialize(_emptyType);
		}
	}
}