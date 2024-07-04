using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class VillageCenter : InteractObject
{
    private MenuUI _menuUI;
    
    public override void Interact()
    {
        base.Interact();
        LoadGameplayUI();
    }

    private void LoadGameplayUI()
    {
        Timing.RunCoroutine(ShowAndGetGameplayUI());
    }

    private IEnumerator<float> ShowAndGetGameplayUI()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(
            UIManager.Instance.LoadAndShowPrefab<MenuUI>("MENU_UI", UIManager.Instance.canvasOverlay)));
        _menuUI = UIManager.Instance.GetUI<MenuUI>("MENU_UI");
        
    }
}
