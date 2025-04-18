using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _width = 8;
    [SerializeField] private int _height = 8;
    public Vector2Int GridSize => new Vector2Int(_width, _height);

    [Header("Tile Settings")]
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tileParent;
    [SerializeField] private Transform _overlappingParent;
    [SerializeField] private TilePool _tilePool;
    [SerializeField] private TileDataSO[] _tileTypes;
    [SerializeField] private SpecialTileDataSO[] _specialTileTypes;
    [SerializeField] private TileDataSO _emptyTileType;

    private BoardModel _boardModel;
    private BoardView _boardView;
    private MatchFinder _matchFinder;
    private TileSwapper _tileSwapper;
    private SpecialTileHandler _specialHandler;

    private TileController _selectedTile;

    public UnityEvent<TileController> OnTileSelected;

    private void Awake()
    {
        _boardModel = new BoardModel();
        _boardView = new BoardView(_tilePrefab, _tileParent, _tilePool, _tileTypes);
    }

    private void Start()
    {
        _boardView.InitializeGrid(_boardModel, _width, _height, HandleTileSelection);
        _matchFinder = new MatchFinder(_boardModel.GetAllTiles());
        _tileSwapper = new TileSwapper(_boardModel.GetAllTiles(), _tileTypes, _specialTileTypes, _overlappingParent);
        _specialHandler = new SpecialTileHandler(_boardModel.GetAllTiles(), _specialTileTypes, _emptyTileType);
    }


    private void HandleTileSelection(TileController tile)
    {
        if (_selectedTile == null)
        {
            _selectedTile = tile;
            tile.OnSelectedTile?.Invoke(true);
        }
        else if (_selectedTile != tile)
        {
            Vector2Int a = _selectedTile.TileIndex;
            Vector2Int b = tile.TileIndex;

            if (Vector2Int.Distance(a, b) == 1)
            {
                OnSwapTriggered(a, b);
            }

            _selectedTile.OnDeSelectedTile?.Invoke(false);
            tile.OnDeSelectedTile?.Invoke(false);
            _selectedTile = null;
        }
    }

    private async void OnSwapTriggered(Vector2Int a, Vector2Int b)
    {
        if (!_tileSwapper.TrySwap(a, b)) return;

        await _tileSwapper.PerformSwapAsync(a, b);

        var matches = _matchFinder.FindMatches();
        foreach (var match in matches)
        {
            foreach (var tile in match.Tiles)
            {
                tile.ChangeIcon(null);
                tile.Initialize(_emptyTileType);
            }
        }
    }
}