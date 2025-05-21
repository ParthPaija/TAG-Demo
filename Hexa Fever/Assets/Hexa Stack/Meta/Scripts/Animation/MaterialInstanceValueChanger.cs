using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;


public class MaterialInstanceValueChanger : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public Image image;
    public string variableName;
    public string colorPropertyName = "_Color";
    public Material material;
    [Range(-1f, 1f)]
    public float fill;
    public Color color = Color.white;
    #endregion

    #region PRIVATE_VARIABLES
    private int variablePropertyID;
    private int colorPropertyID;
    private float previousValue;
    private Color previousColor;
    #endregion

    #region UNITY_CALLBACKS
    private void OnEnable()
    {
        variablePropertyID = Shader.PropertyToID(variableName);
        colorPropertyID = Shader.PropertyToID(colorPropertyName);
        previousValue = fill;
        previousColor = color;
        OnSetMaterialData();
    }
    private void Update()
    {

        if (previousValue != fill)
        {
            SetMaterialFill();
            previousValue = fill;
        }

        if (previousColor != color)
        {
            SetMaterialColor();
            previousColor = color;
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void OnSetMaterialData()
    {

        image.material = new Material(image.material); // Create a new instance based on the provided material

        material = image.material; // Ensure the material variable is referencing the current material
        SetMaterialFill();
        SetMaterialColor();
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
    private void SetMaterialColor()
    {
        material.SetColor(colorPropertyID, color);
    }
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region CO-ROUTINES
    #endregion
}
#if UNITY_EDITOR
[CustomEditor(typeof(MaterialInstanceValueChanger))]
public class MaterialInstanceValueChangerEditor : Editor
{
    SerializedProperty fillProperty;
    float minRange; // Default min range
    float maxRange; // Default max range
    private const string MinRangeKey = "MaterialInstanceValueChanger_MinRange";
    private const string MaxRangeKey = "MaterialInstanceValueChanger_MaxRange";

    private void OnEnable()
    {
        fillProperty = serializedObject.FindProperty("fill");
        // Load the min and max range values from EditorPrefs
        minRange = EditorPrefs.GetFloat(MinRangeKey, -1f); // Default to -1f if not set
        maxRange = EditorPrefs.GetFloat(MaxRangeKey, 1f);  // Default to 1f if not set
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector options
        DrawDefaultInspector();

        // Add fields to set min and max range in the editor
        minRange = EditorGUILayout.FloatField("Min Range", minRange);
        maxRange = EditorGUILayout.FloatField("Max Range", maxRange);

        // Ensure max is always greater than min
        maxRange = Mathf.Max(minRange, maxRange);

        // Slider for fill property
        EditorGUILayout.Slider(fillProperty, minRange, maxRange, new GUIContent("Fill"));

        // Save the min and max range values to EditorPrefs
        if (GUI.changed)
        {
            EditorPrefs.SetFloat(MinRangeKey, minRange);
            EditorPrefs.SetFloat(MaxRangeKey, maxRange);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif