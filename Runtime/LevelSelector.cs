using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    private GameObject layout;
    private LevelData[] levelCount;
    [SerializeField] private GameObject buttonPrefab;
    private string sceneName = "LevelScene";  // Name of the scene to load

    private void Awake()
    {
        // dynamically loading buttonprefab and count of levels 
        buttonPrefab = Resources.Load<GameObject>("Prefabs/LevelButton");
        levelCount = Resources.LoadAll<LevelData>("ScriptableObj");
        layout = GameObject.Find("Layout");
        PopulateGrid();
    }


    /*
     * PLEASE MAKE SURE THAT YOU HAVE ADDED LevelScene TO BUILD SETTINGS  
     * YOU CAN LOCATE THIS UNDER 
     * Assets->Samples->Puzzle Game Tool->1.0.0->Level Scene Example
     */
    void PopulateGrid()
    {
        
        for (int i = 0; i < levelCount.Length; i++)
        {
            Button button = Instantiate(buttonPrefab, layout.transform).GetComponent<Button>();
            button.name = $"Level {i + 1}";
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i + 1}";
            int currentIndex = i;
            button.onClick.AddListener(() =>
            {
                LevelManager.Instance.SetSelectedLevel(levelCount[currentIndex]);
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}