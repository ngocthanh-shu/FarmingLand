using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UIReference", menuName = "ScriptableObject/UIReference")]
public class UIScriptableObject : GeneralScriptableObject
{
    [SerializeField] public List<UIReference> uiReferences;
}

[Serializable]
public class UIReference
{
    public string key;
    public GameObject prefab;
    public bool isSingle;
}
