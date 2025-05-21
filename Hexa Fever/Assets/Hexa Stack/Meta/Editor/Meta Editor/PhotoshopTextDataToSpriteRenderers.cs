//Chintan Vadgama Was Here

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tag.HexaStack;
using UnityEditor;
using UnityEngine;

public class PhotoshopTextDataToSpriteRenderers : EditorWindow
{
    #region PRIVATE_VARIABLES
    private UnityEngine.Object textFile;
    private string directoryPath = "";
    private char dataSeparator = '#';
    private List<LayerData> layerData = new List<LayerData>();
    private List<LayerData> parentLayerData = new List<LayerData>();
    private List<LayerData> artLayerData = new List<LayerData>();
    private LayerData currentLayerParent;
    private LayerData motherLayer;
    private int orderInLayer = 0;
    private float spacing = 1.08f;
    private bool editMode = false;
    #endregion

    #region PUBLIC_VARIABLES
    #endregion

    #region UNITY_CALLBACKS
    [MenuItem(Constant.GAME_NAME +"/PS Text To Sprite Renderer")]
    public static void ShowWindow()
    {
        GetWindow<PhotoshopTextDataToSpriteRenderers>("PS Text To Sprite Renderer");
    }

    private void UpdatePositions()
    {
        foreach (LayerData ld in artLayerData)
        {
            ld.linkedObject.transform.position = ConvertPixelPositionToWorldPositon(ld.position);
        }
    }

    private void OnGUI()
    {
        DrawGUI();
        UpdatePosition();
    }

    private void ScanAndSetEachParentToCenterOfChild()
    {
        foreach (LayerData parentData in parentLayerData)
        {
            SetParentToCenterLocation(parentData.linkedObject.transform);
        }
        motherLayer.linkedObject.transform.position = Vector3.zero;
        //foreach (GameObject obj in Selection.gameObjects)
        //{
        //    SetParentToCenterLocation(obj.transform);
        //}
    }

