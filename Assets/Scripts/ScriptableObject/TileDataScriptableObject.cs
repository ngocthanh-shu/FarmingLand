using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObject/TileData")]
public class TileDataScriptableObject : GeneralScriptableObject
{
    [SerializeField] public Tile identifyInteractableTile;
    [SerializeField] public RuleTile dirtTile;
    [SerializeField] public TileObj buildingGridTileObj;
    
    [SerializeField] public LayerMask interactableLayer;
}
