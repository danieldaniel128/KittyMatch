using System.Collections.Generic;
using UnityEngine;

public class BoardModel
{
	private Dictionary<Vector2Int, TileController> _tiles;

	public BoardModel()
	{
		_tiles = new Dictionary<Vector2Int, TileController>();
	}

	public void SetTile(Vector2Int index, TileController tile)
	{
		_tiles[index] = tile;
	}

	public TileController GetTile(Vector2Int index)
	{
		_tiles.TryGetValue(index, out var tile);
		return tile;
	}

	public Dictionary<Vector2Int, TileController> GetAllTiles() => _tiles;

	public void Clear() => _tiles.Clear();

	public bool IsEmpty(Vector2Int index, string emptyType)
	{
		return _tiles.ContainsKey(index) && _tiles[index].GetModelTileType().Equals(emptyType);
	}
}