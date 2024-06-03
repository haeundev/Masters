using UnityEngine;

public class NoiseController : MonoBehaviour
{
    public NoiseMode noiseMode;

    [SerializeField] private GameObject femaleNPC;
    [SerializeField] private GameObject maleNPC;

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
                break;

            case NoiseMode.SingleTalker:
                OnSingleTalker(speaker);
                break;

            case NoiseMode.Environmental:

                break;
        }
    }

    private void ResetToDefault()
    {
        femaleNPC.SetActive(false);
        maleNPC.SetActive(false);
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