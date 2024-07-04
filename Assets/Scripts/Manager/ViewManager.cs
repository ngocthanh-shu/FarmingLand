using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ViewManager : Singleton<ViewManager>
{
    [field: SerializeField] private List<ViewDictionary> viewDictionaries;

    public void Initialize()
    {
        
    }

    public IView GetView(ViewType type)
    {
        foreach (var view in viewDictionaries)
        {
            if (view.type == type)
            {
                return view.view;
            }
        }

        return null;
    }
}

[Serializable]
public class ViewDictionary
{
    public GeneralView view;
    public ViewType type;
}

public enum ViewType
{
    PlayerView,
}
