using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject basicTilePrefab;
    [SerializeField] private MatchHandler _matchHandler;

    private Tile[,] tiles;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        tiles = new Tile[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                BasicTile tileComponent = newTileObject.GetComponent<BasicTile>();
                tileComponent.Initialize(Random.ColorHSV());
                tileComponent.OnSelectedTile.AddListener(OnTileSelected); // Register listener
                tiles[x, y] = tileComponent;
            }
        }
    }

    public Tile GetTileAt(int x, int y) => tiles[x, y];
    public void SetTileAt(int x, int y, Tile tile) => tiles[x, y] = tile;
    private Vector2Int GetTilePosition(Tile tile)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y] == tile)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }
    private Tile firstSelectedTile = null;
    private void OnTileSelected(Tile selectedTile)
    {
        if (firstSelectedTile == null)
        {
            firstSelectedTile = selectedTile;
        }
        else
        {
            // Calculate positions of both tiles
            Vector2Int pos1 = GetTilePosition(firstSelectedTile);
            Vector2Int pos2 = GetTilePosition(selectedTile);

            // Call OnTileSwap with the selected positions
            OnTileSwap(pos1, pos2);

            // Reset selection
            firstSelectedTile = null;
        }
    }
    //change to dotween.
    public void SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        Tile tempTile = tiles[pos1.x, pos1.y];
        tiles[pos1.x, pos1.y] = tiles[pos2.x, pos2.y];
        tiles[pos2.x, pos2.y] = tempTile;

        // Animate the tiles' movement
        Vector3 pos1WorldPosition = tiles[pos1.x, pos1.y].transform.position;
        Vector3 pos2WorldPosition = tiles[pos2.x, pos2.y].transform.position;

        // Swap positions with animation
        tiles[pos1.x, pos1.y].transform.DOMove(pos2WorldPosition, 0.3f); // Adjust duration as needed
        tiles[pos2.x, pos2.y].transform.DOMove(pos1WorldPosition, 0.3f);
    }
    public void OnTileSwap(Vector2Int pos1, Vector2Int pos2)
    {
        SwapTiles(pos1, pos2);

        var matches = _matchHandler.DetectMatches(this);
        if (matches.Count > 0)
        {
            _matchHandler.HandleMatches(matches);
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