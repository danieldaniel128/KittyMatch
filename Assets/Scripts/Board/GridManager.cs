using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject basicTilePrefab;
    [SerializeField] private Transform _tilesHolder;
    [SerializeField] private MatchHandler _matchHandler;
    [SerializeField] private TileDataSO[] _tileDataSOs;
    [SerializeField] private TileDataSO _emptyTileDataSO;
    private List<TileController> _tiles;
    private TileController[,] _tileGrid;

    private TileController _firstSelectedTile = null;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _tiles = new List<TileController>();
        _tileGrid = new TileController[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, 0), Quaternion.identity, _tilesHolder);
                newTileObject.name = $"Tile({x},{y})";
                TileController tileComponent = newTileObject.GetComponent<TileController>();

                // Randomly select a TileDataSO for this tile
                TileDataSO tileData = _tileDataSOs[Random.Range(0, _tileDataSOs.Length)];

                // Initialize the tile with the properties from the selected TileDataSO
                tileComponent.Initialize(tileData);
                //set the event of OnSelectedTile.
                tileComponent.OnSelectedTile.AddListener(OnTileSelected); // Register listener
                //set index for each created tile.
                tileComponent.SetTileIndex(x, y);
                //add to list of tiles.
                _tiles.Add(tileComponent);
                //add to grid of tiles.
                _tileGrid[x, y] = tileComponent;
            }
        }
    }

    public TileController GetTileAt(int x, int y) { Vector2Int tileIndex = new Vector2Int(x, y); return _tiles.Find(tile => tile.TileIndex == tileIndex); }
    public TileController GetTileAt(Vector2Int tileIndexSearch) { Vector2Int tileIndex = new Vector2Int(tileIndexSearch.x, tileIndexSearch.y); return _tiles.Find(tile => tile.TileIndex == tileIndex); }
    private void OnTileSelected(TileController selectedTile)
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
            Vector2Int pos1 = _firstSelectedTile.TileIndex;
            Vector2Int pos2 = selectedTile.TileIndex;

            if(CanSwapTiles(pos1,pos2))
                // Call OnTileSwap with the selected positions
                OnTileSwap(pos1, pos2);

            // Reset selection
            _firstSelectedTile = null;
        }
    }
    bool CanSwapTiles(Vector2Int posTile1, Vector2Int posTile2)
    {
        //Debug.Log("posTile1: " + posTile1);
        //Debug.Log("posTile2: " + posTile2);
        return Vector2Int.Distance(posTile1,posTile2) == 1;
    }
    public void SwapTiles(Vector2Int tile1Index, Vector2Int tile2Index)
    {
        // Get tiles at the specified positions
        TileController tile1 = GetTileAt(tile1Index.x, tile1Index.y) as TileController;
        TileController tile2 = GetTileAt(tile2Index.x, tile2Index.y) as TileController;

        // Animate the tiles' movement using DoTween
        Vector3 pos1WorldPosition = tile1.transform.position;
        Vector3 pos2WorldPosition = tile2.transform.position;
        //move world pos of tiles dotween.
        tile1.transform.DOMove(pos2WorldPosition, 0.3f); // Adjust duration as needed
        tile2.transform.DOMove(pos1WorldPosition, 0.3f);

        //tile1 has tile2 index
        tile1.SetTileIndex(tile2Index);
        //tile2 has tile1 index
        tile2.SetTileIndex(tile1Index);
        //Debug.Log("tile1: " + tile1.TileIndex);
        //Debug.Log("tile2: "+ tile2.TileIndex);
    }



    public void OnTileSwap(Vector2Int pos1, Vector2Int pos2)
    {
        SwapTiles(pos1, pos2);

        var matches = _matchHandler.DetectMatches(_tiles,5);
        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                //match effect
                foreach (ITile tile in match.Tiles)
                { 
                    (tile as TileController).Initialize(_emptyTileDataSO);
                    (tile as TileController).GetComponent<CanvasGroup>().alpha = 0;
                }
            }
            FillEmptySpaces();
        }
        else
        {
            // No match, swap back
            //remove from comments only when it has logic, because now it swaps back always and it prevents from 
            //SwapTiles(GetTileAt(pos1).TileIndex,GetTileAt(pos2).TileIndex);
        }
    }
    public void FillEmptySpaces()
    {
        // Logic to shift tiles down and spawn new ones at the top
    }
}