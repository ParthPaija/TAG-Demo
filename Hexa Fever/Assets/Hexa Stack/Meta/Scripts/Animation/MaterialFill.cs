using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFill : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public SpriteRenderer image;
    public string variableName;
    public Material material;
    [Range(-0.1f, 1)]
    public float fill;
    #endregion

    #region PRIVATE_VARIABLES
    private int variablePropertyID;
    #endregion

    #region UNITY_CALLBACKS
    private void OnEnable()
    {
        OnSetMaterialData();
    }
    private void Update()
    {
        SetMaterialFill();
    }
    #endregion

    #region PUBLIC_METHODS
    public void OnSetMaterialData()
    {
        material = image.material;
        variablePropertyID = Shader.PropertyToID(variableName);
    }
    [ContextMenu("SetProperty")]
    public void SetProperty()
    {
        image = GetComponent<SpriteRenderer>();
        material = image.material;  
    }
    [ContextMenu("SetMaterialFillValue")]
    public void SetMaterialFillValue()
    {
        SetMaterialFill();
    }
    #endregion

    #region PRIVATE_METHODS
    private void SetMaterialFill()
    {
        material.SetFloat(variablePropertyID, fill);
    }
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region CO-ROUTINES
    #endregion
}
