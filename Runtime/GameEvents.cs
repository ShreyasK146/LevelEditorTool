using System;


/*
 * Class is responsible for handling event Action delegates 
 */
public static class GameEvents 
{
    public static event Action OnCorrectAnswer;
    public static event Action OnIncorrectAnswer;
    public static event Action<int, bool> onButtonSelectionChanged;

    public static void TriggerCorrectAnswerEvent()
    {
        OnCorrectAnswer?.Invoke();
    }

    public static void TriggerIncorrectAnswerEvent()
    {
        OnIncorrectAnswer?.Invoke();
    }

    public static void TriggerButtonSelectionChanged(int buttonIndex, bool isSelected)
    {
        onButtonSelectionChanged?.Invoke(buttonIndex, isSelected);
    }

}
