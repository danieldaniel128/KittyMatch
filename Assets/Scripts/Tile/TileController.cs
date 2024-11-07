using Assets.Scripts.Tile;
using UnityEngine;
using UnityEngine.EventSystems;
public class TileController : MonoBehaviour, IPointerDownHandler
{
    protected TileModel tileModel;
    protected TileView tileView;

    public void Initialize(TileModel model, TileView view)
    {
        tileModel = model;
        tileView = view;

        // Initializing the view based on model's data
        tileView.SetNewTileIcon(tileModel.TileData.TileIcon);
    }

    public  void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Tile clicked");

        // Request model to process the selection toggle and return the new state
        bool isSelected = tileModel.ToggleSelection();

        // Update view based on the processed data
        
    }

}
