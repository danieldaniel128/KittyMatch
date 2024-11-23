using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Header("Popped Parameters")]
    [SerializeField] GameObject _popVFX;
    [SerializeField] ParticleSystem _breakEffect;
    [SerializeField] float _popDuration;

    private TaskCompletionSource<bool> _popTaskCompletionSource;
    private void Awake()
    {
        _stateMachine = new StateMachine();
        //create hashset to states
        _iconStates = new Dictionary<System.Type, IState>
        {
            { typeof(IconIdleState), new IconIdleState(_idleIconImage) },
            { typeof(IconSelectedState), new IconSelectedState(_selectedIconImage,_selectedMaterial) },
            { typeof(IconPoppedState), new IconPoppedState(_idleIconImage,_popVFX,_popDuration,_breakEffect) }
        };

        At(GetState<IconIdleState>(), GetState<IconSelectedState>(), new FuncPredicate(() => IsSelected));
        At(GetState<IconSelectedState>(), GetState<IconPoppedState>(), new FuncPredicate(() => IsPopping));
        At(GetState<IconIdleState>(), GetState<IconPoppedState>(), new FuncPredicate(() => IsPopping));
        Any(GetState<IconIdleState>(), new FuncPredicate(() => !IsPopping && !IsSelected));
        GetState<IconPoppedState>().OnPopComplete += HandlePopComplete;
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
        IsSelected = false;
        IsPopping = false;
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

    public async Task AwaitPop()
    {
        _popTaskCompletionSource = new TaskCompletionSource<bool>();
        await _popTaskCompletionSource.Task;
    }

    private void NotifyPopComplete()
    {
        _popTaskCompletionSource?.TrySetResult(true);
    }
    public void HandlePopComplete()
    {
        //return to pool for reuse icon for other tiles.
        Pool.ReturnToPool(this);
        //finished popping.
        IsPopping = false;
        // Notify that the pop is complete.
        NotifyPopComplete();
    }

}       
