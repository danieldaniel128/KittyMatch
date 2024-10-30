using UnityEngine;

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
                var newTileObject = Instantiate(basicTilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                newTileObject.transform.SetParent(transform);
                BasicTile tileComponent = newTileObject.GetComponent<BasicTile>();
                tileComponent.Initialize(Random.ColorHSV()); // Random color
                tiles[x, y] = tileComponent;
            }
        }
    }

    public Tile GetTileAt(int x, int y) => tiles[x, y];
    public void SetTileAt(int x, int y, Tile tile) => tiles[x, y] = tile;

    //change to dotween.
    public void SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        Tile temp = tiles[pos1.x, pos1.y];
        tiles[pos1.x, pos1.y] = tiles[pos2.x, pos2.y];
        tiles[pos2.x, pos2.y] = temp;

        // Update positions in Unity's world
        tiles[pos1.x, pos1.y].transform.position = new Vector3(pos1.x, pos1.y, 0);
        tiles[pos2.x, pos2.y].transform.position = new Vector3(pos2.x, pos2.y, 0);
        //swapParents
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