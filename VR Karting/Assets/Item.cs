using DataTables;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Collider _collider;
    private Stimuli _stimuliInfo;
    private bool _isFirstSet;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void SetInfo(Stimuli stimuliInfo, bool isFistSet)
    {
        _stimuliInfo = stimuliInfo;
        _isFirstSet = isFistSet;
        gameObject.name = _stimuliInfo.Contrast;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayStimuli();
    }

    private void PlayStimuli()
    {
        var soundObj = new GameObject("Sound");
        var audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip =
            Resources.Load<AudioClip>(_isFirstSet ? $"Audio/{_stimuliInfo.A}" : $"Audio/{_stimuliInfo.B}");
        audioSource.Play();
        Destroy(soundObj, audioSource.clip.length);
        GameEvents.TriggerStimuli(gameObject, _stimuliInfo, _isFirstSet ? $"Audio/{_stimuliInfo.A}" : $"Audio/{_stimuliInfo.B}");
    }
}