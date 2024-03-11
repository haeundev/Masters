using System;
using DataTables;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject, Stimuli, string> OnStimuliTriggered;

    public static void TriggerStimuli(GameObject lookAt, Stimuli stimuliInfo, string stimuliInfoA)
    {
        OnStimuliTriggered?.Invoke(lookAt, stimuliInfo, stimuliInfoA);
    }
}