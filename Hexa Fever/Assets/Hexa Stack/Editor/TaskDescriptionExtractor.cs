using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using Tag.MetaGame.TaskSystem;
using Tag.TaskSystem;

public class TaskDescriptionExtractor : MonoBehaviour
{
    [MenuItem("Tools/Extract Task Descriptions")]
    public static void ExtractTaskDescriptions()
    {
        // Get all SimpleTask assets in the project
        string[] guids = AssetDatabase.FindAssets("t:BaseTaskData");
        StringBuilder csv = new StringBuilder();

        // Add header
        csv.AppendLine("Description");

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            BaseTaskData task = AssetDatabase.LoadAssetAtPath<BaseTaskData>(assetPath);

            if (task != null)
            {
                // Add description to CSV, handle any commas in description
                string description = task.description.Replace(",", "");  // Removing commas to avoid CSV issues
                csv.AppendLine(description);
            }
        }

        // Save to file
        string savePath = Application.dataPath + "/TaskDescriptions.csv";
        File.WriteAllText(savePath, csv.ToString());

        Debug.Log($"Task descriptions exported to: {savePath}");
        AssetDatabase.Refresh();
    }
}