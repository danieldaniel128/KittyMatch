using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public Vector2Int GridSize => new Vector2Int(Width, Height);
    public GameObject basicTilePrefab;
    [SerializeField] private SwipeInputHandler _swipeInputHandler;
    [SerializeField] private Transform _overlappingParent;
    [SerializeField] private Transform _tilesHolder;
    [SerializeField] private TilePool _tilesPool;
    [SerializeField] private MatchHandler _matchHandler;
    [SerializeField] private TileDataSO[] _tilesDataSOs;
    [SerializeField] private SpecialTileDataSO[] _specialTilesDataSOs;
    [SerializeField] private TileDataSO _emptyTileDataSO;
    private Dictionary<Vector2Int,TileController> _tilesDictionary;

    private TileController _firstSelectedTile = null;

    [SerializeField] bool _isSwapping;
    [SerializeField] bool _isMatching;
    public PowerUp _hammerPowerUp;
    public PowerUp _bombPowerUp;
    public PowerUp _swapPowerUp;
    public PowerUp _selectedPowerUp;

    void Awake()
    {
        //add clean actions for cleaning memory leak.
        _hammerPowerUp.effect = (tile) => Debug.Log($"Hammer effect applied on {tile}");//change to real effect later
        _bombPowerUp.effect = (tile) => Debug.Log($"Bomb tiles effect applied on {tile}");
        _swapPowerUp.effect = (tile) => Debug.Log($"swap 2 tiles effect applied on {tile}");
    }
    private void Start()
    {
        InitializeGrid();
        _swipeInputHandler.OnSwipe.AddListener(HandleTileSwipe);
        _swipeInputHandler.OnTap.AddListener(OnTapTile);
    }
    private void OnDestroy()
    {
        _swipeInputHandler.OnSwipe.RemoveAllListeners();
        _swipeInputHandler.OnTap.RemoveAllListeners();
    }
    private void OnApplicationQuit()
    {
        _swipeInputHandler.OnSwipe.RemoveAllListeners();
        _swipeInputHandler.OnTap.RemoveAllListeners();
    }

    private void InitializeGrid()
    {
        _tilesDictionary = new Dictionary<Vector2Int, TileController>();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, _tilesHolder.position.z), Quaternion.identity, _tilesHolder);
                newTileObject.name = $"Tile({x},{y})";
                TileController tileComponent = newTileObject.GetComponent<TileController>();
                TileDataSO tileData = _tilesDataSOs[Random.Range(0, _tilesDataSOs.Length)];
                int maxAttempts = 100; // Limit attempts to avoid infinite loop
                int attempts = 0;

                if (_tilesDataSOs != null && _tilesDataSOs.Length > 1)
                {
                    while (WouldCauseMatch(x, y, tileData) && attempts < maxAttempts)
                    {
                        tileData = _tilesDataSOs[Random.Range(0, _tilesDataSOs.Length)];
                        attempts++;
                    }
                }
                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning($"Max attempts reached at position ({x}, {y}). Using fallback tile.");
                    // Optionally assign a fallback tile or leave as is
                }

                // Initialize the tile with the properties from the selected TileDataSO
                tileComponent.Initialize(tileData);
                tileComponent.AttachPool(_tilesPool);
                //set the event of OnSelectedTile.
                tileComponent.OnTrySelectingTile.AddListener(OnTileDragged); // Register listener
                //set index for each created tile.
                tileComponent.SetTileIndex(x, y);
                //add to list of tiles.
                _tilesDictionary.Add(new Vector2Int(x,y),tileComponent);
                //add to grid of tiles.
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
            _tilesDictionary[new Vector2Int(x - 1,y)].GetModelTileType().Equals(tileData.TileType) &&
            _tilesDictionary[new Vector2Int(x - 2, y)].GetModelTileType().Equals(tileData.TileType))
        {
            return true;
        }

        // Check for vertical match
        if (y >= 2 &&
            _tilesDictionary[new Vector2Int(x, y - 1)].GetModelTileType().Equals(tileData.TileType) &&
            _tilesDictionary[new Vector2Int(x, y - 2)].GetModelTileType().Equals(tileData.TileType))
        {
            return true;
        }

        return false;
    }
    public TileController GetTileAt(int x, int y) => _tilesDictionary[new Vector2Int(x, y)];
    public TileController GetTileAt(Vector2Int tileIndexSearch) => _tilesDictionary[tileIndexSearch];
    private void HandleTileSwipe(Vector2Int swipeDirection)
    {
        if (_firstSelectedTile == null)
            return;
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
        if (_firstSelectedTile != null)
        {
            _firstSelectedTile.OnDeSelectedTile?.Invoke(false);
            _firstSelectedTile.GetIcon().transform.transform.DOScale(Vector3.one, 0.1f);
            _firstSelectedTile = null;
        }
    }
    private async void OnTapTile()
    {
        TileController tappedTile = _firstSelectedTile;
        DeselectDrag();
        if (tappedTile != null)
        {
            Debug.Log("tapped tile at: " + tappedTile.TileIndex);
            if (tappedTile.GetModelTileType().Equals("AllColors"))
            {
                await AllColorsPowerUpOnTile(tappedTile);
            }
            else if (tappedTile.GetModelTileType().Equals("4Row"))
            {
                await FullLinePowerUpOnTile(tappedTile, true);
            }
            else if (tappedTile.GetModelTileType().Equals("4Column"))
            {
                await FullLinePowerUpOnTile(tappedTile, false);
            }
            else if (tappedTile.GetModelTileType().Equals("TShape"))
            {
                await BombPowerUpOnTile(tappedTile);
            }
            else return;
        }
        else return;
        MoveManager.Instance.UseMove();
        _isMatching = true;
        //update grid after first match.
        List<TileController> fellTiles = new List<TileController>();
        //if a specail popped, empty cells need to fill
        await FillEmptySpaces(fellTiles);
        List<Match> matches = _matchHandler.DetectMatches(_tilesDictionary, Width, Height); ;

        // Process matches until none are left
        while (matches.Count > 0)
        {
            //update grid from falling tiles.
            await GridUpdateAfterFallingTiles(matches, fellTiles);
            fellTiles.Clear();
            await FillEmptySpaces(fellTiles);
            matches = _matchHandler.DetectMatches(_tilesDictionary, Width, Height);
        }
        _isMatching = false;
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
        TileDataSO tmpTileDataSO = _specialTilesDataSOs.Cast<TileDataSO>().Concat(_tilesDataSOs).FirstOrDefault(c=> c.TileType.Equals(tile1.GetModelTileType()));
        tile1.Initialize(_specialTilesDataSOs.Cast<TileDataSO>().Concat(_tilesDataSOs).FirstOrDefault(c => c.TileType.Equals(tile2.GetModelTileType())));
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
        TileController tile1 = GetTileAt(pos1);
        TileController tile2 = GetTileAt(pos2);
        _isSwapping = true;
        await SwapTiles(pos1, pos2);
        _isSwapping = false;
        var matches = _matchHandler.DetectMatches(_tilesDictionary, Width,Height);
        // Swap back if no matches found
        if (tile1.GetModelTileType().Equals("AllColors") || tile2.GetModelTileType().Equals("AllColors"))
        {
            if (tile1.GetModelTileType().Equals("AllColors"))
                await AllColorsPowerUpOnTile(tile1, tile2);
            else
                await AllColorsPowerUpOnTile(tile2, tile1);
        }
        else if (tile1.GetModelTileType().Equals("4Row") || tile2.GetModelTileType().Equals("4Row"))
        {
            if (tile1.GetModelTileType().Equals("4Row"))
            {
                await FullLinePowerUpOnTile(tile1, true);
            }
            else
            {
                await FullLinePowerUpOnTile(tile2, true);
            }
        }
        else if (tile1.GetModelTileType().Equals("4Column") || tile2.GetModelTileType().Equals("4Column"))
        {
            if (tile1.GetModelTileType().Equals("4Column"))
            {
                await FullLinePowerUpOnTile(tile1, false);
            }
            else
            {
                await FullLinePowerUpOnTile(tile2, false);
            }
        }
        else if (tile1.GetModelTileType().Equals("TShape") || tile2.GetModelTileType().Equals("TShape"))
        {
            if (tile1.GetModelTileType().Equals("TShape"))
            {
                await BombPowerUpOnTile(tile1);
            }
            else
                await BombPowerUpOnTile(tile2);
        }
        else if (matches.Count == 0)
        {
            _isSwapping = true;
            await SwapTiles(pos1, pos2);
            _isSwapping = false;
            return; // Exit early as no match was found
        }
        
        MoveManager.Instance.UseMove();
        //there is a match.
        _isMatching = true;
        //update grid after first match.
        List<TileController> fellTiles = new List<TileController>();
        //if a specail popped, empty cells need to fill
        await GridUpdateAfterSwap(tile1, tile2, matches);
        await FillEmptySpaces(fellTiles);
        matches = _matchHandler.DetectMatches(_tilesDictionary, Width, Height);
        // Process matches until none are left
        while (matches.Count > 0)
        {
            await GridUpdateAfterFallingTiles(matches, fellTiles);
            fellTiles.Clear();
            await FillEmptySpaces(fellTiles);
            matches = _matchHandler.DetectMatches(_tilesDictionary, Width, Height);
        }
        _isMatching = false;
    }
    private async Task GridUpdateAfterFallingTiles(List<Match> matches,List<TileController> fellTiles)
    {
        // Pop matched tiles
        var popTasks = new List<Task>();
        var poppedTiles = new HashSet<TileController>();
        // Track matches that already had a special tile assigned
        var processedMatches = new HashSet<Match>();
        HashSet<TileController> newlyCreatedSpecials = new HashSet<TileController>();
        foreach (Match match in matches)
        {
            // Check if the match is special
            foreach (TileController tile in match.Tiles)
            {
                if(newlyCreatedSpecials.Contains(tile))
                    continue;
                if (fellTiles != null)
                {
                    
                    if (match.IsSpecial && !processedMatches.Contains(match))//is special and one of the falling tiles is in the special match
                    {
                        // Find the first tile from the falling tiles that is part of the current match
                        TileController firstFellTileInMatch = fellTiles.FirstOrDefault(c => match.Tiles.Contains(c));

                        if (firstFellTileInMatch != null)
                        {
                            // Assign a special icon to this tile and mark the match as processed
                            AssignSpecialTile(firstFellTileInMatch, match);
                            processedMatches.Add(match); // Ensure only one special tile per match
                            newlyCreatedSpecials.Add(firstFellTileInMatch);
                        }
                    }
                    else if (poppedTiles.Add(tile)) // Ensure each tile is processed only once
                    {
                        popTasks.Add(tile.AwaitPopIcon());
                    }
                }
            }
        }
        await Task.WhenAll(popTasks); // Wait for all pops to complete (optional)
        Debug.Log("Pop finished");

        // Clear icons for matched tiles (use the same poppedTiles set)
        foreach (TileController tile in poppedTiles)
        {
            tile.ChangeIcon(null);
            tile.Initialize(_emptyTileDataSO);
        }
    }


    private async Task GridUpdateAfterSwap(TileController tile1, TileController tile2, List<Match> matches)
    {
        // Pop matched tiles
        var popTasks = new List<Task>();
        var poppedTiles = new HashSet<TileController>();
        HashSet<TileController> newlyCreatedSpecials = new HashSet<TileController>();
        foreach (Match match in matches)
        {
            // Check if the match is special
            foreach (TileController tile in match.Tiles)
            {
                if (newlyCreatedSpecials.Contains(tile))
                    continue;
                switch (tile.GetModelTileType())
                {
                    case "TShape":
                        // Get the center tile's coordinates
                        var center = tile.TileIndex;
                        // Iterate through the 3x3 area around the center
                        for (int x = center.x - 1; x <= center.x + 1; x++)
                        {
                            for (int y = center.y - 1; y <= center.y + 1; y++)
                            {
                                // Ensure the coordinates are within the grid bounds
                                if (_tilesDictionary.TryGetValue(new Vector2Int(x, y), out var affectedTile))
                                {
                                    // Trigger the pop for each tile in the 3x3 area
                                    if (poppedTiles.Add(affectedTile))
                                        popTasks.Add(affectedTile.AwaitPopIcon());
                                }
                            }
                        }
                        break;
                    case "4Row":
                        foreach (var tileincol in _tilesDictionary.Where(c => c.Key.y == tile.Y))
                            if (poppedTiles.Add(GetTileAt(tileincol.Key)))
                                popTasks.Add(GetTileAt(tileincol.Key).AwaitPopIcon());
                        break;
                    case "4Column":
                        foreach (var tileincol in _tilesDictionary.Where(c => c.Key.x == tile.X))
                            if (poppedTiles.Add(GetTileAt(tileincol.Key)))
                                popTasks.Add(GetTileAt(tileincol.Key).AwaitPopIcon());
                        break;
                    default://not special
                        if (match.IsSpecial && (tile == tile1 || tile == tile2))//is special and one of the swaps
                        {
                            // Assign a special icon to one tile in the match
                            AssignSpecialTile(tile, match);
                            newlyCreatedSpecials.Add(tile);
                        }
                        else if (poppedTiles.Add(tile)) // Ensure each tile is processed only once
                        {
                            popTasks.Add(tile.AwaitPopIcon());
                        }
                        break;
                }
            }
        }
        await Task.WhenAll(popTasks); // Wait for all pops to complete (optional)
        Debug.Log("Pop finished");

        // Clear icons for matched tiles (use the same poppedTiles set)
        foreach (var tile in poppedTiles)
        {
            tile.ChangeIcon(null);
            tile.Initialize(_emptyTileDataSO);
        }
    }

    // Method to assign a special icon to a tile in a special match
    private void AssignSpecialTile(TileController newSpecialTile, Match match)
    {
        Debug.Log($"Special tile created at {newSpecialTile.TileIndex} with icon {match.MatchType.ToString()}");
        SpecialTileDataSO specialTileDataSO = _specialTilesDataSOs.FirstOrDefault(c => c.SpecialMatchType == match.MatchType);
        if (specialTileDataSO == null)
            Debug.Log(specialTileDataSO + " is null");
        newSpecialTile.Initialize(specialTileDataSO);
        if(match.MatchType == SpecialMatch.FourColumn || match.MatchType == SpecialMatch.FourRow)
            newSpecialTile.AssignSpecialIcon();
    }

    // Method to determine which special icon to use based on the match shape or size

     async Task AllColorsPowerUpOnTile(TileController allColorTile,TileController swappedTile = null)
     {
        var popTasks = new List<Task>();
        var poppedTiles = new HashSet<TileController>();
        if (poppedTiles.Add(allColorTile))
            popTasks.Add(allColorTile.AwaitPopIcon());
        string allColoredSpecialTileColor = string.Empty;
        if (swappedTile != null)
            allColoredSpecialTileColor = swappedTile.GetModelTileType();
        else
        {
            var center = allColorTile.TileIndex;

            // Define possible directions
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),  // Above
                new Vector2Int(0, -1), // Below
                new Vector2Int(-1, 0), // Left
                new Vector2Int(1, 0)   // Right
            };

            // Select a random direction
            var randomDirection = directions[Random.Range(0, directions.Length)];

            // Calculate the target tile index
            var targetTileIndex = center + randomDirection;

            // Check if the target tile exists in the grid
            if (_tilesDictionary.TryGetValue(targetTileIndex, out var affectedTile))
            {
                allColoredSpecialTileColor = affectedTile.GetModelTileType();
            }
        }
        foreach (var tileincol in _tilesDictionary.Where(c => c.Value.GetModelTileType().Equals(allColoredSpecialTileColor)))
            if (poppedTiles.Add(GetTileAt(tileincol.Key)))
                popTasks.Add(GetTileAt(tileincol.Key).AwaitPopIcon());
        await Task.WhenAll(popTasks);
        // Clear icons for matched tiles (use the same poppedTiles set)
        foreach (var tile in poppedTiles)
        {
            tile.ChangeIcon(null);
            tile.Initialize(_emptyTileDataSO);
        }
     }
    async Task BombPowerUpOnTile(TileController selectedTile)
    {
        var popTasks = new List<Task>();
        var poppedTiles = new HashSet<TileController>();            // Get the center tile's coordinates
        var center = selectedTile.TileIndex;
        // Iterate through the 3x3 area around the center
        for (int x = center.x - 1; x <= center.x + 1; x++)
        {
            for (int y = center.y - 1; y <= center.y + 1; y++)
            {
                // Ensure the coordinates are within the grid bounds
                if (_tilesDictionary.TryGetValue(new Vector2Int(x, y), out var affectedTile))
                {
                    // Trigger the pop for each tile in the 3x3 area
                    if (poppedTiles.Add(affectedTile))
                        popTasks.Add(affectedTile.AwaitPopIcon());
                }
            }
        }
        await Task.WhenAll(popTasks);
        // Clear icons for matched tiles (use the same poppedTiles set)
        foreach (var tile in poppedTiles)
        {
            tile.ChangeIcon(null);
            tile.Initialize(_emptyTileDataSO);
        }
    }
    /// <summary>
    /// pop all in horizontal line or vetical line.
    /// </summary>
    /// <param name="specailLineTile"></param>
    /// <param name="isHorizontal"></param>
    /// <returns></returns>
    async Task FullLinePowerUpOnTile(TileController specailLineTile, bool isHorizontal)
    {
        var popTasks = new List<Task>();
        var poppedTiles = new HashSet<TileController>();
        foreach (var tileincol in _tilesDictionary.Where(c => (isHorizontal ? c.Key.y == specailLineTile.Y : c.Key.x == specailLineTile.X)))
            if (poppedTiles.Add(GetTileAt(tileincol.Key)))
                popTasks.Add(GetTileAt(tileincol.Key).AwaitPopIcon());
        await Task.WhenAll(popTasks);
        // Clear icons for matched tiles (use the same poppedTiles set)
        foreach (var tile in poppedTiles)
        {
            tile.ChangeIcon(null);
            tile.Initialize(_emptyTileDataSO);
        }
    }
    public async Task FillEmptySpaces(List<TileController> fellTiles)
    {
        fellTiles.Clear();
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
        //join special and regular lists to fix falling when there is a special bug.
        emptyTile.Initialize(_specialTilesDataSOs.Cast<TileDataSO>().Concat(_tilesDataSOs).FirstOrDefault(c => c.TileType.Equals(fallingTile.GetModelTileType())));
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
        pooledIcon.transform.position = new Vector3(emptyTile.transform.position.x, GetTileAt(0,0).transform.position.y + 1, emptyTile.transform.position.z); // Start above the grid

        // Select a random TileDataSO for the new tile
        TileDataSO tileData = _tilesDataSOs[Random.Range(0, _tilesDataSOs.Length)];
        
        // Animate the new tile falling into place
        Vector3 targetPosition = emptyTile.transform.position;
        sequence.Join(pooledIcon.transform.DOMove(targetPosition, 0.6f).SetEase(Ease.InCubic));

        // Update tile references
        emptyTile.ChangeIcon(pooledIcon as IconHandler);
        emptyTile.Initialize(tileData);
        emptyTile.UnAssignSpecialIcon();

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