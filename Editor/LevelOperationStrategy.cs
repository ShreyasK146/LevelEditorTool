using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class LevelOperationStrategy 
{

}
/*
 * interface to handle different type of level operations such as CREATING, EDITING ,DELETING
 */
public interface ILevelOperationStrategy
{
    void Execute(LevelData levelData, string path);
}

public class CreateLevelStrategy : ILevelOperationStrategy
{
    public void Execute(LevelData levelData, string path)
    {

        EnsurePathExists(path);

        string assetPath = Path.Combine(path, $"Level {levelData.ID}.asset");
        AssetDatabase.CreateAsset(levelData, assetPath);
        AssetDatabase.SaveAssets();
    }
    private void EnsurePathExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path); 
            AssetDatabase.Refresh(); 
        }
    }
}

public class EditLevelStrategy : ILevelOperationStrategy
{
    public void Execute(LevelData levelData, string path)
    {
        AssetDatabase.SaveAssets();
    }
}

public class DeleteLevelStrategy : ILevelOperationStrategy
{
    public void Execute(LevelData levelData, string path)
    {
        if (AssetDatabase.DeleteAsset(path))
        {
            Debug.Log($"Level {levelData.ID} deleted from {path}.");
        }
        else
        {
            Debug.LogWarning($"Failed to delete Level {levelData.ID}.");
        }
    }
}