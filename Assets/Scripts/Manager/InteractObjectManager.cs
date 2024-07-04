using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractObjectManager : Singleton<InteractObjectManager>
{
    private InputAction _clickAction;
    private Camera _camera;
    
    public void Initialize(PlayerInput playerInput, Camera camObj)
    {
        _camera = camObj;
        _clickAction = playerInput.Gameplay.Click;
        
        InitializeAction();
        Enable();
    }

    private void InitializeAction()
    {
        _clickAction.started += OnlickScreen;
    }

    private void OnlickScreen(InputAction.CallbackContext obj)
    {
        RaycastHit2D rayHit =
            Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if(!rayHit) return;
        GameObject target = rayHit.collider.gameObject;
        InteractObject interactObject = target.GetComponent<InteractObject>();
        if (!interactObject) return;
        interactObject.Interact();
    }

    public void Enable()
    {
        _clickAction.Enable();
    }

    public void Disable()
    {
        _clickAction.Disable();
    }
}
