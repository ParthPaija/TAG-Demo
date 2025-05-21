//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;

//public class PrefabList : ScriptableObject
//{
//    public List<GameObject> prefabs = new List<GameObject>();
//}

//[CustomEditor(typeof(PrefabList))]
//public class PrefabListEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        EditorGUILayout.LabelField("Prefab List", EditorStyles.boldLabel);
//        EditorGUILayout.Space();

//        PrefabList prefabList = (PrefabList)target;

//        if (GUILayout.Button("Add Prefab"))
//        {
//            prefabList.prefabs.Add(null);
//        }

//        for (int i = prefabList.prefabs.Count - 1; i >= 0; i--)
//        {
//            EditorGUILayout.BeginHorizontal();

//            prefabList.prefabs[i] = (GameObject)EditorGUILayout.ObjectField(
//                prefabList.prefabs[i],
//                typeof(GameObject),
//                false
//            );

//            if (GUILayout.Button("Remove", GUILayout.Width(60)))
//            {
//                prefabList.prefabs.RemoveAt(i);
//            }

//            EditorGUILayout.EndHorizontal();
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}

//public class PrefabPreviewWindowNew : EditorWindow
//{
//    private PrefabList prefabList;
//    private Vector2 scrollPosition;
//    private GameObject selectedPrefab;
//    private float previewSize = 100f;
//    private float padding = 10f;
//    private PreviewRenderUtility previewRenderer;
//    private GUIStyle previewBackground;

//    [MenuItem("Window/Level Editor/Prefab Preview New")]
//    public static void ShowWindow()
//    {
//        GetWindow<PrefabPreviewWindowNew>("Prefab Preview");
//    }

//    private void OnEnable()
//    {
//        LoadOrCreatePrefabList();
//        InitializePreviewRenderer();
//        InitializeStyles();
//    }

//    private void InitializeStyles()
//    {
//        previewBackground = new GUIStyle();
//        previewBackground.normal.background = EditorGUIUtility.whiteTexture;
//    }

//    private void LoadOrCreatePrefabList()
//    {
//        string[] guids = AssetDatabase.FindAssets("t:PrefabList");
//        if (guids.Length > 0)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
//            prefabList = AssetDatabase.LoadAssetAtPath<PrefabList>(path);
//        }

//        if (prefabList == null)
//        {
//            prefabList = CreateInstance<PrefabList>();
//            string path = "Assets/PrefabList.asset";
//            AssetDatabase.CreateAsset(prefabList, path);
//            AssetDatabase.SaveAssets();
//        }
//    }

//    private void InitializePreviewRenderer()
//    {
//        if (previewRenderer == null)
//        {
//            previewRenderer = new PreviewRenderUtility();

//            // Match scene view camera settings
//            if (SceneView.lastActiveSceneView != null)
//            {
//                previewRenderer.camera.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
//                previewRenderer.camera.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
//                previewRenderer.camera.orthographic = SceneView.lastActiveSceneView.camera.orthographic;
//            }
//            else
//            {
//                // Default camera settings if no scene view is available
//                previewRenderer.camera.transform.position = new Vector3(0, 5, -10);
//                previewRenderer.camera.transform.rotation = Quaternion.Euler(30, 0, 0);
//            }

//            // Add lights similar to scene view
//            previewRenderer.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
//            previewRenderer.lights[0].intensity = 1f;
//            previewRenderer.lights[1].intensity = 0.7f;
//        }
//    }

//    private void DrawPrefabPreview(GameObject prefab, Rect previewRect)
//    {
//        if (prefab == null) return;

//        // Draw selection highlight
//        if (selectedPrefab == prefab)
//        {
//            EditorGUI.DrawRect(previewRect, new Color(0.23f, 0.49f, 0.9f, 0.3f));
//        }

//        Rect actualPreviewRect = new Rect(
//            previewRect.x + 5,
//            previewRect.y + 5,
//            previewRect.width - 10,
//            previewRect.height - 25
//        );

//        //Draw preview background
//        EditorGUI.DrawRect(actualPreviewRect, new Color(0.8f, 0.8f, 0.8f, 1f));

//        // Begin preview
//        previewRenderer.BeginPreview(actualPreviewRect, previewBackground);

//        // Create preview instance
//        GameObject previewInstance = GameObject.Instantiate(prefab);

//        //Handle 2D sprites
//        SpriteRenderer spriteRenderer = previewInstance.GetComponent<SpriteRenderer>();
//        if (spriteRenderer != null)
//        {
//            // Set orthographic camera for 2D
//            previewRenderer.camera.orthographic = true;
//            previewRenderer.camera.orthographicSize = spriteRenderer.bounds.size.y / 2f;
//            previewRenderer.camera.transform.position = new Vector3(0, 0, -10);
//            previewRenderer.camera.transform.rotation = Quaternion.identity;
//        }
//        else
//        {
//            // Match scene view camera for 3D
//            if (SceneView.lastActiveSceneView != null)
//            {
//                previewRenderer.camera.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
//                previewRenderer.camera.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
//                previewRenderer.camera.orthographic = SceneView.lastActiveSceneView.camera.orthographic;
//            }
//        }

