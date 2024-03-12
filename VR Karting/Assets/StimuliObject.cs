using System.Collections.Generic;
using DataTables;
using LiveLarson.Util;
using UnityEngine;

public class StimuliObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> foods;
    private bool _isFirstSet;

    private Stimuli _info;
    public Stimuli Info => _info;
    private StimuliTrigger _stimuliTrigger;

    private void Awake()
    {
        var food = Instantiate(foods.PeekRandom());
        food.transform.SetParent(transform);
        food.transform.localPosition = Vector3.zero;

        _stimuliTrigger = GetComponentInChildren<StimuliTrigger>();
        _stimuliTrigger.OnTrigger += PlayStimuli;
    }

    private void OnDestroy()
    {
        _stimuliTrigger.OnTrigger -= PlayStimuli;
    }

    public void SetInfo(Stimuli stimuliInfo, bool isFistSet)
    {
        _info = stimuliInfo;
        _isFirstSet = isFistSet;
        var word = _isFirstSet ? _info.A : _info.B;
        gameObject.name = $"{_info.Contrast} - {word}";
    }

    private void PlayStimuli()
    {
        var soundObj = new GameObject("Sound");
        var audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip =
            Resources.Load<AudioClip>(_isFirstSet ? $"Audio/{_info.A}" : $"Audio/{_info.B}");
        audioSource.Play();
        Destroy(soundObj, audioSource.clip.length);
        GameEvents.TriggerStimuli(gameObject, _info,
            _isFirstSet ? $"Audio/{_info.A}" : $"Audio/{_info.B}");
    }
}