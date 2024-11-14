using System;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Tile
{
    public class TileView
    {
        private RectTransform _tileRectTransform;
        private RawImage _icon;
        private Vector2 _originalSize;

        public TileView(RawImage icon, RectTransform tileRT, Vector2 originalSize)
        {
            _icon = icon ?? throw new ArgumentNullException(nameof(icon));
            _tileRectTransform = tileRT ?? throw new ArgumentNullException(nameof(tileRT));
            _originalSize = originalSize;
        }

        public void SetNewTileIcon(Sprite spriteIcon)
        {
            if (spriteIcon == null)
            {
                Debug.LogWarning("Sprite icon is null, cannot set tile icon.");
                return;
            }

            // Set the texture of the RawImage to the sprite sheet texture
            Rect spriteRect = spriteIcon.textureRect;
            Texture2D spriteTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
            spriteTexture.SetPixels(spriteIcon.texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height));
            spriteTexture.Apply();

            _icon.texture = spriteTexture;
            _icon.SetNativeSize();
            AdjustSize(spriteTexture);
            //
            Canvas.ForceUpdateCanvases();

        }

        private void AdjustSize(Texture2D texture)
        {
            float size_factor = 1;
            if (_icon.texture.width > _icon.texture.height) //landscape
            {
                size_factor = _originalSize.x / _icon.texture.width;
            }
            else //portrait
            {
                size_factor = _originalSize.y / _icon.texture.height;
            }

            _tileRectTransform.sizeDelta *= size_factor;
        }
        //make an handler that holds every vfx. ask him to show the current vfx of the tile state.
        //public void ActivateSelectedVFX()
        //{
        //    _selectedTileVFX.SetActive(true);
        //}
        //public void DeActivateSelectedVFX()
        //{
        //    _selectedTileVFX.SetActive(false);
        //}
    }
}
