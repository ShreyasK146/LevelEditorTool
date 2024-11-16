using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]  
public class AutoAttachLevelManager
{
    static AutoAttachLevelManager()
    {
        //registering a callback so that whenever some change happens on editor or hierarchy 
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    
    private static void OnHierarchyChanged()
    {
        // so when on this first scene make sure level manager , level selector script are added to gameobjects
        if (SceneManager.GetActiveScene().name == "LevelSelectScene") 
        {
            
            GameObject levelManagerObject = GameObject.Find("LevelManager"); 
            GameObject layout = GameObject.Find("Layout");
            if (levelManagerObject != null)
            {
                
                if (levelManagerObject.GetComponent<LevelManager>() == null)
                {
                   
                    levelManagerObject.AddComponent<LevelManager>();
        
                }
            }
            if (layout != null)
            {
             
                if (layout.GetComponent<LevelSelector>() == null)
                {
                   
                    layout.AddComponent<LevelSelector>();
         
                }
            }
        }
        // so when on this second scene make sure game manager , animation controller scripts are added to gameobjects
        if (SceneManager.GetActiveScene().name == "LevelScene") 
        {

            GameObject panel = GameObject.Find("Panel"); 
            GameObject gameManager = GameObject.Find("GameManager");

            if (panel != null)
            {
               
                if (panel.GetComponent<AnimationController>() == null)
                {
                   
                    panel.AddComponent<AnimationController>();

                }
            }
            if (gameManager != null)
            {
               
                if (gameManager.GetComponent<GameManager1>() == null)
                {
                    
                    gameManager.AddComponent<GameManager1>();

                }
            }

        }
    }
}