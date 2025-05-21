using System;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Tag.HexaStack.Editor
{
    public class AssetDuplicator : OdinEditorWindow
    {
        [FolderPath, Required]
        public string sourcePath;

        [FolderPath, Required]
        public string destinationPath;

        [MinValue(1)]
        public int numberOfCopies = 1;

        [ReadOnly]
        [ShowInInspector]
        [LabelText("Status")]
        private string progressText = "Ready to duplicate";

        [MenuItem("Tools/Asset Duplicator")]
        private static void OpenWindow()
        {
            GetWindow<AssetDuplicator>("Asset Duplicator").Show();
        }

        [Button("Duplicate")]
        public void Duplicate()
        {
            try
            {
                for (int i = 0; i < numberOfCopies; i++)
                {
                    progressText = $"Duplicating... ({i + 1}/{numberOfCopies})";
                    CopyDirectoryDeep(sourcePath, destinationPath + "/" + (i + 1));
                }
                progressText = "Duplication completed successfully!";
            }
            catch (Exception ex)
            {
                progressText = "Error: " + ex.Message;
                Debug.LogError("Duplication failed: " + ex.Message);
            }
        }

        [Button("Duplicate in Parent")]
        public void DuplicateInParent()
        {
            try
            {
                string parentPath = Path.GetDirectoryName(sourcePath);
                string baseFolderName = Path.GetFileName(sourcePath);

                for (int i = 0; i < numberOfCopies; i++)
                {
                    progressText = $"Duplicating... ({i + 1}/{numberOfCopies})";
                    
                    string newFolderName = GetUniqueDirectoryName(parentPath, baseFolderName);
                    string newPath = Path.Combine(parentPath, newFolderName);

                    CopyDirectoryDeep(sourcePath, newPath);
                }
                progressText = "Duplication completed successfully!";
            }
            catch (Exception ex)
            {
                progressText = "Error: " + ex.Message;
                Debug.LogError($"Duplication in parent failed: {ex.Message}");
            }
        }

        private string GetUniqueDirectoryName(string parentPath, string baseName)
        {
            string newName = baseName;
            int counter = 1;

            while (Directory.Exists(Path.Combine(parentPath, newName)))
            {
                newName = $"{baseName}_{counter}";
                counter++;
            }

            return newName;
        }

        public static void CopyDirectoryDeep(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException("Source path does not exist: " + sourcePath);
            }

            CopyDirectoryRecursively(sourcePath, destinationPath);

            List<string> metaFiles = GetFilesRecursively(destinationPath, (f) => f.EndsWith(".meta"));
            List<(string originalGuid, string newGuid)> guidTable = new List<(string originalGuid, string newGuid)>();

            foreach (string metaFile in metaFiles)
            {
                string[] lines = File.ReadAllLines(metaFile);
                if (lines.Length < 2 || !lines[1].StartsWith("guid:"))
                {
                    throw new FormatException("Unexpected .meta file format in " + metaFile);
                }

                string originalGuid = lines[1].Substring(6);
                string newGuid = GUID.Generate().ToString().Replace("-", "");
                guidTable.Add((originalGuid, newGuid));

                lines[1] = "guid: " + newGuid;
                File.WriteAllLines(metaFile, lines);
            }

            List<string> allFiles = GetFilesRecursively(destinationPath);

            foreach (string fileToModify in allFiles)
            {
                if (!fileToModify.EndsWith(".meta") && !IsBinaryFile(fileToModify))
                {
                    string content = File.ReadAllText(fileToModify);

                    foreach (var guidPair in guidTable)
                    {
                        content = content.Replace(guidPair.originalGuid, guidPair.newGuid);
                    }

                    File.WriteAllText(fileToModify, content);
                }
            }

            AssetDatabase.Refresh();
        }

        private static void CopyDirectoryRecursively(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectoryRecursively(subdir.FullName, temppath);
            }
        }

        private static List<string> GetFilesRecursively(string path, Func<string, bool> criteria = null)
        {
            List<string> files = new List<string>();
            if (criteria == null)
            {
                files.AddRange(Directory.GetFiles(path));
            }
            else
            {
                files.AddRange(Directory.GetFiles(path).Where(criteria));
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                files.AddRange(GetFilesRecursively(directory, criteria));
            }

            return files;
        }

        private static bool IsBinaryFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            // Add other binary file extensions as needed
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".tga":
                case ".gif":
                case ".psd":
                case ".tiff":
                case ".exr":
                case ".iff":
                case ".pict":
                // Animation-related formats
                // case ".controller":  // Animator Controller
                case ".anim":        // Animation Clip
                case ".fbx":         // FBX (often contains animations)
                case ".mixer":       // Audio Mixer (also binary)
                    return true;
                default:
                    return false;
            }
        }
    }
}
