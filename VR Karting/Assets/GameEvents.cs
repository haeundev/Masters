using System;
using DataTables;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject, Stimuli, string> OnStimuliTriggered;
    public static event Action OnScoreTriggered;

    public static void TriggerStimuli(GameObject lookAt, Stimuli stimuliInfo, string stimuliInfoA)
    {
        OnStimuliTriggered?.Invoke(lookAt, stimuliInfo, stimuliInfoA);
    }

    public static void TriggerScore()
    {
        OnScoreTriggered?.Invoke();
    }
}