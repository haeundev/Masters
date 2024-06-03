using System.Collections.Generic;
using DataTables;
using LiveLarson.Util;
using UnityEngine;

public class StimuliObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> foods;

    private Stimuli _info;
    private string _answer;
    public Stimuli Info => _info;
    private StimuliTrigger _stimuliTrigger;
    private FoodItem _food;

    private void Awake()
    {
        var food = Instantiate(foods.PeekRandom());
        food.transform.SetParent(transform);
        food.transform.localPosition = Vector3.zero;

        _food = food.GetComponentInChildren<FoodItem>();
        _food.OnTrigger += AddScore;

        _stimuliTrigger = GetComponentInChildren<StimuliTrigger>();
        _stimuliTrigger.OnTrigger += TryPlayStimuli;
    }

    private void AddScore()
    {
        GlobalInfo.Score++;
    }

    private void OnDestroy()
    {
        _stimuliTrigger.OnTrigger -= TryPlayStimuli;
        _food.OnTrigger -= AddScore;
    }

    public void SetInfo(Stimuli stimuliInfo, string answer)
    {
        _info = stimuliInfo;
        _answer = answer;
        gameObject.name = $"{_info.Contrast}: answer ==> {answer}";
    }

    private void TryPlayStimuli()
    {
        if (GlobalInfo.IsStimuliAnswered == false)
        {
            if (GlobalInfo.IsFirstStimuli)
            {
                GlobalInfo.IsFirstStimuli = false;
                PlayStimuli();
                Debug.Log("First stimuli");
                return;
            }
            
            PushAllStimuliForward();
        }
        else
        {
            PlayStimuli();
        }
    }
    
    private void PushAllStimuliForward()
    {
        GameEvents.TriggerPushAllStimuliForward();
    }

    private void PlayStimuli()
    {
        var soundObj = new GameObject("Sound");
        var audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>($"Audio/{_answer}");
        audioSource.Play();
        Destroy(soundObj, audioSource.clip.length);
        
        GlobalInfo.CurrentStimuli = _info;
        GlobalInfo.CurrentAnswer = _answer;
        
        GameEvents.TriggerStimuli(gameObject, _info, _answer);
    }
}