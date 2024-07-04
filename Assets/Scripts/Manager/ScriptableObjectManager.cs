using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScriptableObjectManager : Singleton<ScriptableObjectManager>
{
    public List<ScriptableDictionary> scriptableDictionaries;

    public void Initialize()
    {
        
    }

    public GeneralScriptableObject GetScriptableObject(ScriptableType type)
    {
        foreach (var scriptable in scriptableDictionaries)
        {
            if (scriptable.type == type)
            {
                return scriptable.generalScriptableObject;
            }
        }

        return null;
    }
}

[Serializable]
public class ScriptableDictionary
{
    [field: SerializeField]
    public GeneralScriptableObject generalScriptableObject;
    
    [field: SerializeField]
    public ScriptableType type;
}

public enum ScriptableType
{
    PlayerData,
    UIReference,
    TileData,
}
