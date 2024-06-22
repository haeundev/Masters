using UnityEngine;
using UnityEngine.UI;

public class FloatEffect : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Float = Animator.StringToHash("Float");
    private static FloatEffect Instance { get; set; }

    [SerializeField] private AudioClip sfxOnCorrect;
    [SerializeField] private AudioClip sfxOnWrong;
    [SerializeField] private Sprite spriteOnCorrect;
    [SerializeField] private Sprite spriteOnWrong;

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    public static void Play(bool isAnswer)
    {
        Instance._animator.SetTrigger(Float);

        var audioSource = Instance.gameObject.GetComponent<AudioSource>();
        audioSource.clip = isAnswer ? Instance.sfxOnCorrect : Instance.sfxOnWrong;
        audioSource.volume = isAnswer ? 0.07f : 0.2f;
        audioSource.Play();

        var image = Instance.gameObject.GetComponent<Image>();
        image.sprite = isAnswer ? Instance.spriteOnCorrect : Instance.spriteOnWrong;
    }
}