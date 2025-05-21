using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tag.HexaStack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CVTools
{
    public class PhotoshopTextDataToUI : EditorWindow
    {
        #region PRIVATE_VARIABLES
        private UnityEngine.Object textFile;
        private string directoryPath = "";
        private char dataSeparator = '#';
        private List<LayerDataUI> layerDataUI = new List<LayerDataUI>();
        private List<LayerDataUI> parentLayerDataUI = new List<LayerDataUI>();
        private List<LayerDataUI> artLayerDataUI = new List<LayerDataUI>();
        private LayerDataUI currentLayerParent;
        private LayerDataUI motherLayer;
        private CanvasScaler canvasScaler;

        private bool deleteDuplicates = false;

        private RectTransform mainRect;

        private UnityEngine.Object folder;
        private Font fonts;

        private string[] tagTypes = new string[] { "(B)", "(T)", "(I)" };

        //DebugMode
        private bool isDebugMode = true;
        #endregion

        #region PUBLIC_VARIABLES
        #endregion

        #region UNITY_CALLBACKS
        [MenuItem(Constant.GAME_NAME + "/PS Text To UI")]
        public static void ShowWindow()
        {
            GetWindow<PhotoshopTextDataToUI>("PS Text To UI");
        }

        private void OnGUI()
        {
            DrawGUI();
            if (isDebugMode)
            {
                DrawDebugModeGUI();
            }
        }

        private void DrawDebugModeGUI()
        {
        }

        private void ReImportSprites()
        {
            String[] spritePaths = Directory.GetFiles(AssetDatabase.GetAssetPath(folder), "*.png");
            float progressIncrement = 1 / spritePaths.Length;
            float totalProgress = 0f;
            foreach (string assetSprite in spritePaths)
            {
                EditorUtility.DisplayProgressBar("Re Importing", "Re Importing Spirtes", totalProgress);
                TextureImporter curTextureAsset = (TextureImporter)TextureImporter.GetAtPath(assetSprite);
                curTextureAsset.textureType = TextureImporterType.Sprite;
                curTextureAsset.isReadable = true;
                curTextureAsset.SaveAndReimport();
                totalProgress += progressIncrement;
            }
            EditorUtility.ClearProgressBar();
        }

        private void RealignUI()
        {
            for (int i = parentLayerDataUI.Count - 1; i > 0; i--)
            {
                SetChildAlignedBounds(parentLayerDataUI[i].linkedObject.GetComponent<RectTransform>());
            }
            SetAndStretchToParentSize(GetRecTransform(motherLayer.linkedObject));
        }

        public void SetAndStretchToParentSize(RectTransform rectToStretch)
        {
            //Setting The AnchorPos
            rectToStretch.anchorMin = Vector2.zero;
            rectToStretch.anchorMax = Vector2.one;
            //Setting The TopBotPos
            rectToStretch.offsetMax = Vector2.zero;
            rectToStretch.offsetMin = Vector2.zero;
        }

        private void SetChildAlignedBounds(RectTransform rectToAlign)
        {
            RectTransform[] chilRecs = GetChidTransforms(rectToAlign);

            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            foreach (RectTransform rect in chilRecs)
            {
                Vector3[] corners = new Vector3[4];
                rect.GetWorldCorners(corners);

                foreach (Vector3 vec in corners)
                {
                    min = Vector3.Min(min, vec);
                    max = Vector3.Max(max, vec);
                }

                rect.SetParent(null);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            rectToAlign.position = bounds.center;
            rectToAlign.sizeDelta = bounds.size;

            foreach (RectTransform rect in chilRecs)
            {
                rect.SetParent(rectToAlign);
            }

        }

        private void MakeMeBound(RectTransform curRec)
        {
            RectTransform[] childss = GetChidTransforms(curRec);
            for (int i = 0; i < childss.Length; i++)
            {

                childss[i].SetParent(null);
            }

            curRec.anchoredPosition = Vector2.zero;

            for (int i = childss.Length - 1; i >= 0; i--)
            {
                childss[i].SetParent(curRec);
            }
        }

        private RectTransform GetRecTransform(GameObject obj)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null)
            {
                rt = obj.AddComponent<RectTransform>();
            }
            return rt;
        }

        private void ScanAndSetEachParentToCenterOfChild()
        {
            GetRecTransform(motherLayer.linkedObject);

            foreach (LayerDataUI lData in layerDataUI)
            {
                if (lData.linkedObject.GetComponent<Transform>() != null)
                {
                    GetRecTransform(lData.linkedObject);
                }
            }

            foreach (LayerDataUI parentData in parentLayerDataUI)
            {
                MakeMeBound(GetRecTransform(parentData.linkedObject));
            }
        }

        private RectTransform[] GetChidTransforms(RectTransform parentTransform)
        {
            RectTransform[] childs = new RectTransform[parentTransform.childCount];
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                childs[i] = parentTransform.GetChild(i).GetComponent<RectTransform>();
            }
            return childs;
        }

        #endregion

        #region PRIVATE_METHODS
        private void DrawGUI()
        {
            isDebugMode = EditorGUILayout.ToggleLeft(" : Debug Mode", isDebugMode);
            AddSpace(1);
            textFile = EditorGUILayout.ObjectField("Text File:", textFile, typeof(UnityEngine.Object), true);
            folder = EditorGUILayout.ObjectField("Folder : ", folder, typeof(UnityEngine.Object), true);
            fonts = (Font)EditorGUILayout.ObjectField("Fonts : ", fonts, typeof(Font), true);
            if (textFile != null)
            {
                AddSpace(3);
                if (folder != null)
                {
                    DrawButton("1. Re-Import Sprites", ReImportSprites);
                    DrawButton("2. Generate UI", GenerateUI);
                }
            }
        }

        private void GenerateUI()
        {
            System.DateTime datevalue2 = System.DateTime.Now;
            ShowConfirmations();
            InitCleanning();
            SetAssetPath();
            PrintTextOutput();
            ScanAndSetEachParentToCenterOfChild();
            RemoveDuplicateSprites();
            RealignUI();
            EditorUtility.DisplayDialog("Done", "Time Took To Complete The Operation : " + (System.DateTime.Now - datevalue2).TotalMilliseconds + " ms", "Nice!");
        }

        private void ShowConfirmations()
        {
            deleteDuplicates = EditorUtility.DisplayDialog("Delete!!!", "Delete Duplicate Sprites From Project???", "Yes", "No");
        }

        public void InitCleanning()
        {
            if (!isDebugMode)
            {
                if (GameObject.Find(textFile.name) != null)
                {
                    DestroyImmediate(GameObject.Find(textFile.name));
                }
            }
        }

        private void SetAssetPath()
        {
            directoryPath = AssetDatabase.GetAssetPath(folder);
        }

        private void AddSpace(int spaceSize)
        {
            for (int i = 0; i < spaceSize; i++)
            {
                EditorGUILayout.Space();
            }
        }

        private void InitMotherLayer()
        {
            canvasScaler = FindObjectOfType<CanvasScaler>();
            parentLayerDataUI.Clear();
            artLayerDataUI.Clear();
            motherLayer = new LayerDataUI();
            GameObject mainParent = new GameObject(textFile.name);
            mainParent.transform.SetParent(canvasScaler.transform);
            motherLayer.name = textFile.name;
            motherLayer.id = int.Parse(GetParentID(textFile));
            motherLayer.type = LayerTypeUI.Group;
            motherLayer.linkedObject = mainParent;
            currentLayerParent = motherLayer;
            parentLayerDataUI.Add(motherLayer);
        }
        private void DrawButton(string buttonName, Action actionToPerform)
        {
            if (GUILayout.Button(buttonName))
            {
                actionToPerform.Invoke();
            }
        }

        private void DrawButton(string buttonName, Action actionToPerform, bool isDebugModeButton)
        {
            if (isDebugMode)
            {
                if (GUILayout.Button(buttonName))
                {
                    actionToPerform.Invoke();
                }
            }
        }

        private void PrintTextOutput()
        {
            InitMotherLayer();
            ReadDataLineWise(textFile);
        }

        private void PrintLayerDataUI(LayerDataUI LayerDataUI)
        {
            Debug.Log("ID : " + LayerDataUI.id);
            Debug.Log("Name : " + LayerDataUI.name);
            Debug.Log("Parent : " + LayerDataUI.parent);
            Debug.Log("Parent ID : " + LayerDataUI.parentID);
            Debug.Log("Position : " + LayerDataUI.position);
            Debug.Log("Type : " + LayerDataUI.type);
        }

        private Vector3 ConvertPixelPositionToWorldPositon(Vector2 positionToConvert)
        {
            Vector3 convertedVector = new Vector3(positionToConvert.x, positionToConvert.y, 0);
            //convertedVector = Camera.main.ScreenToViewportPoint(convertedVector);
            return convertedVector;
        }

        private LayerDataUI GetLayerDataUIFromLineData(string[] currentLineData)
        {
            LayerDataUI LayerDataUIToAssign = new LayerDataUI();
            LayerDataUIToAssign.id = int.Parse(currentLineData[0]);
            LayerDataUIToAssign.position = (GetLayerPosition(currentLineData[1]));
            LayerDataUIToAssign.name = currentLineData[2];
            LayerDataUIToAssign.parentID = int.Parse(currentLineData[3]);
            LayerDataUIToAssign.type = GetLayerTypeUI(currentLineData[4]);
            LayerDataUIToAssign.size = GetLayerSize(currentLineData[1]);
            LayerDataUIToAssign.elementType = GetElementType(currentLineData[2]);
            LayerDataUIToAssign.normalName = RemoveTags(currentLineData[2]);
            return LayerDataUIToAssign;
        }

        private string RemoveTags(string name)
        {
            for (int i = 0; i < tagTypes.Length; i++)
            {
                name = name.Replace(tagTypes[i], "");
            }
            return name;
        }

        private Vector2 GetLayerSize(string positionData)
        {
            positionData = positionData.Replace(" px", "");
            string[] bounds = positionData.Split(',');
            float x = float.Parse(bounds[0]);
            float y = float.Parse(bounds[1]);
            float width = float.Parse(bounds[2]) - x;
            float height = float.Parse(bounds[3]) - y;

            return new Vector2(width, height);
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

            //Sides
            Vector3 topLeft = GetVector(x, x + height);
            Vector3 bottomLeft = GetVector(x, y);
            Vector3 topRight = GetVector(x + width, y + height);
            Vector3 bottomRight = GetVector(x + width, y);

            return new Vector2(midx, canvasScaler.referenceResolution.y - midy);
        }

        private Vector3 GetVector(float x, float y)
        {
            return new Vector3(x, y, 0);
        }

        private LayerTypeUI GetLayerTypeUI(string typeData)
        {
            if (typeData == "LayerSet")
                return LayerTypeUI.Group;
            else
                return LayerTypeUI.Layer;
        }
        private void ReadDataLineWise(UnityEngine.Object fileToRead)
        {
            layerDataUI.Clear();
            string path = AssetDatabase.GetAssetPath(fileToRead);
            StreamReader reader = new StreamReader(path);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)))
                {
                    line = line.Substring(0, line.Length - 1);
                    LayerDataUI currentLayerDataUI = GetLayerDataUIFromLineData(line.Split(dataSeparator));
                    layerDataUI.Add(currentLayerDataUI);
                    GenerateObjectFromLayerDataUI(currentLayerDataUI);
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

        private LayerDataUI GetLayerDataUIFromID(int idToCompare)
        {
            for (int i = 0; i < parentLayerDataUI.Count; i++)
            {
                if (parentLayerDataUI[i].id == idToCompare)
                {
                    return parentLayerDataUI[i];
                }
            }
            return null;
        }

        private void GenerateObjectFromLayerDataUI(LayerDataUI dataToGenerateFrom)
        {
            if (currentLayerParent.id != dataToGenerateFrom.parentID)
                currentLayerParent = GetLayerDataUIFromID(dataToGenerateFrom.parentID);

            GameObject generatedObject = new GameObject(dataToGenerateFrom.name);

            dataToGenerateFrom.parent = currentLayerParent;
            dataToGenerateFrom.linkedObject = generatedObject;
            generatedObject.transform.SetParent(currentLayerParent.linkedObject.transform, false);
            if (dataToGenerateFrom.type == LayerTypeUI.Group)
            {
                generatedObject.transform.position = motherLayer.linkedObject.transform.position;
                dataToGenerateFrom.isParent = true;
                generatedObject.name = SetPrefix("Group", generatedObject.name);
                currentLayerParent = dataToGenerateFrom;
                parentLayerDataUI.Add(dataToGenerateFrom);
            }
            else
            {
                generatedObject.transform.position = ConvertPixelPositionToWorldPositon(dataToGenerateFrom.position);
                dataToGenerateFrom.isParent = false;
                if (GetElementType(generatedObject.name) == ElementType.Text)
                {
                    Text generatedText = generatedObject.AddComponent<Text>();
                    generatedText.rectTransform.sizeDelta = dataToGenerateFrom.size;
                    generatedText.text = generatedObject.name.Replace("(T)", "");
                    generatedText.alignment = TextAnchor.MiddleCenter;
                    generatedText.resizeTextForBestFit = true;
                    generatedText.resizeTextMaxSize = 200;
                    generatedObject.name = SetPrefix("Text", generatedObject.name);
                    generatedText.alignByGeometry = true;
                    if (fonts != null)
                    {
                        generatedText.font = fonts;
                    }
                }
                else
                {
                    generatedObject.name = SetPrefix("Image", generatedObject.name);
                    Image image = generatedObject.AddComponent<Image>();
                    image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(directoryPath + "/" + dataToGenerateFrom.name + ".png");
                    image.SetNativeSize();
                }

                if (GetElementType(generatedObject.name) == ElementType.Button)
                {
                    generatedObject.AddComponent<Button>();
                }

                artLayerDataUI.Add(dataToGenerateFrom);
            }
        }

        private string SetPrefix(string prefix, string objectName)
        {
            return prefix + " - " + objectName;
        }

        private ElementType GetElementType(string objectName)
        {
            if (objectName.Contains("(T)"))
            {
                return ElementType.Text;
            }
            else if (objectName.Contains("(B)"))
            {
                return ElementType.Button;
            }
            return ElementType.Image;
        }

        private void RemoveDuplicateSprites()
        {
            Image[] sr = GetSpritesWithSameResolution(motherLayer.linkedObject);
            for (int i = 0; i < sr.Length; i++)
            {
                if (sr[i] != null)
                {
                    for (int j = 0; j < sr.Length; j++)
                    {
                        if (isSameSprite(sr[i], sr[j]) && i != j)
                        {
                            if (!isDebugMode)
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sr[j].sprite));
                            sr[j].sprite = sr[i].sprite;
                            sr[j] = null;
                        }
                    }
                    sr[i] = null;
                }
            }
        }

        private bool isSameSprite(Image image1, Image image2)
        {
            if (image1 != null && image2 != null)
            {
                return ComparePixels(image1.sprite.texture, image2.sprite.texture);
            }
            return false;
        }

        private bool ComparePixels(Texture2D texture1, Texture2D texture2)
        {
            Color[] texPixels1 = texture1.GetPixels();
            Color[] texPixels2 = texture2.GetPixels();

            if (texPixels1.Length != texPixels2.Length)
                return false;

            int counter = 0;
            for (int i = 0; i < texPixels1.Length; i++)
            {
                if (texPixels1[i] != texPixels2[i])
                {
                    counter++;
                }
            }

            if (counter > 0)
                return false;
            else
                return true;
        }

        private Image[] GetSpritesWithSameResolution(GameObject objectToScan)
        {
            Image[] childSR = objectToScan.GetComponentsInChildren<Image>();
            List<Image> duplicateChilds = new List<Image>();
            for (int i = 0; i < childSR.Length; i++)
            {
                if (childSR[i] != null)
                {
                    for (int j = 0; j < childSR.Length; j++)
                    {
                        if (GetImageResolution(childSR[i]) == GetImageResolution(childSR[j]))
                        {
                            duplicateChilds.Add(childSR[j]);
                            childSR[j] = null;
                        }
                    }
                    childSR[i] = null;
                }
            }
            return duplicateChilds.ToArray();
        }

        private Vector2 GetImageResolution(Image image)
        {
            if (image != null)
            {
                return new Vector2(image.sprite.texture.width, image.sprite.texture.height);
            }
            return Vector2.zero;
        }
        #endregion

        #region PUBLIC_METHODS
        #endregion

        #region ENUMERATORS
        #endregion
    }

    public class LayerDataUI
    {
        public int id;
        public Vector2 position;
        public Vector2 size;
        public string name;
        public string normalName;
        public int parentID;
        public LayerDataUI parent;
        public LayerTypeUI type;
        public GameObject linkedObject;
        public bool isParent;
        public ElementType elementType;
    }

    public enum LayerTypeUI
    {
        Layer,
        Group
    }

    public enum ElementType
    {
        Text,
        Button,
        Image,
    }
}