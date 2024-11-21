using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class IconHandler : PooledObject
{

    StateMachine _stateMachine;
    private Dictionary<System.Type, IState> _iconStates;

    public bool IsSelected;
    public bool IsPopping;

    //animator.

    [Header("Idle Parameters")]
    [SerializeField] private RawImage _idleIconImage;
    [Header("Selected Parameters")]
    [SerializeField] private RawImage _selectedIconImage;
    [SerializeField] private Material _selectedMaterial;

    private bool _hasInit;
    private void Awake()
    {
        _stateMachine = new StateMachine();
        //create hashset to states
        _iconStates = new Dictionary<System.Type, IState>
        {
            { typeof(IconIdleState), new IconIdleState(_idleIconImage) },
            { typeof(IconSelectedState), new IconSelectedState(_selectedIconImage,_selectedMaterial) },
            { typeof(IconPoppedState), new IconPoppedState() }
        };


        At(GetState<IconIdleState>(), GetState<IconSelectedState>(), new FuncPredicate(() => IsSelected));
        At(GetState<IconSelectedState>(), GetState<IconPoppedState>(), new FuncPredicate(() => IsPopping));
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    private void OnEnable()
    {
        ResetPooledObject();
    }
    void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
    public override void ResetPooledObject()
    {
        _stateMachine.SetState(GetState<IconIdleState>());
    }

    private T GetState<T>() where T : IState
    {
        return (T)_iconStates[typeof(T)];
    }
    public void SetIconImage(Texture2D iconTexture)
    {
        _idleIconImage.texture = iconTexture;
        _selectedIconImage.texture = iconTexture;
    }
}       
