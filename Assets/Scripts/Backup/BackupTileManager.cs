using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackupTileManager : MonoBehaviour
{
    
    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tilemap plantMap;
    [SerializeField] private Tilemap backgroundMap;
    [SerializeField] private Tilemap checkMap;

    private LayerMask _interactableLayer;
    
    private Tile _hiddenInteractableTile;
    private Tile _plantTile;
    private RuleTile _dirtTile;

    private TileObj _buildingGridTile;
    private Vector3Int _hoveredTilePos;

    private Vector3Int[,] _dirtMatrices;
    private List<Vector3Int> _angleList;
    private List<TileObj> _gridTile;
    private GameObject _gridParent;

    private ActionStatus _status;
    
    public void Initialize()
    {
        InitializeData();
        //_dirtMatrices = new Vector3Int[3,3];
        //_angleList = new List<Vector3Int>();
        _gridTile = new List<TileObj>();
        //InitializeFarmArea();
        GenerateGrid();
        
    }

    private void InitializeData()
    {
        TileDataScriptableObject tileData =
            (TileDataScriptableObject)ScriptableObjectManager.Instance.GetScriptableObject(ScriptableType.TileData);
        
        if(!tileData) return;

        _interactableLayer = tileData.interactableLayer;
        
        _hiddenInteractableTile = tileData.identifyInteractableTile;
        _buildingGridTile = tileData.buildingGridTileObj;
        _dirtTile = tileData.dirtTile;
        
        _gridParent = new GameObject();
        _gridParent.transform.SetParent(transform);
        _gridParent.SetActive(false);

        _status = ActionStatus.Interact;
    }

    private void InitializeFarmArea()
    {
        TileBase tmp;
        
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            tmp = interactableMap.GetTile(position);
            if (tmp != null && tmp.name == "DirtAvailable")
            {
                interactableMap.SetTile(position, _hiddenInteractableTile);
                checkMap.SetTile(position, _hiddenInteractableTile);
            }

            tmp = interactableMap.GetTile(GetPositonByType(position, DirectionType.Up));
            if (tmp == null) continue;
            tmp = interactableMap.GetTile(GetPositonByType(position, DirectionType.Down));
            if (tmp == null) continue;
            tmp = interactableMap.GetTile(GetPositonByType(position, DirectionType.Left));
            if (tmp == null) continue;
            tmp = interactableMap.GetTile(GetPositonByType(position, DirectionType.Right));
            if (tmp == null) continue;
            
            plantMap.SetTile(position, _hiddenInteractableTile);
        }
    }

    private bool IsInteractable(Tilemap map, Vector3Int position)
    {
        TileBase tile = map.GetTile(position);

        if (tile != null && tile.name == "Interactable") return true;
        return false;
    }

    private void SetInteracted(Vector3Int position)
    {
        interactableMap.SetTile(position, _dirtTile);
    }

    private bool IsPlant(Vector3Int position)
    {
        TileBase tile = plantMap.GetTile(position);
        if (tile != null && tile.name == "Interactable") return true;
        return false;
    }

    private void SetPlant(Vector3Int position)
    {
        plantMap.SetTile(position, _plantTile);
    }

    public Vector3Int CalculatePlantPosition(Vector2 position)
    {
        var vector3Int = interactableMap.WorldToCell(position);
        vector3Int.x = position.x <= vector3Int.x + 0.5f ? vector3Int.x : vector3Int.x + 1;
        return vector3Int;
    }

    public void SetFarmingDirt(Vector3Int position, DirectionType type)
    {
        switch (type)
        {
            case DirectionType.Center:
                if (IsInteractable(interactableMap, position) || CheckDirt(position))
                {
                    if(IsPlant(position)) SetPlant(position);
                    SetInteracted(position);
                    _dirtMatrices[1, 1] = position;
                    SetFarmingDirt(GetPositonByType(position, DirectionType.Up), DirectionType.Up);
                    SetFarmingDirt(GetPositonByType(position, DirectionType.Down), DirectionType.Down);
                    SetFarmingDirt(GetPositonByType(position, DirectionType.Left), DirectionType.Left);
                    SetFarmingDirt(GetPositonByType(position, DirectionType.Right), DirectionType.Right);
                    SetFarmingDirt(position, DirectionType.Default);
                }
                
                break;
            case DirectionType.Up:
                if (IsInteractable(interactableMap, position))
                {
                    SetInteracted(position);
                    _dirtMatrices[1, 0] = position;
                    _angleList.Add(position);
                }
                else
                {
                    TileBase tile = interactableMap.GetTile(position);
                    if (tile == null)
                    {
                        SetFarmingDirt(GetPositonByType(position, DirectionType.Down, 2), DirectionType.Up);
                    }
                    else
                    {
                        _dirtMatrices[1, 0] = position;
                    }
                }
                break;
            case DirectionType.Down:
                if (IsInteractable(interactableMap, position))
                {
                    SetInteracted(position);
                    _dirtMatrices[1, 2] = position;
                    _angleList.Add(position);
                }
                else
                {
                    TileBase tile = interactableMap.GetTile(position);
                    if (tile == null)
                    {
                        SetFarmingDirt(GetPositonByType(position, DirectionType.Up, 2), DirectionType.Down);
                    }
                    else
                    {
                        _dirtMatrices[1, 2] = position;
                    }
                }
                break;
            case DirectionType.Left:
                if (IsInteractable(interactableMap, position))
                {
                    SetInteracted(position);
                    _dirtMatrices[0, 1] = position;
                    _angleList.Add(position);
                }
                else
                {
                    TileBase tile = interactableMap.GetTile(position);
                    if (tile == null)
                    {
                        SetFarmingDirt(GetPositonByType(position, DirectionType.Right, 2), DirectionType.Left);
                    }
                    else
                    {
                        _dirtMatrices[0, 1] = position;
                    }
                }
                break;
            case DirectionType.Right:
                if (IsInteractable(interactableMap, position))
                {
                    SetInteracted(position);
                    _dirtMatrices[2, 1] = position;
                    _angleList.Add(position);
                }
                else
                {
                    TileBase tile = interactableMap.GetTile(position);
                    if (tile == null)
                    {
                        SetFarmingDirt(GetPositonByType(position,DirectionType.Left, 2), DirectionType.Right);
                    }
                    else
                    {
                        _dirtMatrices[2, 1] = position;
                    }
                }
                break;
            case DirectionType.Default:
                int leftX = _dirtMatrices[0, 1].x;
                int upY = _dirtMatrices[1, 0].y;
                int rightX = _dirtMatrices[2, 1].x;
                int downY = _dirtMatrices[1, 2].y;
                
                Vector3Int pos = new Vector3Int(leftX, upY);
                _dirtMatrices[0, 0] = pos;
                SetAvailableInteracted(pos);
                _angleList.Add(pos);

                pos = new Vector3Int(leftX, downY);
                _dirtMatrices[0, 2] = pos;
                SetAvailableInteracted(pos);
                _angleList.Add(pos);
                
                pos = new Vector3Int(rightX,upY);
                _dirtMatrices[2, 0] = pos;
                SetAvailableInteracted(pos);
                _angleList.Add(pos);
                
                pos = new Vector3Int(rightX,downY);
                _dirtMatrices[2, 2] = pos;
                SetAvailableInteracted(pos);
                _angleList.Add(pos);
                break;
        }
    }

    private bool CheckDirt(Vector3Int position)
    {
        foreach (var pos in _angleList)
        {
            if (pos == position)
            {
                _angleList.Remove(pos);
                return true;
            }
        }

        return false;
    }

    private void SetAvailableInteracted(Vector3Int newPos)
    {
        if(IsInteractable(interactableMap, newPos)) SetInteracted(newPos);
    }

    private Vector3Int GetPositonByType(Vector3Int position, DirectionType type, int bonus = 0)
    {
        switch (type)
        {
            case DirectionType.Up:
                return new Vector3Int(position.x, position.y + 1 + bonus);
            case DirectionType.Down:
                return new Vector3Int(position.x, position.y - 1 - bonus);
            case DirectionType.Left:
                return new Vector3Int(position.x - 1 - bonus, position.y);
            case DirectionType.Right:
                return new Vector3Int(position.x + 1 + bonus, position.y);
            default:
                return new Vector3Int();
        }
    }

    private void GenerateGrid()
    {
        foreach (var position in backgroundMap.cellBounds.allPositionsWithin)
        {
            if(!CheckPositionToPlaceGrid(position)) continue; 
            var spawnTile = Instantiate(_buildingGridTile, position, quaternion.identity, _gridParent.transform);
            spawnTile.name = $"Tile {position.x} {position.y}";
            var isOffset = (position.x % 2 == 0 && position.y % 2 != 0) || (position.x % 2 != 0 && position.y % 2 == 0);
            //spawnTile.Init(isOffset, SetHoveredTile, SetTileAtPos);
            _gridTile.Add(spawnTile);
        }
    }

    private bool CheckPositionToPlaceGrid(Vector3Int position)
    {
        TileBase tileBase = backgroundMap.GetTile(GetPositonByType(position, DirectionType.Up));
        if(!tileBase) return false;
        tileBase = backgroundMap.GetTile(GetPositonByType(position, DirectionType.Down));
        if(!tileBase) return false;
        tileBase = backgroundMap.GetTile(GetPositonByType(position, DirectionType.Left));
        if(!tileBase) return false;
        tileBase = backgroundMap.GetTile(GetPositonByType(position, DirectionType.Right));
        if(!tileBase) return false;

        Vector2 checkBoxSize = new Vector2(0, 0);
        Collider2D objectExists =
            Physics2D.OverlapBox(new Vector2(position.x, position.y), checkBoxSize, _interactableLayer);
        if (objectExists) return false;

        return true;
    }

    public void SetActiveGrid(bool isActive)
    {
        _gridParent.SetActive(isActive);
        
    }

    public void SetActionStatus(ActionStatus status)
    {
        _status = status;
    }

    private void SetHoveredTile(Vector2 pos)
    {
        switch (_status)
        {
            case ActionStatus.Building:
                if (IsRuleTileExist(checkMap, _hoveredTilePos, _dirtTile))
                {
                    checkMap.SetTile(_hoveredTilePos, _hiddenInteractableTile);
                }
                _hoveredTilePos = new Vector3Int((int)pos.x, (int)pos.y, 0);
                if (!IsRuleTileExist(checkMap, _hoveredTilePos, _dirtTile))
                {
                    checkMap.SetTile(_hoveredTilePos, _dirtTile);
                }
                break;
        }
    }

    private void SetTileAtPos(Vector2 pos)
    {
        Vector3Int position = new Vector3Int((int)pos.x, (int)pos.y, 0);
        TileBase tileBase = interactableMap.GetTile(position);
        if (!tileBase || tileBase.name == _hiddenInteractableTile.name)
        {
            if(UIManager.Instance.IsPointerOverUIElement()) return;
            interactableMap.SetTile(position, _dirtTile);
        }
    }

    public void RefreshReviewTileMap()
    {
        checkMap.SetTile(_hoveredTilePos, _hiddenInteractableTile);
    }
    
    private bool IsRuleTileExist(Tilemap map, Vector3Int pos, RuleTile tile)
    {
        TileBase baseTile = map.GetTile(pos);
        if (baseTile != null && baseTile.name == tile.name) return true;
        return false;
    }

    public Vector3Int GetHoveredTile()
    {
        return _hoveredTilePos;
    }
}
