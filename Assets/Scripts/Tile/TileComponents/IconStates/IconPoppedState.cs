using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPoppedState : IconBaseState
{
    RawImage _iconIdleImage;
    GameObject _popObjectVFX;
    GameObject _specialPopObjectVFX;
    float _deactivateEffectTime;
    Color _breakingVFXColor;

    public event System.Action OnPopComplete;
    float _popDuration = 1.0f;
    IconHandler _icon;
    public IconPoppedState(IconHandler icon,RawImage iconIdleImage, GameObject popVFX,GameObject specialVFX, Color breakingVFXColor, float deactivateEffectTime =0.5f)
    {
        _iconIdleImage = iconIdleImage;
        _popObjectVFX = popVFX;
        _specialPopObjectVFX = specialVFX;
        _breakingVFXColor = breakingVFXColor;
        _deactivateEffectTime = deactivateEffectTime;
        _icon = icon;
    }
    public override void OnEnter()
    {
        _iconIdleImage.gameObject.SetActive(true);
        _iconIdleImage.enabled = false;
        foreach(Transform VFXSystem in _popObjectVFX.transform)
        {
            ParticleSystem.MainModule main = VFXSystem.GetComponent<ParticleSystem>().main;
            if(VFXSystem == _popObjectVFX.transform.GetChild(3))
            {
                Color color = _breakingVFXColor;
                color.a = 1;
                main.startColor = color;
                continue;
            }
            main.startColor = _breakingVFXColor;
        }
            _popObjectVFX.SetActive(true);
        if (_icon.IsSpecial)
            _specialPopObjectVFX.SetActive(true);
        _iconIdleImage.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(WaitForPopComplete());
    }
    public override void OnExit() 
    {
        _iconIdleImage.gameObject.SetActive(false);
        _iconIdleImage.enabled = true;
        _popObjectVFX.SetActive(false);
        _specialPopObjectVFX.SetActive(false);

    }
    private IEnumerator WaitForPopComplete()
    {
        yield return new WaitForSeconds(_deactivateEffectTime);
        OnPopComplete?.Invoke(); // Notify that the pop effect is complete
        _iconIdleImage.gameObject.SetActive(false);
        _iconIdleImage.enabled = true;
        _popObjectVFX.SetActive(false);
        _specialPopObjectVFX.SetActive(false);
    }
    public void SetNewPopColor(Color color)
    {
        _breakingVFXColor = color;
    }
   
}
