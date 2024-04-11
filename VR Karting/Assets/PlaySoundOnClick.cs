using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnClick : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AudioClip audioClip;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        button.onClick.AddListener(PlaySound);
    }
    
    private void PlaySound()
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}