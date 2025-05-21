using Sirenix.OdinInspector;
using System.Collections.Generic;
using Tag.HexaStack.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PrefabPreviewWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private GameObject selectedPrefab;
    [PropertyRange(100, 300)]
    public float previewSize = 200f;
    private float padding = 15f;
    private Dictionary<GameObject, PreviewRenderUtility> previewUtilities = new Dictionary<GameObject, PreviewRenderUtility>();

    [FolderPath(RequireExistingPath = true)]
    private string prefabFolderPath = "Assets/Hexa Stack/Asset/TileSet";

    private List<GameObject> prefabsInFolder = new List<GameObject>();

    [MenuItem("Window/Level Editor/Prefab Preview")]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabPreviewWindow>("Prefab Preview");
        window.minSize = new Vector2(450, 400);
    }

    protected void OnEnable()
    {
        RefreshPrefabList();
        titleContent = new GUIContent("Prefab Preview", EditorGUIUtility.IconContent("Prefab Icon").image);
    }

    protected void OnDisable()
    {
        // Cleanup all PreviewRenderUtility instances
        Debug.Log("Cleaning up PreviewRenderUtility instances...");
        foreach (var utility in previewUtilities.Values)
        {
            utility.Cleanup(); // Dispose of the utility
        }

        Debug.Log("All PreviewRenderUtility instances cleaned up.");
        previewUtilities.Clear(); // Clear the dictionary to avoid memory leaks
    }

    private void OnDestroy()
    {
        foreach (var utility in previewUtilities.Values)
        {
            if (utility != null)
            {
                utility.Cleanup(); // Clean up resources
            }
        }

        previewUtilities.Clear();
        Debug.Log("PreviewRenderUtility instances cleaned up in OnDestroy.");
    }

    //~PrefabPreviewWindow()
    //{
    //    foreach (var utility in previewUtilities.Values)
    //    {
    //        utility.Cleanup();
    //    }
    //    previewUtilities.Clear();
    //}

    private void DrawPrefabPreview(GameObject prefab, Rect previewRect)
    {
        if (prefab == null) return;

        // Draw card background
        EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f, 1f));
        var innerRect = new Rect(previewRect.x + 2, previewRect.y + 2, previewRect.width - 4, previewRect.height - 4);
        EditorGUI.DrawRect(innerRect, new Color(0.3f, 0.3f, 0.3f, 1f));

        if (selectedPrefab == prefab)
        {
            var highlightRect = new Rect(previewRect.x + 1, previewRect.y + 1, previewRect.width - 2, previewRect.height - 2);
            EditorGUI.DrawRect(highlightRect, new Color(0.23f, 0.49f, 0.9f, 0.3f));
        }

        // Get or create preview utility
        if (!previewUtilities.ContainsKey(prefab))
        {
            var utility = new PreviewRenderUtility(true);
            utility.camera.transform.position = new Vector3(0, 0, -6);
            utility.camera.transform.rotation = Quaternion.identity;
            utility.camera.clearFlags = CameraClearFlags.Color;
            utility.camera.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            utility.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
            utility.lights[0].intensity = 1.2f;
            utility.lights[1].transform.rotation = Quaternion.Euler(30f, 210f, 0f);
            utility.lights[1].intensity = 1.2f;
            utility.ambientColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            previewUtilities[prefab] = utility;
        }

        var previewUtility = previewUtilities[prefab];

        // Preview area (excluding the label)
        Rect previewArea = new Rect(
            previewRect.x + 5,
            previewRect.y + 5,
            previewRect.width - 10,
            previewRect.height - 30
        );

        if (Event.current.type == EventType.Repaint)
        {
            previewUtility.BeginPreview(previewArea, GUIStyle.none);

            // Get prefab bounds
            var prefabInstance = previewUtility.InstantiatePrefabInScene(prefab);
            var bounds = GetBoundsRecursive(prefabInstance);

            // Center and scale the object to fit the preview
            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float scale = 1f / maxSize * 2f;
            prefabInstance.transform.localScale = Vector3.one * scale * 0.8f;
            prefabInstance.transform.position = -bounds.center * scale;

            // Apply rotation
            prefabInstance.transform.rotation = Quaternion.Euler(-90, -0, 0);

            previewUtility.Render();
            Object.DestroyImmediate(prefabInstance);

            GUI.DrawTexture(previewArea, previewUtility.EndPreview());
        }

        // Draw prefab name with background
        Rect labelRect = new Rect(
            previewRect.x,
            previewRect.y + previewRect.height - 25,
            previewRect.width,
            25
        );

        // Label background
        EditorGUI.DrawRect(labelRect, new Color(0.2f, 0.2f, 0.2f, 0.9f));

        // DEBUG: Log when mouse is over the label
        if (labelRect.Contains(Event.current.mousePosition))
        {
        }

        // Check click on label
        if (labelRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
        {
            Event.current.Use(); // Prevent further propagation
            selectedPrefab = prefab;
            //InstantiatePrefabInScene(prefab); // Instantiate prefab
            MainWindow.Instance.mapCreatore.PlaceNewTileSet(selectedPrefab);

            //foreach (var utility in previewUtilities.Values)
            //{
            //    utility.Cleanup(); // Dispose of the utility
            //}

            //previewUtilities.Clear(); // Clear the dictionary to avoid memory leaks

            this.Close();
        }

        // Draw name
        var style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        GUI.Label(labelRect, prefab.name, style);
    }

    private void InstantiatePrefabInScene(GameObject prefab)
    {
        if (prefab == null) return;

        // Create instance in active scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        // Place at origin for simplicity
        instance.transform.position = Vector3.zero;
    }

    private Bounds GetBoundsRecursive(GameObject gameObject)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);

        var bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

    protected void OnGUI()
    {
        EditorGUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        float windowWidth = position.width - padding * 2;
        int columns = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (previewSize + padding)));
        float startX = (windowWidth - (columns * (previewSize + padding) - padding)) / 2 + padding;

        float currentX = startX;
        float currentY = padding;

        for (int i = 0; i < prefabsInFolder.Count; i++)
        {
            GameObject prefab = prefabsInFolder[i];
            if (prefab == null) continue;

            if (currentX + previewSize > windowWidth + padding)
            {
                currentX = startX;
                currentY += previewSize + padding;
            }

            Rect previewRect = new Rect(currentX, currentY, previewSize, previewSize);

            DrawPrefabPreview(prefab, previewRect);
            currentX += previewSize + padding;
        }

        GUILayout.Space(currentY + previewSize + padding);
        EditorGUILayout.EndScrollView();
    }

    private void RefreshPrefabList()
    {
        prefabsInFolder.Clear();

        if (!string.IsNullOrEmpty(prefabFolderPath) && System.IO.Directory.Exists(prefabFolderPath))
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:prefab", new[] { prefabFolderPath });
            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null)
                {
                    prefabsInFolder.Add(prefab);
                }
            }
        }
    }
}
