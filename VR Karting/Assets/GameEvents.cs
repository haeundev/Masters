using System;
using DataTables;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject, Stimuli, string> OnStimuliTriggered;
    public static event Action<int> OnScoreUpdate;
    public static event Action OnPushAllStimuliForward;

    public static void TriggerStimuli(GameObject lookAt, Stimuli stimuliInfo, string word)
    {
        OnStimuliTriggered?.Invoke(lookAt, stimuliInfo, word);
        Debug.Log($"Triggered: {lookAt.name} - {stimuliInfo.Contrast} - {word}");
    }

    public static void TriggerScoreUpdate(int score)
    {
        OnScoreUpdate?.Invoke(score);
        Debug.Log($"Score: {score}");
    }

    public static void TriggerPushAllStimuliForward()
    {
        OnPushAllStimuliForward?.Invoke();
        Debug.Log("Push all stimuli forward");
    }
}