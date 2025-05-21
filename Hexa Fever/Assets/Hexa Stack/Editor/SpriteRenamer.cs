using UnityEngine;
using UnityEditor;

public class SpriteRenamer : EditorWindow
{
    private string postfix = "_sprite";
    private bool includeSubfolders = true;
    private DefaultAsset folder;

    [MenuItem("Tools/Sprite Renamer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteRenamer>("Sprite Renamer");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite Batch Renaming Tool", EditorStyles.boldLabel);

        folder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Select Folder",
            folder,
            typeof(DefaultAsset),
            false
        );

        postfix = EditorGUILayout.TextField("Postfix to Add", postfix);
        includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);

        if (GUILayout.Button("Rename Sprites"))
        {
            if (folder != null)
            {
                RenameSpritesInFolder();
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "Please select a folder first!",
                    "OK"
                );
            }
        }
    }

    private void RenameSpritesInFolder()
    {
        string folderPath = AssetDatabase.GetAssetPath(folder);
        string searchPattern = includeSubfolders ? "/**/*.png" : "/*.png";
        string[] guids = AssetDatabase.FindAssets(
            "t:Sprite",
            new[] { folderPath }
        );

        int renamedCount = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null && textureImporter.textureType == TextureImporterType.Sprite)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                if (!fileName.EndsWith(postfix))
                {
                    string directory = System.IO.Path.GetDirectoryName(assetPath);
                    string newPath = System.IO.Path.Combine(
                        directory,
                        fileName + postfix + ".png"
                    );

                    AssetDatabase.MoveAsset(assetPath, newPath);
                    renamedCount++;
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Complete",
            $"Renamed {renamedCount} sprites",
            "OK"
        );
    }
}