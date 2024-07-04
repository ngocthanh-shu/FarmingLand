using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    private readonly Rigidbody2D _rb;

    private InputAction _moveAction;

    private Vector2 _direction;

    private float _speed;

    public PlayerMovement(Rigidbody2D rb, PlayerInput input)
    {
        _rb = rb;
        Initialize(input);
    }

    private void Initialize(PlayerInput input)
    {
        _direction = new Vector2();

        _moveAction = input.Player.Move;

        _speed = 1f;
    }

    public void EnableAction()
    {
        _moveAction.Enable();
    }

    public void DisableAction()
    {
        _moveAction.Disable();
    }

    public void InitializeAction()
    {
        _moveAction.performed += Move;
        _moveAction.canceled += Move;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        _direction = obj.ReadValue<Vector2>();
        var vel = _rb.velocity;
        vel.x = _speed * _direction.x;
        vel.y = _speed * _direction.y;
        _rb.velocity = vel;
    }
}
