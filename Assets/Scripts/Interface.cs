public interface IView
{
    public void Initialize(IViewData data = null);
}

public interface IViewData
{
    
}

public interface IInteract
{
    public void Interact();
}

public interface IInteractData
{
    
}

public interface IUI
{
    public void Initialize();
    public void InitializeAction();
    public void InitializeAnimation();
    public void SetDefault();
    public void SetData(IUIData data = null);
    public void ShowUI(IUIData data = null);
    public void HideUI();
}

public interface IUIData
{
    
}
