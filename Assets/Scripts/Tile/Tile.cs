using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Tile : MonoBehaviour , IPointerDownHandler
{
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] protected RawImage Icon;
    [SerializeField] Sprite _spriteIcon;
    [SerializeField] TileDataSO _tileDataSO;
    private Vector2 _originalSize = new Vector2(40, 40);
    public UnityEvent<Tile> OnSelectedTile;
    [SerializeField] GameObject _selectedTileVFX;
    public abstract void Activate();
    [ContextMenu("Render new sprite")]
    public virtual void InitTile(TileDataSO tileDataSO)
    {
        _tileDataSO = tileDataSO;
        if(tileDataSO.TileIcon != null)
            InitPicture(tileDataSO.TileIcon);
    }
    public virtual bool CanMatchWith(Tile otherTile)
    {
        // Default match logic for all tiles (can be overridden)
        return false;
    }
    public void DestroyTile()
    {
        Destroy(gameObject); // Destroy tile GameObject in Unity
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("tile clicked");
        OnSelectedTile?.Invoke(this);
    }
    public void InitPicture(Sprite selectedSprite)
    {
        /// Set the texture of the RawImage to the sprite sheet texture

        // Convert the sprite into a texture
        Rect spriteRect = selectedSprite.textureRect;
        Texture2D spriteTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
        spriteTexture.SetPixels(selectedSprite.texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height));
        spriteTexture.Apply();

        // Assign the texture to the RawImage component
        Icon.texture = spriteTexture;
        // Assign the texture to the RawImage component
        Icon.SetNativeSize();
        float size_factor = 1;

        if (Icon.texture.width > Icon.texture.height) //landscape
        {
            size_factor = _originalSize.x / Icon.texture.width;
        }
        else //portrait
        {
            size_factor = _originalSize.y / Icon.texture.height;
        }

        m_RectTransform.sizeDelta *= size_factor;
    }
    public void ActivateSelectedVFX()
    {
        _selectedTileVFX.SetActive(true);
    }
    public void DeActivateSelectedVFX()
    {
        _selectedTileVFX.SetActive(false);
    }
}
