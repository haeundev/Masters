using UnityEngine;

public class NoiseController : MonoBehaviour
{
    public NoiseMode noiseMode;

    [SerializeField] private GameObject femaleNPC;
    [SerializeField] private GameObject maleNPC;
    [SerializeField] private GameObject rain;
    [SerializeField] private GameObject snow;
    [SerializeField] public EnvironmentController environmentController;

    public static NoiseController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init(NoiseMode mode, bool isWeek2, string speaker)
    {
        noiseMode = mode;

        ResetToDefault();

        switch (noiseMode)
        {
            case NoiseMode.None:
                environmentController.SetEnvironment(EnvironmentController.WeatherType.Clear);
                break;

            case NoiseMode.SingleTalker:
                OnSingleTalker(speaker);
                environmentController.SetEnvironment(EnvironmentController.WeatherType.Clear);
                break;

            case NoiseMode.Environmental:
                // rain or snow, randomly
                if (Random.value > 0.5f)
                {
                    rain.SetActive(true);
                    environmentController.SetEnvironment(EnvironmentController.WeatherType.Rainy);
                }
                else
                {
                    snow.SetActive(true);
                    environmentController.SetEnvironment(EnvironmentController.WeatherType.Snowy);
                }
                break;
        }
    }

    private void ResetToDefault()
    {
        femaleNPC.SetActive(false);
        maleNPC.SetActive(false);
        rain.SetActive(false);
        snow.SetActive(false);
    }

    private void OnSingleTalker(string speaker)
    {
        if (speaker.StartsWith('F'))
            femaleNPC.SetActive(true);
        else
            maleNPC.SetActive(true);
    }
    
    public static Transform GetSoundSourceTransform()
    {
        if (Instance.femaleNPC.activeSelf)
            return Instance.femaleNPC.transform;
        if (Instance.maleNPC.activeSelf)
            return Instance.maleNPC.transform;
        else return KartController.Instance.transform;
    }
}