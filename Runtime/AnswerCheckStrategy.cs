using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Strategy Pattern to implement different type of answer checks based on different level or so..
 */
public class AnswerCheckStrategy
{

}

public interface IAnswerCheckStrategy
{
    bool CheckAnswer(Dictionary<int,bool>buttonStates, LevelData levelData);   
}

public class ExactMatchStrategy : IAnswerCheckStrategy
{
    public bool CheckAnswer(Dictionary<int, bool> buttonStates, LevelData levelData)
    {
        int matchedCount = 0;
        int selectedCount = 0;

        for (int i = 0; i < levelData.words.Count; i++)
        {
            if (buttonStates[i])
            {
                selectedCount++;
                if (LevelManager.Instance.SelectedLevelData.answers[i])
                    matchedCount++;
            }
        }
        int correctAnswersCount = LevelManager.Instance.SelectedLevelData.answers.Count(a => a);
        return matchedCount == correctAnswersCount && matchedCount == selectedCount; // making sure actual answer count is same as selected count
    }
}

// we can implement this in future if partial match should yield some result for some levels 
public class PartialMatchStrategy : IAnswerCheckStrategy 
{
    public bool CheckAnswer(Dictionary<int, bool> buttonStates, LevelData levelData)
    {
        throw new System.NotImplementedException();
    }
}

