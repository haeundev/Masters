using UnityEngine;

public class FoodScoreUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
    private void Awake()
    {
        GameEvents.OnUserStarted += OnUserStarted;
        GameEvents.OnScoreUpdate += UpdateScore;
    }

    private void OnUserStarted()
    {
        scoreText.text = "0 / 56";
    }

    private void Start()
    {
        scoreText.text = "Ready to shop?";
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"{score} / 56";
    }

    private void OnDestroy()
    {
        GameEvents.OnUserStarted -= OnUserStarted;
        GameEvents.OnScoreUpdate -= UpdateScore;
    }
}