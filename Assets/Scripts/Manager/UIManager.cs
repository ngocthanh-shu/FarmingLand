using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] public Transform canvasOverlay;
    
    public Dictionary<string, List<BaseUI>> DictionaryUI;
    
    private UIScriptableObject _uiScriptable;

    public void Initialize()
    {
        DictionaryUI = new Dictionary<string, List<BaseUI>>();
        _uiScriptable = (UIScriptableObject) ScriptableObjectManager.Instance.GetScriptableObject(ScriptableType.UIReference);
    }

    public T GetUI<T>(string key) where T : BaseUI
    {
        if (!DictionaryUI.ContainsKey(key) || DictionaryUI[key] == null || DictionaryUI[key].Count <= 0) return null;
        foreach (var ui in DictionaryUI[key])
        {
            if (ui == null || !ui.gameObject.activeInHierarchy) continue;
            return (T)ui;
        }

        return null;
    }

    public IEnumerator<float> LoadAndShowPrefab<T>(string key, Transform parent = null, IUIData data = null) where T : BaseUI
    {
        BaseUI baseUI = null;
        var uiRef = GetUIReference(key);
        if(uiRef == null || uiRef.prefab == null) yield break;

        if (DictionaryUI.ContainsKey(key) && DictionaryUI[key] != null && DictionaryUI[key].Count > 0)
        {
            if (uiRef.isSingle)
            {
                baseUI = DictionaryUI[key][0];
            }
            else
            {
                foreach (var ui in DictionaryUI[key])
                {
                    if (ui != null && !ui.gameObject.activeInHierarchy)
                    {
                        baseUI = ui;
                        break;
                    }
                }
            }

            if (baseUI != null)
            {
                baseUI.SetData(data);
            }
        }

        if (!baseUI)
        {
            yield return Timing.WaitForOneFrame;
            baseUI = InstantiatePrefab(key, uiRef.prefab, parent);
            if(!baseUI) yield break;
            if(!baseUI.IsInit()) baseUI.Initialize();

            while (!baseUI.IsInit())
            {
                yield return Timing.WaitForOneFrame;
            }
            
            baseUI.SetData(data);
            yield return Timing.WaitForOneFrame;
        }
        
        baseUI.ShowUI(data);
    }
    
    
    public void DestroyUIOnParent(Transform parent)
    {
        int childCount = parent.childCount;
        Transform child;
        BaseUI baseUI;

        for (int i = childCount - 1; i >= 0; i--)
        {
            child = parent.GetChild(i);
            baseUI = child.GetComponent<BaseUI>();
            
            if(baseUI == null) continue;
            Destroy(child.gameObject);
        }
    }

    private BaseUI InstantiatePrefab(string key, GameObject prefab, Transform parent)
    {
        try
        {
            var obj = Instantiate(prefab, parent);
            var baseUI = obj.GetComponent<BaseUI>();
            if (baseUI == null) baseUI = obj.AddComponent<BaseUI>();
            if (!DictionaryUI.ContainsKey(key)) DictionaryUI[key] = new List<BaseUI>() { baseUI };
            else DictionaryUI[key].Add(baseUI);
            return baseUI;
        }
        catch (Exception e)
        {
            Debug.LogError($"[UI] InstantiatePrefab [{key}] is exception = {e}");
        }

        return null;
    }

    private UIReference GetUIReference(string key)
    {
        foreach (var uiRef in _uiScriptable.uiReferences)
        {
            if(!key.Equals(uiRef.key)) continue;
            return uiRef;
        }

        return null;
    }
    
    public bool IsPointerOverUIElement()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.layer != 5)
            {
                raycastResults.RemoveAt(i);
                i--;
            }
        }

        return raycastResults.Count > 0;
    }
}