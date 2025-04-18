using System.Collections.Generic;
using UnityEngine;

public class BoardView
{
	private GameObject _tilePrefab;
	private Transform _tileParent;
	private TilePool _tilePool;
	private TileDataSO[] _tileTypes;

	public BoardView(GameObject tilePrefab, Transform tileParent, TilePool tilePool, TileDataSO[] tileTypes)
	{
		_tilePrefab = tilePrefab;
		_tileParent = tileParent;
		_tilePool = tilePool;
		_tileTypes = tileTypes;
	}
    public void InitializeGrid(BoardModel boardModel, int width, int height, UnityEngine.Events.UnityAction<TileController> onSelectCallback)
    {

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int index = new Vector2Int(x, y);
                List<TileDataSO> validTypes = new List<TileDataSO>(_tileTypes);

                // Filter out any tile types that would cause a match
                validTypes.RemoveAll(tileData => WouldCauseMatch(x, y, tileData, boardModel));
                // If all types would cause matches, fallback to original pool
                if (validTypes.Count == 0)
                {
                    Debug.LogWarning($"No valid tile types at {index}, falling back to full set.");
                    validTypes = new List<TileDataSO>(_tileTypes);
                }

                TileDataSO chosen = validTypes[Random.Range(0, validTypes.Count)];
                

                TileController tile = CreateTile(index, chosen);
                boardModel.SetTile(index, tile);
                tile.OnTrySelectingTile.AddListener(onSelectCallback);
            }
        }
    }
    private TileController CreateTile(Vector2Int index, TileDataSO data)
	{
		var obj = GameObject.Instantiate(_tilePrefab, new Vector3(index.x, index.y, 0), Quaternion.identity, _tileParent);
		obj.name = $"Tile({index.x},{index.y})";
		var controller = obj.GetComponent<TileController>();

		controller.Initialize(data);
		controller.AttachPool(_tilePool);
		controller.SetTileIndex(index);

		return controller;
	}
    private bool WouldCauseMatch(int x, int y, TileDataSO tileData, BoardModel boardModel)
    {
        string type = tileData.TileType;

        if (x >= 2)
        {
            var left1 = boardModel.GetTile(new Vector2Int(x - 1, y));
            var left2 = boardModel.GetTile(new Vector2Int(x - 2, y));
            if (left1 != null && left2 != null &&
                left1.GetModelTileType().Equals(type) && left2.GetModelTileType().Equals(type))
                return true;
        }

        if (y >= 2)
        {
            var down1 = boardModel.GetTile(new Vector2Int(x, y - 1));
            var down2 = boardModel.GetTile(new Vector2Int(x, y - 2));
            if (down1 != null && down2 != null &&
                down1.GetModelTileType().Equals(type) && down2.GetModelTileType().Equals(type))
                return true;
        }

        return false;
    }
}