//        // Auto-adjust camera distance based on bounds
//        Renderer[] renderers = previewInstance.GetComponentsInChildren<Renderer>();
//        if (renderers.Length > 0)
//        {
//            Bounds bounds = renderers[0].bounds;
//            foreach (Renderer renderer in renderers)
//            {
//                bounds.Encapsulate(renderer.bounds);
//            }

//            float objectSize = bounds.size.magnitude;
//            float distance = objectSize * 2f;

//            if (!spriteRenderer) // Only for 3D objects
//            {
//                previewRenderer.camera.transform.position = -previewRenderer.camera.transform.forward * distance;
//            }
//        }

//        // Render the preview
//        previewRenderer.Render();

//        // Get the rendered texture
//        Texture previewTexture = previewRenderer.EndPreview();

//        // Draw the preview texture
//        GUI.DrawTexture(actualPreviewRect, previewTexture, ScaleMode.ScaleToFit);

//        // Cleanup
//        DestroyImmediate(previewInstance);

//        // Draw prefab name
//        Rect labelRect = new Rect(
//            previewRect.x,
//            previewRect.y + previewRect.height - 20,
//            previewRect.width,
//            20
//        );
//        EditorGUI.DrawRect(labelRect, new Color(0.8f, 0.8f, 0.8f, 0.9f));
//        GUI.Label(labelRect, prefab.name, EditorStyles.centeredGreyMiniLabel);
//    }

//    private void InstantiatePrefabInScene(GameObject prefab)
//    {
//        // Get the scene view camera
//        SceneView sceneView = SceneView.lastActiveSceneView;
//        if (sceneView != null)
//        {
//            // Create the prefab instance
//            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
//            Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");

//            // Position the instance at the scene view camera's pivot point or raycast hit point
//            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit))
//            {
//                instance.transform.position = hit.point;
//            }
//            else
//            {
//                // If no hit, place it at a distance from the camera
//                instance.transform.position = sceneView.camera.transform.position +
//                                           sceneView.camera.transform.forward * 5f;
//            }

//            // Select the newly created instance
//            Selection.activeGameObject = instance;
//            SceneView.RepaintAll();
//        }
//    }

//    private void OnGUI()
//    {
//        EditorGUILayout.BeginVertical();

//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("Manage Prefabs", GUILayout.Width(100)))
//        {
//            Selection.activeObject = prefabList;
//            EditorGUIUtility.PingObject(prefabList);
//        }

//        if (GUILayout.Button("Match Scene View", GUILayout.Width(100)))
//        {
//            if (SceneView.lastActiveSceneView != null)
//            {
//                previewRenderer.camera.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
//                previewRenderer.camera.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
//                previewRenderer.camera.orthographic = SceneView.lastActiveSceneView.camera.orthographic;
//                Repaint();
//            }
//        }

//        previewSize = EditorGUILayout.Slider("Preview Size", previewSize, 50f, 200f);
//        EditorGUILayout.EndHorizontal();

//        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//        float currentX = padding;
//        float currentY = padding;
//        float windowWidth = position.width - padding * 2;
//        int columns = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (previewSize + padding)));

//        for (int i = 0; i < prefabList.prefabs.Count; i++)
//        {
//            GameObject prefab = prefabList.prefabs[i];
//            if (prefab == null) continue;

//            if (currentX + previewSize > windowWidth)
//            {
//                currentX = padding;
//                currentY += previewSize + padding;
//            }

//            Rect previewRect = new Rect(currentX, currentY, previewSize, previewSize);

//            DrawPrefabPreview(prefab, previewRect);

//            // Handle mouse events
//            if (Event.current.type == EventType.MouseDown &&
//                previewRect.Contains(Event.current.mousePosition))
//            {
//                selectedPrefab = prefab;

//                switch (Event.current.button)
//                {
//                    case 0: // Left click - Select and Instantiate
//                        InstantiatePrefabInScene(prefab);
//                        break;

//                    case 1: // Right click - Context menu
//                        ShowContextMenu(selectedPrefab);
//                        break;
//                }

//                Event.current.Use();
//                Repaint();
//            }

//            currentX += previewSize + padding;
//        }

//        GUILayout.Space(currentY + previewSize + padding);

//        EditorGUILayout.EndScrollView();
//        EditorGUILayout.EndVertical();
//    }

//    private void ShowContextMenu(GameObject prefab)
//    {
//        GenericMenu menu = new GenericMenu();

//        menu.AddItem(new GUIContent("Select Prefab"), false, () =>
//        {
//            Selection.activeObject = prefab;
//        });

//        menu.AddItem(new GUIContent("Ping Prefab"), false, () =>
//        {
//            EditorGUIUtility.PingObject(prefab);
//        });

//        menu.ShowAsContext();
//    }

//    private void OnDisable()
//    {
//        if (previewRenderer != null)
//        {
//            previewRenderer.Cleanup();
//            previewRenderer = null;
//        }
//    }
//}
