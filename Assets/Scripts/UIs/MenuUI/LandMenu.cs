using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LandMenu : MonoBehaviour
{
    [SerializeField] private Button size2X2Btn;
    [SerializeField] private Button size3X3Btn;
    #region Initialize

    public void Initialize()
    {
        InitializeAction();
    }

    private void InitializeAction()
    {
        size2X2Btn.onClick.AddListener(ChooseSize2X2Landing);
        size3X3Btn.onClick.AddListener(ChooseSize3X3Landing);
    }
    
    #endregion

    #region Private Methods

    private void ChooseSize2X2Landing()
    {
        TileManager.Instance.ChooseLandSize(LandSize.Size2X2);
    }
    
    private void ChooseSize3X3Landing()
    {
        TileManager.Instance.ChooseLandSize(LandSize.Size3X3);
    }

    #endregion
    
}
