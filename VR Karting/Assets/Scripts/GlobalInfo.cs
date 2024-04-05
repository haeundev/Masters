using DataTables;

public static class GlobalInfo
{
    public static EnvironmentController.WeatherType WeatherType;
    public static EnvironmentController.TimeOfDay TimeOfDay;

    public static Stimuli CurrentStimuli;
    public static string CurrentAnswer;
    public static int Score { get; set; }
}