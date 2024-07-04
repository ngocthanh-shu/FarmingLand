using System;
using UnityEngine;

public class GameManadger : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerInput _playerInput;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
        _playerInput = new PlayerInput();
        
        InitializeManager();
        
        //_playerController = new PlayerController(_playerInput);
    }

    private void Start()
    {
        
    }

    private void InitializeManager()
    {
        ScriptableObjectManager.Instance.Initialize();
        TileManager.Instance.Initialize();
        ViewManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        InteractObjectManager.Instance.Initialize(_playerInput, _camera);
    }

    private void FixedUpdate()
    {
        //_playerController.OnFixUpdate();
    }
}
