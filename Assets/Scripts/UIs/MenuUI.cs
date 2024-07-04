using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuUI : BaseUI
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buildingMenuBtn;
    [SerializeField] private Button landMenuBtn;
    [SerializeField] private Button seedMenuBtn;

    [SerializeField] private LandMenu landMenu;
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject seedMenu;
    
    [Header("Animation Ajust")] 
    [SerializeField] private float starPosY;
    [SerializeField] private float endPosY;
    [SerializeField] private int duration;
    
    
    private RectTransform _rect;
    private CanvasGroup _canvas;
    
    private Sequence _hideSequence;
    private Sequence _showSequence;

    public override void Initialize()
    {
        base.Initialize();
        landMenu.Initialize();
    }

    public override void SetDefault()
    {
        base.SetDefault();
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponent<CanvasGroup>();
        buildingMenu.SetActive(true);
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, endPosY);
    }

    public override void InitializeAnimation()
    {
        base.InitializeAnimation();
        SetShowAnimation();
        SetHideAnimation();
    }

    private void SetShowAnimation()
    {
        _showSequence = DOTween.Sequence();
        _showSequence.Append(_rect.DOAnchorPosY(starPosY, duration));
        _showSequence.Join(_canvas.DOFade(0f, 0));
        _showSequence.Join(_canvas.DOFade(1f, duration));
        _showSequence.SetAutoKill(false);
    }

    private void SetHideAnimation()
    {
        _hideSequence = DOTween.Sequence();
        _hideSequence.Append(_rect.DOAnchorPosY(endPosY, duration));
        _hideSequence.Join(_canvas.DOFade(0f, duration));
        _hideSequence.OnComplete(HideUI);
        _hideSequence.SetAutoKill(false);
    }

    public override void InitializeAction()
    {
        base.InitializeAction();
        closeBtn.onClick.AddListener(OnCloseUI);
        landMenuBtn.onClick.AddListener(OpenLandMenu);
        buildingMenuBtn.onClick.AddListener(OpenBuildingMenu);
        seedMenuBtn.onClick.AddListener(OpenSeedMenu);
    }

    private void OpenSeedMenu()
    {
        SetActiveForMenu(seedMenu);
        OutLandingMode();
    }

    private void OpenBuildingMenu()
    {
        SetActiveForMenu(buildingMenu);
        OutLandingMode();
    }

    private void OpenLandMenu()
    {
        SetActiveForMenu(landMenu.gameObject);
        OnLandingMode();
    }

    private void OnLandingMode()
    {
        TileManager.Instance.SetActionStatus(ActionStatus.Building);
        TileManager.Instance.SetActiveGrid(true);
    }

    private void OutLandingMode()
    {
        TileManager.Instance.RefreshReviewTileMap();
        TileManager.Instance.SetActiveGrid(false);
    }

    private void SetActiveForMenu(GameObject menu)
    {
        buildingMenu.SetActive(false);
        landMenu.gameObject.SetActive(false);
        seedMenu.SetActive(false);
        
        menu.SetActive(true);
    }

    public override void ShowUI(IUIData data = null)
    {
        base.ShowUI(data);
        if(!_hideSequence.IsPlaying() && !_showSequence.IsPlaying() && !Mathf.Approximately(_rect.anchoredPosition.y, starPosY))
            _showSequence.Restart();
    }

    private void OnCloseUI()
    {
        OutLandingMode();
        TileManager.Instance.SetActionStatus(ActionStatus.Interact);
        _hideSequence.Restart();
    }
    
}
