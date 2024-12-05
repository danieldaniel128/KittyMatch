using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPoppedState : IconBaseState
{
    RawImage _iconIdleImage;
    GameObject _popObjectVFX;
    float _deactivateEffectTime;
    Color _breakingVFXColor;

    public event System.Action OnPopComplete;
    float _popDuration = 1.0f;
    public IconPoppedState(RawImage iconIdleImage, GameObject popVFX, Color breakingVFXColor, float deactivateEffectTime =0.5f)
    {
        _iconIdleImage = iconIdleImage;
        _popObjectVFX = popVFX;
        _breakingVFXColor = breakingVFXColor;
        _deactivateEffectTime = deactivateEffectTime;
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

        _iconIdleImage.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(WaitForPopComplete());
    }
    public override void OnExit() 
    {
        _iconIdleImage.gameObject.SetActive(false);
        _iconIdleImage.enabled = true;
        _popObjectVFX.SetActive(false);

    }
    private IEnumerator WaitForPopComplete()
    {
        yield return new WaitForSeconds(_deactivateEffectTime);
        OnPopComplete?.Invoke(); // Notify that the pop effect is complete
        _iconIdleImage.gameObject.SetActive(false);
        _iconIdleImage.enabled = true;
        _popObjectVFX.SetActive(false);
    }
    public void SetNewPopColor(Color color)
    {
        _breakingVFXColor = color;
    }
   
}
