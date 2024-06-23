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
    private GameController _gameController;

    private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        
        var food = Instantiate(foods.PeekRandom());
        food.transform.SetParent(transform);
        food.transform.localPosition = Vector3.zero;

        _food = food.GetComponentInChildren<FoodItem>();

        _stimuliTrigger = GetComponentInChildren<StimuliTrigger>();
        _stimuliTrigger.OnTrigger += TryPlayStimuli;
    }
    
    private void OnDestroy()
    {
        _stimuliTrigger.OnTrigger -= TryPlayStimuli;
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
        
        _gameController.Arrows.SetActive(true);
        
        // sfx
        var sfx = new GameObject("SFX - PushAllStimuliForward").AddComponent<AudioSource>();
        sfx.clip = GetComponentInChildren<FoodItemCommon>().bounceOffSound;
        sfx.volume = .2f;
        sfx.Play();
        var duration = sfx.clip.length;
        Destroy(sfx.gameObject, duration);
    }

    private void PlayStimuli()
    {
        // for eat sfx
        _food.transform.SetParent(default, true);
        
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
        Debug.Log($"Playing word: {_answer}");
        
        var word = new GameObject("Target Word").AddComponent<AudioSource>();
        var path = $"Audio/Words/Target/{_speakerID}/{_answer}";
        word.clip = Resources.Load<AudioClip>(path);
        
        if (word.clip == null)
        {
            Debug.LogError($"Audio clip not found: {path}");
            yield break;
        }
        
        // wait for eat sfx to finish
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
        {
            word.volume = 0.63096f;
            word.Play();
            var duration = word.clip.length;
            GlobalInfo.StimuliPlayEndedTime = Time.time + duration;
            Destroy(word.gameObject, duration);
        }).AddTo(word);
    }

    private IEnumerator PlayWordWithSingleTalker()
    {
        var soundSource = NoiseController.GetSoundSourceTransform();

        var word = new GameObject("Target Word").AddComponent<AudioSource>();
        word.clip = Resources.Load<AudioClip>($"Audio/Words/Target/{_speakerID}/{_answer}");
        word.volume = 0.63096f;
        word.transform.position = soundSource.position;
        word.spatialize = false;
        word.spatialBlend = 0;
        word.minDistance = 100f;
        word.maxDistance = 1000f;

        var sentence = new GameObject("Single Talker Sentence").AddComponent<AudioSource>();
        sentence.transform.position = soundSource.position;
        sentence.volume = 1;
        sentence.spatialize = true;
        sentence.spatialBlend = 1;
        sentence.minDistance = 100f;
        sentence.maxDistance = 1000f;
        
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
        {
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
                GlobalInfo.StimuliPlayEndedTime = Time.time + word.clip.length;
                Destroy(word.gameObject, word.clip.length);
            }).AddTo(word);
        }).AddTo(sentence);

        yield break;
    }
}