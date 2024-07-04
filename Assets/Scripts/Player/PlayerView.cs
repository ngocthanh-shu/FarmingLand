using UnityEngine;

public class PlayerView : GeneralView
{
    private PlayerMovement _playerMovement;
    private PlayerFarming _playerFarming;

    private PlayerAnimation _playerAnimation;
    
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private PlayerInput _playerInput;

    private Animator _animator;

    public override void Initialize(IViewData data)
    {
        base.Initialize(data);
        
        //Awake
        PlayerViewData playerData = (PlayerViewData)data;

        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        
        _playerInput = playerData.PlayerInput;
        
        
        _playerMovement = new PlayerMovement(_rb, _playerInput);
        _playerFarming = new PlayerFarming(_playerInput, GetPlayerPosition);
        _playerAnimation = new PlayerAnimation(_animator, _playerInput);
        
        //OnEnable
        Enable();
        
        //Start
        _playerMovement.InitializeAction();
        _playerAnimation.InitializeAction();
    }
    

    protected override void Enable()
    {
        base.Enable();
        _playerMovement.EnableAction();
        _playerAnimation.EnableAction();
    }

    protected override void Disable()
    {
        base.Disable();
        _playerMovement.DisableAction();
        _playerAnimation.DisableAction();
    }

    public void SetPlayerRunSpeed(float runSpeed)
    {
        _playerMovement.SetSpeed(runSpeed);
    }

    public void OnFixUpdate()
    {
        
    }

    private Vector2 GetPlayerPosition()
    {
        return transform.position;
    }
}

public class PlayerViewData : IViewData
{
    public PlayerInput PlayerInput;

    public PlayerViewData(PlayerInput input)
    {
        PlayerInput = input;
    }
}
