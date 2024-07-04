using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TileObj : MonoBehaviour
{
    [SerializeField] private Color baseColor;
    [SerializeField] private Color offsetColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject highlight;

    private Action<Vector2> _onGetPos;
    private Action<TileGridStatus> _onPlaceTileAtPos;
    private Action _exitByMouse;

    private SpriteRenderer _highlightRenderer;

    private TileGridStatus _status;
 
    public void Init(bool isOffset, Action<Vector2> getPosFunc, Action<TileGridStatus> placeTileAtPos, Action exitTile) {
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
        _onGetPos += getPosFunc;
        _onPlaceTileAtPos += placeTileAtPos;
        _exitByMouse += exitTile;

        _status = TileGridStatus.Available;
        _highlightRenderer = highlight.GetComponent<SpriteRenderer>();
    }
 
    void OnMouseEnter() {
        highlight.SetActive(true);
        _onGetPos?.Invoke(transform.position);
    }

    private void OnMouseDown()
    {
        _onPlaceTileAtPos?.Invoke(_status);
    }

    public void TurnHighlight(bool isActive, TileGridStatus status)
    {
        highlight.SetActive(isActive);
        _highlightRenderer.color = GetColorStatus(status);
        _status = status;
    }

    public Vector3Int GetPosition()
    {
        return new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
    }

    private Color GetColorStatus(TileGridStatus status)
    {
        switch (status)
        {
            case TileGridStatus.Available:
                return new Color(255, 255, 255, 70);
                break;
            case TileGridStatus.Unavailable:
                return new Color(255, 0, 0, 70);
                break;
            default:
                return Color.white;
        }
    }

    void OnMouseExit()
    {
        _exitByMouse?.Invoke();
    }
}

public enum TileGridStatus
{
    Available,
    Unavailable,
}
