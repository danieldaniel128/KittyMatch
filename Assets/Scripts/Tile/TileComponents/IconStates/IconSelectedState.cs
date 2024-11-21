using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSelectedState : IconBaseState
{
    RawImage _iconSelectedImage;
    Material _selectedMaterial;

    public IconSelectedState(RawImage iconSelectedImage,Material selectedMaterial) 
    {
        _iconSelectedImage = iconSelectedImage;
        _selectedMaterial = selectedMaterial;
    }
    
    public override void OnEnter()
    {
        Debug.Log("entered selected icon");
        _iconSelectedImage.gameObject.SetActive(true);
        Material selectedMaterialInstance = new Material(_selectedMaterial);
        // Set the texture of the RawImage to the sprite sheet texture
        _iconSelectedImage.material = selectedMaterialInstance;
        selectedMaterialInstance.SetTexture("_Texture2D", _iconSelectedImage.texture); // Update the material
    }
    public override void OnExit()
    {
        Debug.Log("exit selected icon");
        _iconSelectedImage.gameObject.SetActive(false);
    }
}
