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

        public void SetNewTileIcon(Texture2D iconTexture)
        {
            if (iconTexture == null)
            {
                Debug.LogWarning("icon texture is null, cannot set tile icon.");
                return;
            }

            // Set the texture of the RawImage to the sprite sheet texture

            _icon.texture = iconTexture;
            _icon.SetNativeSize();
        }

        ///in case the texture is not in the currect size fitting to parent, make it fit approximetly _originalSize.
        //private void AdjustSize(Texture2D texture)
        //{
        //    float size_factor = 1;
        //    if (_icon.texture.width > _icon.texture.height) //landscape
        //    {
        //        size_factor = _originalSize.x / _icon.texture.width;
        //    }
        //    else //portrait
        //    {
        //        size_factor = _originalSize.y / _icon.texture.height;
        //    }
        //    _tileRectTransform.sizeDelta *= size_factor;
        //}

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
