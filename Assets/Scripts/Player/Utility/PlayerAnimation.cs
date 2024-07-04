using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation
{
    private readonly Animator _animator;
    private readonly InputAction _moveAction;
    private Vector2 _direction;
    private static readonly int PosX = Animator.StringToHash("posX");
    private static readonly int PosY = Animator.StringToHash("posY");
    private static readonly int IsMove = Animator.StringToHash("IsMove");

    public PlayerAnimation(Animator animator, PlayerInput playerInput)
    {
        _animator = animator;
        _moveAction = playerInput.Player.Move;
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
        _moveAction.performed += MoveAnimation;
        _moveAction.canceled += CancelMoveAnimation;
    }

    private void CancelMoveAnimation(InputAction.CallbackContext obj)
    {
        _animator.SetBool(IsMove, false);
    }

    private void MoveAnimation(InputAction.CallbackContext obj)
    {
        _direction = obj.ReadValue<Vector2>();
        
        _animator.SetBool(IsMove, true);
        _animator.SetFloat(PosX, _direction.x);
        _animator.SetFloat(PosY, _direction.y);
    }
}
