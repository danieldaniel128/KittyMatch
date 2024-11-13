using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject basicTilePrefab;
    [SerializeField] private Transform _tilesHolder;
    [SerializeField] private MatchHandler _matchHandler;
    [SerializeField] private TileDataSO[] _tileDataSOs;
    [SerializeField] private TileDataSO _emptyTileDataSO;
    private ITile[,] _tiles;

    private ITile _firstSelectedTile = null;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _tiles = new TileController[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, 0), Quaternion.identity, _tilesHolder);
                TileController tileComponent = newTileObject.GetComponent<TileController>();

                // Randomly select a TileDataSO for this tile
                TileDataSO tileData = _tileDataSOs[Random.Range(0, _tileDataSOs.Length)];

                // Initialize the tile with the properties from the selected TileDataSO
                tileComponent.Initialize(tileData);

                tileComponent.OnSelectedTile.AddListener(OnTileSelected); // Register listener
                _tiles[x, y] = tileComponent;
            }
        }
    }

    public ITile GetTileAt(int x, int y) => _tiles[x, y];
    public void SetTileAt(int x, int y, ITile tile) => _tiles[x, y] = tile;
    private Vector2Int GetTilePosition(ITile tile)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_tiles[x, y] == tile)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }
    private void OnTileSelected(ITile selectedTile)
    {
        if (_firstSelectedTile == null)
        {
            _firstSelectedTile = selectedTile;
            //_firstSelectedTile.ActivateSelectedVFX();
        }
        else
        {
            //selectedTile.ActivateSelectedVFX();
            // Calculate positions of both tiles
            Vector2Int pos1 = GetTilePosition(_firstSelectedTile);
            Vector2Int pos2 = GetTilePosition(selectedTile);

            // Call OnTileSwap with the selected positions
            OnTileSwap(pos1, pos2);

            // Reset selection
            _firstSelectedTile = null;
        }
    }
    //change to dotween.
    public void SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        TileController tempTile = _tiles[pos1.x, pos1.y] as TileController;
        _tiles[pos1.x, pos1.y] = _tiles[pos2.x, pos2.y];
        _tiles[pos2.x, pos2.y] = tempTile;

        TileController tile1 = _tiles[pos1.x, pos1.y] as TileController;
        TileController tile2 = _tiles[pos2.x, pos2.y] as TileController;
        // Animate the tiles' movement
        Vector3 pos1WorldPosition = tile1.transform.position;
        Vector3 pos2WorldPosition = tile2.transform.position;

        // Swap positions with animation
        tile1.transform.DOMove(pos2WorldPosition, 0.3f); // Adjust duration as needed
        tile2.transform.DOMove(pos1WorldPosition, 0.3f);
    }
    public void OnTileSwap(Vector2Int pos1, Vector2Int pos2)
    {
        SwapTiles(pos1, pos2);

        var matches = _matchHandler.DetectMatches(_tiles);
        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                //match effect
                foreach (ITile tile in match.MatchTiles)
                    (tile as TileController).Initialize(_emptyTileDataSO);
            }
            FillEmptySpaces();
        }
        else
        {
            // No match, swap back
            SwapTiles(pos1, pos2);
        }
    }
    public void FillEmptySpaces()
    {
        // Logic to shift tiles down and spawn new ones at the top
    }
}