using System;
using DataTables;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject, Stimuli, string> OnStimuliTriggered;
    public static event Action<int> OnScoreUpdate;

    public static void TriggerStimuli(GameObject lookAt, Stimuli stimuliInfo, string word)
    {
        OnStimuliTriggered?.Invoke(lookAt, stimuliInfo, word);
    }

    public static void TriggerScoreUpdate(int score)
    {
        OnScoreUpdate?.Invoke(score);
    }
}