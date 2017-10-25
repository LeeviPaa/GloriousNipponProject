using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorFileCleanerUtility
{
    [MenuItem("Tool/Clean Useless Files")]
    private static void CleanMenuItem()
    {
        Clean(Application.dataPath);
    }

    public static void Clean(string root)
    {
        Debug.Log("Starting useless file cleaning...");
        string[] emptyDirectories = FindEmptyDirectories(root);
        string[] directoryMetafiles = FindDirectoryMetafiles(emptyDirectories);

        foreach (string path in directoryMetafiles)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Deleted: " + MakeRelativePath(path, Application.dataPath.Length));
            }
        }

        foreach (string path in emptyDirectories)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
                //Debug.Log("Deleted: " + MakeRelativePath(path, Application.dataPath.Length));
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Cleaning complete!");
    }

    private static string MakeRelativePath(string path, int charsToRemove)
    {
        if (charsToRemove <= path.Length && charsToRemove >= 0)
        {
            int dif = path.Length - charsToRemove;
            char[] cutString = new char[dif];
            for (int i = 0; i < dif; i++)
            {
                cutString[i] = path[i + path.Length - dif];
            }
            return new string(cutString);
        }
        return path;
    }

    private static string FindParentWithDirectory(string toFind, string start)
    {
        toFind = toFind.ToLower();
        DirectoryInfo currentFolder = new DirectoryInfo(start);
        DirectoryInfo[] directories = currentFolder.GetDirectories();
        foreach (DirectoryInfo dir in directories)
        {
            if (dir.Name.ToLower() == toFind)
            {
                return currentFolder.FullName;
            }
        }
        if (currentFolder.Parent != null)
        {
            return FindParentWithDirectory(toFind, currentFolder.Parent.FullName);
        }
        else
        {
            return "";
        }
    }

    private static string FindFirstDirectory(string toFind, string root)
    {
        toFind = toFind.ToLower();
        int currentDepth = 0;
        List<DirectoryInfo> currentDepthDirectories = new List<DirectoryInfo>() { new DirectoryInfo(root) };
        do
        {
            foreach (DirectoryInfo dir in currentDepthDirectories)
            {
                if (dir.Name.ToLower() == toFind)
                {
                    return dir.FullName;
                }
            }
            List<DirectoryInfo> nextDepthDirectories = new List<DirectoryInfo>();
            foreach (DirectoryInfo dir in currentDepthDirectories)
            {
                nextDepthDirectories.AddRange(dir.GetDirectories());
            }
            currentDepthDirectories = nextDepthDirectories;
            currentDepth++;
        }
        while (currentDepthDirectories.Count > 0);
        return "";
    }

    private static string[] FindEmptyDirectories(string path)
    {
        string[] directories = Directory.GetDirectories(path);
        List<string> emptyDirectories = new List<string>();
        foreach (string dir in directories)
        {
            emptyDirectories.AddRange(FindEmptyDirectories(dir));
        }

        string[] files = Directory.GetFiles(path);
        if (files.Length == 0)
        {
            bool success = true;
            foreach (string dir in directories)
            {
                bool found = false;
                foreach (string emptyDir in emptyDirectories)
                {
                    if (dir == emptyDir)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                emptyDirectories.Add(path);
            }
        }
        return emptyDirectories.ToArray();
    }

    private static string[] FindDirectoryMetafiles(string[] paths)
    {
        List<string> foundMetafilePaths = new List<string>();
        foreach (string path in paths)
        {
            DirectoryInfo current = new DirectoryInfo(path);
            string metafilePath = current.Parent.FullName + "\\" + current.Name + ".meta";
            if (File.Exists(metafilePath))
            {
                foundMetafilePaths.Add(metafilePath);
            }
        }
        return foundMetafilePaths.ToArray();
    }
}