    private void SetParentToCenterLocation(Transform objectToCenter)
    {
        Transform[] childs = GetChidTransforms(objectToCenter);
        if (childs.Length > 0)
        {
            Bounds bound = new Bounds(childs[0].position, Vector3.zero);
            for (int i = 0; i < childs.Length; i++)
            {
                bound.Encapsulate(childs[i].position);
                childs[i].SetParent(null);
            }
            objectToCenter.position = bound.center;
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].parent = objectToCenter.transform;
            }
        }
    }

    private Transform[] GetChidTransforms(Transform parentTransform)
    {
        Transform[] childs = new Transform[parentTransform.childCount];
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            childs[i] = parentTransform.GetChild(i);
        }
        return childs;
    }

    private void UpdatePosition()
    {
        if (editMode)
        {
            UpdatePositions();
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private void DrawGUI()
    {
        textFile = EditorGUILayout.ObjectField("Text File:", textFile, typeof(UnityEngine.Object), true);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        if (textFile != null)
        {
            DrawButton("Set Path", SetPNGAssetPath);
            DrawButton("Read", PrintTextOutput);
        }
        if (editMode)
        {
            GUI.backgroundColor = Color.green;
            DrawButton("Edit Is On", OnEdit);
        }
        else
        {
            GUI.backgroundColor = Color.red;
            DrawButton("Edit Is Off", OnEdit);
        }

        GUI.backgroundColor = Color.white;
        DrawButton("Set Parent To Center", ScanAndSetEachParentToCenterOfChild);

        if(GUILayout.Button("Dpooo"))
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }

    private void OnEdit()
    {
        editMode = !editMode;
    }

    private void SetPNGAssetPath()
    {
        directoryPath = EditorUtility.OpenFolderPanel("Select Assets Folder with PNGs...", Application.dataPath, "");
        if (!directoryPath.StartsWith(Application.dataPath))
        {
            directoryPath = "";
        }
        else
        {
            directoryPath = GetRelativeFilePath(directoryPath);
        }
    }

    string GetRelativeFilePath(string fullPath)
    {
        if (fullPath == string.Empty) { return ""; }
        string relativeFilePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
        return relativeFilePath;
    }

    private void InitMotherLayer()
    {
        parentLayerData.Clear();
        artLayerData.Clear();
        motherLayer = new LayerData();
        GameObject mainParent = new GameObject(textFile.name);
        motherLayer.name = textFile.name;
        motherLayer.id = int.Parse(GetParentID(textFile));
        motherLayer.type = LayerType.Group;
        motherLayer.linkedObject = mainParent;
        currentLayerParent = motherLayer;
        parentLayerData.Add(motherLayer);
    }
    private void DrawButton(string buttonName, Action actionToPerform)
    {
        if (GUILayout.Button(buttonName))
        {
            actionToPerform.Invoke();
        }
    }

    private void DrawButton(string buttonName, Action actionToPerform, Color buttonColor)
    {
        if (GUILayout.Button(buttonName))
        {
            actionToPerform.Invoke();
        }
    }

    private void PrintTextOutput()
    {
        System.DateTime datevalue2 = System.DateTime.Now;
        Resolution currentRes = Screen.currentResolution;
        Screen.SetResolution(1920, 1080, false);
        InitData();
        InitMotherLayer();
        ReadDataLineWise(textFile);
        Screen.SetResolution(currentRes.width, currentRes.height, false);
        EditorUtility.DisplayDialog("Done", "Time Took To Complete The Operation : " + (System.DateTime.Now - datevalue2).TotalMilliseconds + " ms", "Nice!");
    }

    private void InitData()
    {
        orderInLayer = 0;
    }

    private void PrintLayerData(LayerData layerData)
    {
        Debug.Log("ID : " + layerData.id);
        Debug.Log("Name : " + layerData.name);
        Debug.Log("Parent : " + layerData.parent);
        Debug.Log("Parent ID : " + layerData.parentID);
        Debug.Log("Position : " + layerData.position);
        Debug.Log("Type : " + layerData.type);
    }

    private Vector3 ConvertPixelPositionToWorldPositon(Vector2 positionToConvert)
    {
        Vector3 convertedVector = new Vector3(positionToConvert.x, positionToConvert.y, 0);
        return Camera.main.ScreenToWorldPoint(convertedVector) * spacing;
    }

    private LayerData GetLayerDataFromLineData(string[] currentLineData)
    {
        LayerData layerDataToAssign = new LayerData();
        layerDataToAssign.id = int.Parse(currentLineData[0]);
        layerDataToAssign.position = (GetLayerPosition(currentLineData[1]));
        layerDataToAssign.name = currentLineData[2];
        layerDataToAssign.parentID = int.Parse(currentLineData[3]);
        layerDataToAssign.type = GetLayerType(currentLineData[4]);
        return layerDataToAssign;
    }

    private Vector2 GetLayerPosition(string positionData)
    {
        positionData = positionData.Replace(" px", "");
        string[] bounds = positionData.Split(',');
        float x = float.Parse(bounds[0]);
        float y = float.Parse(bounds[1]);
        float width = float.Parse(bounds[2]) - x;
        float height = float.Parse(bounds[3]) - y;
        float midx = x + (width / 2);
        float midy = y + (height / 2);
        return new Vector2(midx, -midy);
    }

    private LayerType GetLayerType(string typeData)
    {
        if (typeData == "LayerSet")
            return LayerType.Group;
        else
            return LayerType.Layer;
    }
    private void ReadDataLineWise(UnityEngine.Object fileToRead)
    {
        layerData.Clear();
        string path = AssetDatabase.GetAssetPath(fileToRead);
        StreamReader reader = new StreamReader(path);
        string line = "";
        while ((line = reader.ReadLine()) != null)
        {
            if (!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)))
            {
                line = line.Substring(0, line.Length - 1);
                LayerData currentLayerData = GetLayerDataFromLineData(line.Split(dataSeparator));
                layerData.Add(currentLayerData);
                GenerateObjectFromLayerData(currentLayerData);
            }
        }
        reader.Close();
        reader.DiscardBufferedData();
    }

    private string GetParentID(UnityEngine.Object fileToRead)
    {
        string path = AssetDatabase.GetAssetPath(fileToRead);
        StreamReader reader = new StreamReader(path);
        string line = "";
        line = reader.ReadLine();
        return line.Split(dataSeparator)[3];
    }

    private LayerData GetLayerDataFromID(int idToCompare)
    {
        for (int i = 0; i < parentLayerData.Count; i++)
        {
            if (parentLayerData[i].id == idToCompare)
            {
                return parentLayerData[i];
            }
        }
        return null;
    }

    private void GenerateObjectFromLayerData(LayerData dataToGenerateFrom)
    {
        if (currentLayerParent.id != dataToGenerateFrom.parentID)
            currentLayerParent = GetLayerDataFromID(dataToGenerateFrom.parentID);

        GameObject generatedObject = new GameObject(dataToGenerateFrom.name);

        dataToGenerateFrom.parent = currentLayerParent;
        generatedObject.transform.position = ConvertPixelPositionToWorldPositon(dataToGenerateFrom.position);
        dataToGenerateFrom.linkedObject = generatedObject;
        generatedObject.transform.parent = currentLayerParent.linkedObject.transform;
        if (dataToGenerateFrom.type == LayerType.Group)
        {
            generatedObject.name += "[G]";
            currentLayerParent = dataToGenerateFrom;
            parentLayerData.Add(dataToGenerateFrom);
        }
        else
        {
            generatedObject.name += "[L]";
            SpriteRenderer spriteRenderer = generatedObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(directoryPath + "/" + dataToGenerateFrom.name + ".png");
            spriteRenderer.sortingOrder = 500 - orderInLayer;
            artLayerData.Add(dataToGenerateFrom);
            orderInLayer++;
        }
    }
    #endregion

    #region PUBLIC_METHODS
    #endregion

    #region ENUMERATORS
    #endregion
}

public class LayerData
{
    public int id;
    public Vector2 position;
    public string name;
    public int parentID;
    public LayerData parent;
    public LayerType type;
    public GameObject linkedObject;
}

public enum LayerType
{
    Layer,
    Group
}