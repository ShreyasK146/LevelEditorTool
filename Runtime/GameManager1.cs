
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManager1 : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI question;
    [SerializeField] TextMeshProUGUI result;
    [SerializeField] GameObject buttonPrefab;
    Animator animator;
    private int numberOfOptions;
    private GameObject layout;
    //private Button buttonObject;
    //Button buttonPrefab;
    LevelData levelData;
    private Dictionary<int, bool> buttonStates;
    //[SerializeField] Animator panelAnimator;
    private IAnswerCheckStrategy answerCheckStrategy;
    private Button checkAnswerButton;

    // in this start method we are making sure that text fields are properly loaded and subscribing to events 
    private void Start()
    {

        checkAnswerButton = GameObject.Find("ShowResult").GetComponent<Button>();
        checkAnswerButton.onClick.AddListener(CheckAnswer);
        buttonPrefab = Resources.Load<GameObject>("Prefabs/LevelButton");
        levelData = LevelManager.Instance.SelectedLevelData;
        if (question == null)
        {
            question = GameObject.Find("Question").GetComponent<TextMeshProUGUI>();
        }

        if (result == null)
        {
            result = GameObject.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        }

        buttonStates = new Dictionary<int, bool>();
        numberOfOptions = LevelManager.Instance.SelectedLevelData.words.Count;
       
        question.text = LevelManager.Instance.SelectedLevelData.question;
        question.color = Color.yellow;
        layout = GameObject.Find("Layout");
        SetAnswerCheckStrategy(new ExactMatchStrategy());
        PopulateGrid(buttonPrefab);
        GameEvents.onButtonSelectionChanged += HandleButtonSelectionChanged;


    }

    // this is where answer check strategy will be assigned

    public void SetAnswerCheckStrategy(IAnswerCheckStrategy strategy)
    {
        answerCheckStrategy = strategy;
    }



    /*
     * Populating the buttons based on the number of options
     */
    void PopulateGrid(GameObject buttonPrefab)
    {

        for (int i = 0; i < numberOfOptions; i++)
        {
            Button button = Instantiate(buttonPrefab, layout.transform).GetComponent<Button>();
            //Button button = Instantiate(buttonPrefab, layout.transform);
            button.name = "Button " + i;
            button.GetComponentInChildren<TextMeshProUGUI>().text = LevelManager.Instance.SelectedLevelData.words[i];
            buttonStates[i] = false;
            int selectedIndex = i;
            button.onClick.AddListener(() =>
            {
                buttonStates[selectedIndex] = !buttonStates[selectedIndex];
                button.GetComponent<Image>().color = buttonStates[selectedIndex] ? Color.grey : Color.white; // if selected change the button color from white to grey or vise versa
                GameEvents.TriggerButtonSelectionChanged(selectedIndex, buttonStates[selectedIndex]);
            });
        }
    }
    /*
     * when player presses check button it basically checks if the selected buttons and answers match if yes different animation is played by triggering events 
     */
    public void CheckAnswer()
    {
        bool resultState = answerCheckStrategy.CheckAnswer(buttonStates, levelData);

        if (resultState)
        {
            result.text = "Correct...";
            result.color = Color.green;
            
            GameEvents.TriggerCorrectAnswerEvent();
        }
        else
        {
            result.text = "Wrong...";
            result.color = Color.red;
    
            GameEvents.TriggerIncorrectAnswerEvent();
        }

    }
    private void HandleButtonSelectionChanged(int buttonIndex, bool isSelected)
    {
        buttonStates[buttonIndex] = isSelected;
    }

    private void OnDestroy()
    {
        GameEvents.onButtonSelectionChanged -= HandleButtonSelectionChanged;
    }
}
