using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Pool;
using System.Text.RegularExpressions;
using static UnityEditor.Handles;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject basicTilePrefab;
    [SerializeField] private SwipeInputHandler _swipeInputHandler;
    [SerializeField] private Transform _overlappingParent;
    [SerializeField] private Transform _tilesHolder;
    [SerializeField] private TilePool _tilesPool;
    [SerializeField] private MatchHandler _matchHandler;
    [SerializeField] private TileDataSO[] _tileDataSOs;
    [SerializeField] private TileDataSO _emptyTileDataSO;
    private List<TileController> _tiles;
    private TileController[,] _tileGrid;

    private TileController _firstSelectedTile = null;


    [SerializeField] bool _isSwapping;
    [SerializeField] bool _isMatching;
    private void Start()
    {
        InitializeGrid();
        _swipeInputHandler.OnSwipe.AddListener(HandleTileSwipe);
        _swipeInputHandler.OnTap.AddListener(DeselectDrag);
    }

    private void InitializeGrid()
    {
        _tiles = new List<TileController>();
        _tileGrid = new TileController[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, _tilesHolder.position.z), Quaternion.identity, _tilesHolder);
                newTileObject.name = $"Tile({x},{y})";
                TileController tileComponent = newTileObject.GetComponent<TileController>();
                TileDataSO tileData = _tileDataSOs[Random.Range(0, _tileDataSOs.Length)];
                if(_tileDataSOs!=null && _tileDataSOs.Length>1)//prevents from the loop to go forever since 1 tiledata will always cause a match.
                    do
                    {
                        // Randomly select a TileDataSO for this tile
                        tileData = _tileDataSOs[Random.Range(0, _tileDataSOs.Length)];
                    }
                    while (WouldCauseMatch(x, y, tileData));

                // Initialize the tile with the properties from the selected TileDataSO
                tileComponent.Initialize(tileData);
                tileComponent.AttachPool(_tilesPool);
                //set the event of OnSelectedTile.
                tileComponent.OnTrySelectingTile.AddListener(OnTileDragged); // Register listener
                //set index for each created tile.
                tileComponent.SetTileIndex(x, y);
                //add to list of tiles.
                _tiles.Add(tileComponent);
                //add to grid of tiles.
                _tileGrid[x, y] = tileComponent;
            }
        }
    }
    /// <summary>
    /// Checks if placing a tile at a given position would create a match.
    /// </summary>
    private bool WouldCauseMatch(int x, int y, TileDataSO tileData)
    {
        // Check for horizontal match
        if (x >= 2 &&
            _tileGrid[x - 1, y].GetModelTileType().Equals(tileData.TileType) &&
            _tileGrid[x - 2, y].GetModelTileType().Equals(tileData.TileType))
        {
            return true;
        }

        // Check for vertical match
        if (y >= 2 &&
            _tileGrid[x, y - 1].GetModelTileType().Equals(tileData.TileType) &&
            _tileGrid[x, y - 2].GetModelTileType().Equals(tileData.TileType))
        {
            return true;
        }

        return false;
    }
    public TileController GetTileAt(int x, int y) { Vector2Int tileIndex = new Vector2Int(x, y); return _tiles.Find(tile => tile.TileIndex == tileIndex); }
    public TileController GetTileAt(Vector2Int tileIndexSearch) { Vector2Int tileIndex = new Vector2Int(tileIndexSearch.x, tileIndexSearch.y); return _tiles.Find(tile => tile.TileIndex == tileIndex); }
    private void HandleTileSwipe(Vector2Int swipeDirection)
    {
        //Calculate positions of both tiles
        Vector2Int draggedTilePos = _firstSelectedTile.TileIndex;
        Vector2Int swapTargetPos = _firstSelectedTile.TileIndex + swipeDirection;
        TileController swapTargetTile = GetTileAt(swapTargetPos);
        if (swapTargetTile == null || !CanSwapTiles(draggedTilePos, swapTargetPos))
        {
            DeselectDrag();
            return;
        }
        swapTargetTile.OnSelectedTile?.Invoke(true);
        //visual to selected effect
        swapTargetTile.GetIcon().transform.DOScale(Vector3.one * 1.1f, 0.1f);
        // Call OnTileSwap with the selected positions
        OnTileSwap(draggedTilePos, swapTargetPos);
        // Reset selection
        _firstSelectedTile = null;
    }
    private void DeselectDrag()
    {
        _firstSelectedTile.OnDeSelectedTile?.Invoke(false);
        _firstSelectedTile.GetIcon().transform.transform.DOScale(Vector3.one, 0.1f);
        _firstSelectedTile = null;
    }
    private void OnTileDragged(TileController selectedTile)
    {
        //cant select while
        if (_isSwapping || _isMatching || selectedTile.GetModelTileType().Equals(_emptyTileDataSO.TileType))
            return;
        if (_firstSelectedTile == null)
        {
            _firstSelectedTile = selectedTile;
            //visual to selected effect
            selectedTile.OnSelectedTile?.Invoke(true);
            _firstSelectedTile.GetIcon().transform.DOScale(Vector3.one * 1.1f, 0.1f);
        }
    }
    bool CanSwapTiles(Vector2Int posTile1, Vector2Int posTile2)
    {
        //Debug.Log("posTile1: " + posTile1);
        //Debug.Log("posTile2: " + posTile2);
        return Vector2Int.Distance(posTile1,posTile2) == 1;
    }
    public async Task SwapTiles(Vector2Int tile1Index, Vector2Int tile2Index)
    {
        // Get tiles at the specified positions
        TileController tile1 = GetTileAt(tile1Index.x, tile1Index.y) as TileController;
        TileController tile2 = GetTileAt(tile2Index.x, tile2Index.y) as TileController;

        // Animate the tiles' movement using DoTween
        Vector3 pos1WorldPosition = tile1.transform.position;
        Vector3 pos2WorldPosition = tile2.transform.position;
        Sequence sequence = DOTween.Sequence();
        //set parent for overlapping for ui.
        tile1.GetIcon().transform.SetParent(_overlappingParent);
        tile2.GetIcon().transform.transform.SetParent(_overlappingParent);

        //move world pos of tiles dotween.
        sequence.Join(tile1.GetIcon().transform.transform.DOMove(pos2WorldPosition, 0.3f)); // Adjust duration as needed
        sequence.Join(tile2.GetIcon().transform.transform.DOMove(pos1WorldPosition, 0.3f));
        await sequence.Play().AsyncWaitForCompletion();
        //change icons
        IconHandler tmpTileIcon = tile1.GetIcon();
        tile1.ChangeIcon(tile2.GetIcon());
        tile2.ChangeIcon(tmpTileIcon);
        //change references of the tiles icon and data.
        TileDataSO tmpTileDataSO = _tileDataSOs.FirstOrDefault(c=> c.TileType.Equals(tile1.GetModelTileType()));
        tile1.Initialize(_tileDataSOs.FirstOrDefault(c => c.TileType.Equals(tile2.GetModelTileType())));
        tile2.Initialize(tmpTileDataSO);
        //visual back to deselect.
        tile1.GetIcon().transform.transform.DOScale(Vector3.one, 0.1f);
        tile2.GetIcon().transform.transform.DOScale(Vector3.one, 0.1f);
        tile1.ConnectIconToParent();
        tile2.ConnectIconToParent();
        //DeSelect tiles after swapping.
        tile1.OnDeSelectedTile?.Invoke(false);
        tile2.OnDeSelectedTile?.Invoke(false);
    }



    public async void OnTileSwap(Vector2Int pos1, Vector2Int pos2)
    {
        _isSwapping = true;
        await SwapTiles(pos1, pos2);
        _isSwapping = false;

        var matches = _matchHandler.DetectMatches(_tiles, Height);

        // Swap back if no matches found
        if (matches.Count == 0)
        {
            _isSwapping = true;
            await SwapTiles(pos1, pos2);
            _isSwapping = false;
            return; // Exit early as no match was found
        }

        // Process matches until none are left
        do
        {
            _isMatching = true;

            // Pop matched tiles
            var popTasks = new List<Task>();
            var poppedTiles = new HashSet<TileController>();
            foreach (Match match in matches)
            {
                foreach (TileController tile in match.Tiles)
                {
                    if (poppedTiles.Add(tile)) // Ensure each tile is processed only once
                    {
                        tile.ActivatePopIcon();//
                        popTasks.Add(tile.AwaitPopIcon());
                    }
                }
            }
            //await Task.WhenAll(popTasks); // Wait for all pops to complete (optional)
            Debug.Log("Pop finished");

            // Clear icons for matched tiles (use the same poppedTiles set)
            foreach (var tile in poppedTiles)
            {
                tile.ChangeIcon(null);
                tile.Initialize(_emptyTileDataSO);
            }

            // Fill empty spaces
            await FillEmptySpaces();

            // Detect new matches
            matches = _matchHandler.DetectMatches(_tiles, Height);
        }
        while (matches.Count > 0);


        _isMatching = false; // Reset state
    }



    public async Task FillEmptySpaces()
    {
        List<TileController> fellTiles = new List<TileController>();
        Sequence sequence = DOTween.Sequence();

        for (int x = 0; x < Width; x++) // Process each column individually
        {
            ProcessColumn(x, sequence, fellTiles);
        }

        // Play the sequence and wait for it to complete
        await sequence.Play().AsyncWaitForCompletion();

        // Reconnect icons to their parents
        ReconnectFellTiles(fellTiles);
    }

    private void ProcessColumn(int x, Sequence sequence, List<TileController> fellTiles)
    {
        for (int y = Height - 1; y >= 0; y--) // Start from the bottom row and move up
        {
            TileController emptyTile = GetTileAt(x, y);

            if (IsEmptyTile(emptyTile))
            {
                if (!TryFillWithFallingTile(x, y, sequence, fellTiles))
                {
                    CreateNewTileAt(x, y, sequence, fellTiles);
                }
            }
        }
    }

    private bool IsEmptyTile(TileController tile)
    {
        return tile.GetModelTileType().Equals(_emptyTileDataSO.TileType);
    }

    private bool TryFillWithFallingTile(int x, int y, Sequence sequence, List<TileController> fellTiles)
    {
        for (int aboveY = y - 1; aboveY >= 0; aboveY--) // Look for the first non-empty tile above
        {
            TileController fallingTile = GetTileAt(x, aboveY);

            if (!IsEmptyTile(fallingTile))
            {
                MoveTileDown(fallingTile, GetTileAt(x, y), sequence, fellTiles);
                return true;
            }
        }
        return false;
    }

    private void MoveTileDown(TileController fallingTile, TileController emptyTile, Sequence sequence, List<TileController> fellTiles)
    {
        float distance = emptyTile.Y - fallingTile.Y;
        float fallDuration = Mathf.Lerp(0.3f, 0.6f, distance / Height); // Adjust range as needed

        // Move the tile down
        Vector3 emptyTileWorldPos = emptyTile.transform.position;

        fallingTile.GetIcon().transform.SetParent(_overlappingParent);
        sequence.Join(fallingTile.GetIcon().transform.DOMove(emptyTileWorldPos, fallDuration).SetEase(Ease.InCubic));

        // Update tile references
        emptyTile.ChangeIcon(fallingTile.GetIcon());
        emptyTile.Initialize(_tileDataSOs.FirstOrDefault(c => c.TileType.Equals(fallingTile.GetModelTileType())));
        fallingTile.Initialize(_emptyTileDataSO);
        fallingTile.ChangeIcon(null);

        fellTiles.Add(emptyTile);
    }

    private void CreateNewTileAt(int x, int y, Sequence sequence, List<TileController> fellTiles)
    {
        TileController emptyTile = GetTileAt(x, y);

        // Get a new icon from the pool
        var pooledIcon = _tilesPool.GetPooledObject();
        pooledIcon.transform.SetParent(_overlappingParent);
        pooledIcon.transform.position = new Vector3(emptyTile.transform.position.x, _tileGrid[0, 0].transform.position.y + 1, emptyTile.transform.position.z); // Start above the grid

        // Select a random TileDataSO for the new tile
        TileDataSO tileData = _tileDataSOs[Random.Range(0, _tileDataSOs.Length)];
        
        // Animate the new tile falling into place
        Vector3 targetPosition = emptyTile.transform.position;
        sequence.Join(pooledIcon.transform.DOMove(targetPosition, 0.6f).SetEase(Ease.InCubic));

        // Update tile references
        emptyTile.ChangeIcon(pooledIcon as IconHandler);
        emptyTile.Initialize(tileData);

        fellTiles.Add(emptyTile);
    }

    private void ReconnectFellTiles(List<TileController> fellTiles)
    {
        foreach (var tile in fellTiles)
        {
            if (tile.GetIcon().transform != null)
            {
                tile.ConnectIconToParent();
            }
        }
    }



}