using System;
using UnityEngine;

public class FoodScoreUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
    private TextFileHandler _fileHandler;
    
    private GameController _gameController;
    
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
        _gameController = FindObjectOfType<GameController>();
        
        scoreText.text = "Ready to shop?";
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"{score} / 56";
    }

    private void OnApplicationQuit()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd HH-mm");
        _fileHandler = new TextFileHandler(Application.persistentDataPath, $"Drive Score p{_gameController.ParticipantID} {date}.txt");
        _fileHandler.WriteLine(scoreText.text);
        _fileHandler.CloseFile();
    }

    private void OnDestroy()
    {
        GameEvents.OnUserStarted -= OnUserStarted;
        GameEvents.OnScoreUpdate -= UpdateScore;
    }
}