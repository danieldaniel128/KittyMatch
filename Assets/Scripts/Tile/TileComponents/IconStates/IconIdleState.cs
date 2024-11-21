using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconIdleState : IconBaseState
{
    RawImage _iconIdleImage;

    public IconIdleState(RawImage iconIdleImage)
    {
        _iconIdleImage = iconIdleImage;
    }


    // Start is called before the first frame update
    public override void OnEnter()
    {
        _iconIdleImage.gameObject.SetActive(true);
    }
    public override void OnExit()
    {
        Debug.Log("exit idle state");
        _iconIdleImage.gameObject.SetActive(false);
    }
}
