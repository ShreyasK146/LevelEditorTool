using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator panelAnimator;
    LevelData levelData;
    private void Awake()
    {
        // making sure panel animator is assigned
        if (panelAnimator == null)
        {
            panelAnimator = GetComponent<Animator>();
            if (panelAnimator == null)
            {
                Debug.LogError("Animator component not found on the Panel GameObject.");
                return;
            }
        }

        // Get the selected level data.
        levelData = LevelManager.Instance?.SelectedLevelData;

        if (levelData == null)
        {
            Debug.LogError("Selected LevelData is missing.");
            return;
        }

        AssignAnimatorController();
    }

    private void AssignAnimatorController()
    {
        // Assign base animator controller if it's missing.
        if (panelAnimator.runtimeAnimatorController == null)
        {
            var baseController = Resources.Load<RuntimeAnimatorController>("Animations/Base/BaseController");
            if (baseController != null)
            {
                panelAnimator.runtimeAnimatorController = baseController;
                Debug.Log("Base AnimatorController assigned.");
            }
            else
            {
                Debug.LogError("Failed to load Base AnimatorController from Resources.");
                return;
            }
        }

        // Assign override animator controller if available.
        // this will ensure different animation sets are played for different levels if its added during level creation or edition
        if (levelData.overrideController != null)
        {
            panelAnimator.runtimeAnimatorController = levelData.overrideController;
            Debug.Log("Override AnimatorController assigned from LevelData.");
        }
        else
        {
            Debug.Log("No override controller found in LevelData. Using Base AnimatorController.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to game events.
        GameEvents.OnCorrectAnswer += PlayCorrectAnswerAnimation;
        GameEvents.OnIncorrectAnswer += PlayDefaultAnimation;
    }

    private void OnDisable()
    {
        // Unsubscribe from game events.
        GameEvents.OnCorrectAnswer -= PlayCorrectAnswerAnimation;
        GameEvents.OnIncorrectAnswer -= PlayDefaultAnimation;
    }

    // this will be played or will be playing when answer is not correct or answer is not entered yet
    private void PlayDefaultAnimation()
    {
        if (panelAnimator != null)
        {
            panelAnimator.SetBool("pass", false);
        }
        else
        {
            Debug.LogWarning("Animator is not assigned. Cannot play Default Animation.");
        }
    }
    // if answer is correct the animation will transition
    private void PlayCorrectAnswerAnimation()
    {
        if (panelAnimator != null)
        {
            panelAnimator.SetBool("pass", true);
        }
        else
        {
            Debug.LogWarning("Animator is not assigned. Cannot play Correct Answer Animation.");
        }
    }
}
