using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour, IUI
{
    [SerializeField] private bool destroyOnHide = false;
    private bool _isInitialize = false;
    
    public virtual void Initialize()
    {
        _isInitialize = true;
        SetDefault();
        InitializeAction();
        InitializeAnimation();
    }

    public virtual void InitializeAction()
    {
        
    }

    public virtual void InitializeAnimation()
    {
        
    }

    public virtual void SetDefault()
    {
        
    }

    public virtual void SetData(IUIData data = null)
    {
        
    }

    public virtual void ShowUI(IUIData data = null)
    {
        gameObject.SetActive(true);
    }

    public virtual void HideUI()
    {
        if(destroyOnHide) Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public virtual bool IsInit()
    {
        return _isInitialize;
    }
}
