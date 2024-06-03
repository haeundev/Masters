using System;
using System.Collections;
using System.Collections.Generic;
using DataTables;
using LiveLarson.Util;
using UniRx;
using UnityEngine;
using Observable = UniRx.Observable;

public class StimuliObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> foods;

    private Stimuli _info;
    private string _answer;
    public Stimuli Info => _info;
    private StimuliTrigger _stimuliTrigger;
    private FoodItem _food;
    private int _sessionID;
    private NoiseMode _noiseMode;
    private string _speakerID;

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

    public void SetInfo(Stimuli stimuliInfo, int sessionID, string speakerID, NoiseMode noiseMode, string answer)
    {
        _info = stimuliInfo;
        _answer = answer;
        _sessionID = sessionID;
        _speakerID = speakerID;
        _noiseMode = noiseMode;
        
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
        GlobalInfo.CurrentStimuli = _info;
        GlobalInfo.CurrentAnswer = _answer;

        GameEvents.TriggerStimuli(gameObject, _info, _answer);

        switch (_noiseMode)
        {
            case NoiseMode.SingleTalker:
                StartCoroutine(PlayWordWithSingleTalker());
                break;
            
            default:
                StartCoroutine(PlayWord());
                break;
        }
    }

    private IEnumerator PlayWord()
    {
        var word = new GameObject("Target Word").AddComponent<AudioSource>();
        word.clip = Resources.Load<AudioClip>($"Audio/Words/Target/{_speakerID}/{_answer}");
        word.Play();
        Destroy(word.gameObject, word.clip.length);
        
        yield break;
    }

    private IEnumerator PlayWordWithSingleTalker()
    {
        var soundSource = NoiseController.GetSoundSourceTransform();

        var word = new GameObject("Target Word").AddComponent<AudioSource>();
        word.clip = Resources.Load<AudioClip>($"Audio/Words/Target/{_speakerID}/{_answer}");
        word.transform.position = soundSource.position;
        word.spatialize = false;
        word.spatialBlend = 0;
        word.minDistance = 100f;
        word.maxDistance = 1000f;

        var sentence = new GameObject("Single Talker Sentence").AddComponent<AudioSource>();
        sentence.transform.position = soundSource.position;
        sentence.spatialize = true;
        sentence.spatialBlend = 1;
        sentence.minDistance = 100f;
        sentence.maxDistance = 1000f;
        
        var path = $"Audio/Sentences/{_speakerID}/";
        // choose random sentence from directory
        var sentenceAudioFiles = Resources.LoadAll<AudioClip>(path);
        sentence.clip = sentenceAudioFiles.PeekRandom();
        sentence.Play();
        Destroy(sentence.gameObject, sentence.clip.length);

        var halfDuration = sentence.clip.length / 3;
        Observable.Timer(TimeSpan.FromSeconds(halfDuration)).Subscribe(_ =>
        {
            word.Play();
            Destroy(word.gameObject, word.clip.length);
        }).AddTo(word);

        yield break;
    }
}