using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class TileManager : Singleton<TileManager>
{
    #region Serializable Attributes

    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tilemap backgroundMap;
    [SerializeField] private Tilemap checkMap;

    #endregion

    #region Attributes

    private TileManagerData _data;
    
    private Vector3Int _hoveredTilePos;
    private List<Vector3Int> _hoveredTilePositions;
    private List<Vector3Int> _listOutLineTmp;

    private List<LandPosition> _landPositions;
    
    private List<TileObj> _gridTile;
    
    private GameObject _gridParent;

    private ActionStatus _status;

    private LandSize _currentLandSize;

    #endregion
    
    #region Initialize Methods

    public void Initialize()
    {
        InitializeData();
        
        GenerateGrid();
    }

    private void InitializeData()
    {
        TileDataScriptableObject tileData =
            (TileDataScriptableObject)ScriptableObjectManager.Instance.GetScriptableObject(ScriptableType.TileData);
        
        if(!tileData) return;
        
        _data = new TileManagerData(tileData);
        
        _gridTile = new List<TileObj>();
        _hoveredTilePositions = new List<Vector3Int>();
        _listOutLineTmp = new List<Vector3Int>();
        
        _landPositions = new List<LandPosition>();
        
        _gridParent = new GameObject();
        _gridParent.transform.SetParent(transform);
        _gridParent.SetActive(false);

        _status = ActionStatus.Interact;
        _currentLandSize = LandSize.Size2X2;
    }

    #endregion

    #region Local Methods

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
            interactableMap.SetTile(position, _data.HiddenInteractableTile);
            checkMap.SetTile(position, _data.HiddenInteractableTile);
            var spawnTile = Instantiate(_data.BuildingGridTile, position, quaternion.identity, _gridParent.transform);
            spawnTile.name = $"Tile {position.x} {position.y}";
            var isOffset = (position.x % 2 == 0 && position.y % 2 != 0) || (position.x % 2 != 0 && position.y % 2 == 0);
            spawnTile.Init(isOffset, SetHoveredTile, SetTileAtPos, TurnOffAllHighlight);
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
            Physics2D.OverlapBox(new Vector2(position.x, position.y), checkBoxSize, _data.InteractableLayer);
        if (objectExists) return false;

        return true;
    }
    
    private void SetHoveredTile(Vector2 pos)
    {
        _listOutLineTmp.Clear();
        switch (_status)
        {
            case ActionStatus.Building:
                if (IsRuleTileExist(checkMap, _hoveredTilePositions, _data.DirtTile))
                {
                    SetTileForPosition(checkMap, _hoveredTilePositions, _data.HiddenInteractableTile);
                }
                
                _hoveredTilePos = new Vector3Int((int)pos.x, (int)pos.y, 0);
                _hoveredTilePositions = GetPlaceTiles(_hoveredTilePos, true);
                
                if (!IsRuleTileExist(checkMap, _hoveredTilePositions, _data.DirtTile))
                {
                    SetTileForPosition(checkMap, _hoveredTilePositions, _data.DirtTile);
                    if (IsAvailableTile(_hoveredTilePositions) || IsHoveredPositionOutline())
                    {
                        SetHighLightForTiles(_hoveredTilePos, TileGridStatus.Available);
                    }
                    else
                    {
                        SetHighLightForTiles(_hoveredTilePos, TileGridStatus.Unavailable);
                    }
                }
                break;
        }
    }

    private void SetHighLightForTiles(Vector3Int pos, TileGridStatus status)
    {
        
        List<Vector3Int> listPosition = GetPlaceTiles(pos, false);
        foreach (var position in listPosition)
        {
            foreach (var grid in _gridTile)
            {
                var tilePos3Int = grid.GetPosition();
                if (tilePos3Int == position)
                {
                    grid.TurnHighlight(true, status);
                }
            }
        }
    }

    private bool IsAvailableTile(List<Vector3Int> listPosition)
    {
        foreach (var position in listPosition)
        {
            TileBase tile = interactableMap.GetTile(position);
            
            if (tile == null || tile.name != _data.HiddenInteractableTile.name)
            {
                if (!CheckExistAtOutline(position)) return false;
            }
        }

        return true;
    }

    private bool IsHoveredPositionOutline()
    {
        List<Vector3Int> noneOutlinePositions = GetPlaceTiles(_hoveredTilePos, false);

        if (!CheckExistAtOutline(_hoveredTilePos)) return false;
        foreach (Vector3Int position in noneOutlinePositions)
        {
            if (CheckExistLand(position)) return false;
        }

        return true;
    }

    private void SetTileAtPos(TileGridStatus status)
    {
        if(status == TileGridStatus.Unavailable) return;
        SetTileForPosition(interactableMap,_hoveredTilePositions, _data.DirtTile);
        RemoveOutline(_listOutLineTmp);
        _landPositions.Add(FilterOutLineAndLand(_hoveredTilePos, _hoveredTilePositions));
        _listOutLineTmp.Clear();
    }
    
    private bool IsRuleTileExist(Tilemap map, List<Vector3Int> listPosition, RuleTile tile)
    {
        foreach (var position in listPosition)
        {
            TileBase baseTile = map.GetTile(position);
            if (baseTile != null && baseTile.name == tile.name) return true;
        }
        return false;
    }

    private void SetTileForPosition(Tilemap map, List<Vector3Int> listPosition, Tile tile)
    {
        foreach (var position in listPosition)
        {
            map.SetTile(position, tile);
        }
    }
    
    private void SetTileForPosition(Tilemap map, List<Vector3Int> listPosition, RuleTile tile)
    {
        foreach (var position in listPosition)
        {
            map.SetTile(position, tile);
        }
    }

    private List<Vector3Int> GetPlaceTiles(Vector3Int position, bool haveOutLine)
    {
        switch (_currentLandSize)
        {
            case LandSize.Size2X2:
                return CalculateListVector3Int(position, 2, 2, haveOutLine);
            case LandSize.Size3X3:
                return CalculateListVector3Int(position, 3, 3, haveOutLine);
            default:
                return null;
        }
    }

    private List<Vector3Int> CalculateListVector3Int(Vector3Int pos, int sizeX, int sizeY, bool haveOutLine)
    {
        int outLine = haveOutLine ? 1 : 0;
        
        int posXMin = pos.x - outLine;
        int posXMax = pos.x + (sizeX - 1) + outLine;

        int posYMin = pos.y - outLine;
        int posYMax = pos.y + (sizeY - 1) + outLine;

        List<Vector3Int> listPos = new List<Vector3Int>();
        Vector3Int posItem;
        
        for (int x = posXMin; x < posXMax + 1; x++)
        {
            for (int y = posYMin; y < posYMax + 1; y++)
            {
                posItem = new Vector3Int(x, y, 0);
                listPos.Add(posItem);
            }
        }

        return listPos;
    }

    private LandPosition FilterOutLineAndLand(Vector3Int corePosition, List<Vector3Int> listPosition)
    {
        List<Vector3Int> noneOutline = GetPlaceTiles(corePosition, false);
        List<Vector3Int> outline = new List<Vector3Int>();

        foreach (Vector3Int position in listPosition)
        {
            if(!noneOutline.Contains(position)) outline.Add(position);
        }

        return new LandPosition()
        {
            Key = corePosition,
            LandPositions = noneOutline,
            OutlinePositions = outline,
        };
    }

    private bool CheckExistAtOutline(Vector3Int position)
    {
        foreach (var item in _landPositions)
        {
            if (item.OutlinePositions.Contains(position))
            {
                _listOutLineTmp.Add(position);
                return true;
            }
        }

        return false;
    }

    private bool CheckExistLand(Vector3Int position)
    {
        foreach (var item in _landPositions)
        {
            if (item.LandPositions.Contains(position)) return true;
        }

        return false;
    }

    private void RemoveOutline(List<Vector3Int> positions)
    {
        foreach (var item in _landPositions)
        {
            foreach (var position in positions)
            {
                if(!item.OutlinePositions.Contains(position)) continue;
                item.OutlinePositions.Remove(position);
            }
        }
    }

    #endregion

    #region Public Methods

    public void SetActiveGrid(bool isActive)
    {
        _gridParent.SetActive(isActive);
    }

    public void SetActionStatus(ActionStatus status)
    {
        _status = status;
    }
    
    public void RefreshReviewTileMap()
    {
        TurnOffAllHighlight();
        SetTileForPosition(checkMap, _hoveredTilePositions, _data.HiddenInteractableTile);
    }

    public void ChooseLandSize(LandSize size)
    {
        _currentLandSize = size;
    }

    private void TurnOffAllHighlight()
    {
        foreach (var gridTile in _gridTile)
        {
            gridTile.TurnHighlight(false, TileGridStatus.Available);
        }
    }

    #endregion
}

public enum DirectionType
{
    Center,
    Up,
    Down,
    Left,
    Right,
    Default,
}

public enum LandSize
{
    Size2X2,
    Size3X3,
}

public enum ActionStatus
{
    Building,
    Interact,
}

public class TileManagerData
{
    public LayerMask InteractableLayer;
    
    public Tile HiddenInteractableTile;
    public RuleTile DirtTile;

    public TileObj BuildingGridTile;

    private static Random _rnd;

    public TileManagerData(TileDataScriptableObject scriptableObject)
    {
        InteractableLayer = scriptableObject.interactableLayer;
        HiddenInteractableTile = scriptableObject.identifyInteractableTile;
        DirtTile = scriptableObject.dirtTile;
        BuildingGridTile = scriptableObject.buildingGridTileObj;
    }
}

public class LandPosition
{
    public Vector3Int Key;
    public List<Vector3Int> LandPositions;
    public List<Vector3Int> OutlinePositions;
}