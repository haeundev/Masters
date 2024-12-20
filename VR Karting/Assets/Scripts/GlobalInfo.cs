using DataTables;

public static class GlobalInfo
{
    public static EnvironmentController.WeatherType WeatherType;
    public static EnvironmentController.TimeOfDay TimeOfDay;

    public static Stimuli CurrentStimuli;
    public static string CurrentAnswer;
    private static int score;
    public static bool IsStimuliAnswered = false;
    public static bool IsFirstStimuli = true;

    public static int Score
    {
        get => score;
        set
        {
            score = value;
            GameEvents.TriggerScoreUpdate(score);
        }
    }

    public static float StimuliPlayEndedTime { get; set; }
}