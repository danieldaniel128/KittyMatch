using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPoppedState : IconBaseState
{
    RawImage _iconIdleImage;
    GameObject _popObjectVFX;
    ParticleSystem _popVFXSystem;

    public event System.Action OnPopComplete;
    float _popDuration = 1.0f;
    public IconPoppedState(RawImage iconIdleImage, GameObject popVFX,float popDuration = 1.0f, ParticleSystem popVFXSystem = null)
    {
        _iconIdleImage = iconIdleImage;
        _popObjectVFX = popVFX;
        _popDuration = popDuration;
        _popVFXSystem = popVFXSystem;
    }
    public override void OnEnter()
    {
        _iconIdleImage.gameObject.SetActive(true);
        _iconIdleImage.enabled = false;
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
        yield return new WaitForSeconds(_popVFXSystem.main.startLifetimeMultiplier);
        Debug.Log("finished popping");
        OnPopComplete?.Invoke(); // Notify that the pop effect is complete
        _iconIdleImage.gameObject.SetActive(false);
        _iconIdleImage.enabled = true;
        _popObjectVFX.SetActive(false);
    }
   
}
