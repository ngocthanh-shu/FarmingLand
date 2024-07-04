using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFarming
{
    private InputAction _plantAction;

    private Vector2 _playerPosition;

    private Func<Vector2> _getPlayerPosition;

    public PlayerFarming(PlayerInput input, Func<Vector2> playerPostion)
    {
        Initialize(input);
        Enable();
        InitializeAction();

        _getPlayerPosition += playerPostion;
    }

    private void Initialize(PlayerInput input)
    {
        _plantAction = input.Player.Farm;
    }

    private void Enable()
    {
        _plantAction.Enable();
    }

    private void Disable()
    {
        _plantAction.Disable();
    }

    private void InitializeAction()
    {
        _plantAction.performed += OnPlant;
    }

    private void OnPlant(InputAction.CallbackContext obj)
    {
        //_playerPosition = _getPlayerPosition.Invoke();
        //Vector3Int position = TileManager.Instance.CalculatePlantPosition(_playerPosition);
        //TileManager.Instance.SetFarmingDirt(position, DirectionType.Center);
    }
}
