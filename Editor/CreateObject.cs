
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;



public class CreateObject : EditorWindow
{

    private LevelData newLevelData = null;
    private LevelData levelData;
    private bool toggleValue;
    private int selectedIndex = 0;
    private int selectedIndexToRemove = 0;
    string path = "Assets/Resources/ScriptableObj/";
    bool isCreatingNewLevel;
    private bool confirmDelete = false;
    private ILevelOperationStrategy currentLevelOperationStrategy;

    /*
     * 
     * 
     * 
     * WINDOW -> SCRIPTABLE OBJECT CREATOR IS THE TOOL TO CREATE LEVELS   
     * 
     * 
     * 
     */

    [MenuItem("Window/ScriptableObjectCreator")]
    public static void ShowWindow()
    {
        CreateObject obj = (CreateObject)GetWindow(typeof(CreateObject));
        obj.minSize = new Vector2(500, 1200);
        obj.maxSize = new Vector2(500, 1200);
        
    }
    void OnGUI()
    {
        EnsurePathExists(path);

        List<string> guIds = AssetDatabase.FindAssets("", new[] { path }).ToList(); // to basically gets all guids of files in that path 
        List<string> options1 = guIds.Select(guId => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guId))) // gets the file name without extension of all guids
        .ToList();

        /*
         * THIS CODE CONTAINS THE LOGIC TO CREATE A NEW LEVLES ON EDITOR WINDOW
         * QUESTION, ANIMATION OVERRIDE CONTROLLER , ANSWERS , OPTIONS AND SAVING OF A NEW LEVEL IS DONE HERE
         * ALSO HANDLES THE LEVEL REMOVING PART AS WELL
         */

        GUILayout.Label("Create New Level", EditorStyles.boldLabel);
        if (GUILayout.Button("Create"))
        {
            isCreatingNewLevel = true;

            if (newLevelData == null)
            {
                newLevelData = ScriptableObject.CreateInstance<LevelData>();
            }
        }
        if (isCreatingNewLevel && newLevelData != null)
        {

            DrawLevelCreationUI(options1);

        }

        DrawEditLevelUI(options1);
        DrawDeleteLevelUI(options1);

    }

    private void DrawLevelCreationUI(List<string> options1)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Question");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        newLevelData.question = EditorGUILayout.TextField(newLevelData.question);
        EditorGUILayout.EndHorizontal();

        newLevelData.overrideController = (AnimatorOverrideController)EditorGUILayout.ObjectField(
"Animator Controller", newLevelData.overrideController, typeof(AnimatorOverrideController), false);


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Options To Choose");
        GUILayout.Space(57);
        GUILayout.Label("Answers");
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space(20);
        HandleWordEdit(newLevelData);
        EditorGUILayout.Space(20);
        newLevelData.ID = GetNextLevelID(options1);
        EditorGUILayout.BeginHorizontal();
        if (newLevelData.words.Count >= 5)
        {
            if (GUILayout.Button("Save"))
            {
                currentLevelOperationStrategy = new CreateLevelStrategy();
                currentLevelOperationStrategy.Execute(newLevelData, path);
                isCreatingNewLevel = false;
                newLevelData = null;
            }

        }

        if (GUILayout.Button("Cancel"))
        {
            isCreatingNewLevel = false;
            newLevelData = null;
        }
        EditorGUILayout.EndHorizontal();
    }

    /*
     * This is needed to not to overwrite levels based on index;
     */
    private int GetNextLevelID(List<string> options1)
    {
        int maxId = 0;

        foreach( var level in options1)
        {
            string assetPath = Path.Combine(path, $"{level}.asset");
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
            if (levelData != null && levelData.ID > maxId) 
                maxId = levelData.ID;
        }
        return maxId + 1;
    }

    private void DrawEditLevelUI(List<string> options1)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("EditLevel", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        selectedIndex = EditorGUILayout.Popup("Levels", selectedIndex, options1.ToArray());
        EditorGUILayout.EndHorizontal();

        if (selectedIndex >= 0 && selectedIndex < options1.Count)
        {
            string assetPath = Path.Combine(path, $"{options1[selectedIndex]}.asset");
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
            ShowCurrentLevelDetails(level, assetPath);
        }
    }

    private void DrawDeleteLevelUI(List<string> options1)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Select a level to remove");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (options1.Count > 0)
        {
            selectedIndexToRemove = EditorGUILayout.Popup("Levels", selectedIndexToRemove, options1.ToArray());

            confirmDelete = GUILayout.Toggle(confirmDelete, "Are you sure?");

            if (confirmDelete)
            {
                if (GUILayout.Button("Delete"))
                {
                    string assetPath = Path.Combine(path, $"{options1[selectedIndexToRemove]}.asset");
                    LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);

                   
                    currentLevelOperationStrategy = new DeleteLevelStrategy();
                    currentLevelOperationStrategy.Execute(level, assetPath);


                    options1.RemoveAt(selectedIndexToRemove);
                    selectedIndexToRemove = Mathf.Clamp(selectedIndexToRemove, 0, options1.Count - 1);

                    
                    Repaint();
                    Debug.Log($"Level {options1[selectedIndexToRemove]} deleted successfully.");
                }
            }
        }
        else
        {
            GUILayout.Label("No levels available to remove.");
        }
        EditorGUILayout.EndHorizontal();
    }



    /*
     * THIS CODE IS RESPONSIBLE TO SHOW THE CURRENT LEVEL DETAILS BASED ON THE OPTIONS YOU SELECT FROM DROPDOWN
     */
    void ShowCurrentLevelDetails(LevelData level , string assetName)
    {
       
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Question",EditorStyles.boldLabel);
        level.question = EditorGUILayout.TextField(level.question);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Options to Choose");
        GUILayout.Space(57);
        GUILayout.Label("Answers");
        EditorGUILayout.EndHorizontal();
        if (level.words == null) level.words = new List<string>();
        if (level.answers == null) level.answers = new List<bool>();


        HandleWordEdit(level);
        level.overrideController = (AnimatorOverrideController)EditorGUILayout.ObjectField(
    "Animator Controller", level.overrideController, typeof(AnimatorOverrideController), false);

        if (GUI.changed)
        {
            currentLevelOperationStrategy = new EditLevelStrategy();
            currentLevelOperationStrategy.Execute(level, assetName);  
            Debug.Log("Level data updated and saved.");
        }
    }


    /*
     * SEPERATED THIS PART OF THE CODE WHERE IT HANDLES THE OPTIONS ADDING , REMOVING ALONG WITH ANSWERS 
     */
    void HandleWordEdit(LevelData level)
    {

        for (int i = 0; i < level.words.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();
            level.words[i] = EditorGUILayout.TextField(level.words[i]);
            GUILayout.Space(75);
            level.answers[i] = EditorGUILayout.Toggle(level.answers[i]);
            EditorGUILayout.EndHorizontal();

        }
      
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            level.words.Add(string.Empty);
            level.answers.Add(false);
        }
        // because lets keep minimum 5 questions right to save a level
        if (level.words.Count > 5)
        {
            if (GUILayout.Button("-"))
            {
                level.words.RemoveAt(level.words.Count - 1);
                level.answers.RemoveAt(level.answers.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    // just to make sure path is there
    private void EnsurePathExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path); 
            AssetDatabase.Refresh();

        }
    }

}
