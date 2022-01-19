using UnityEngine;
using TMPro;

public class UILevelScore : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _levelText;
    private CanvasGroup _canvasGroup;
    private const string SCORE_TEXT_TEMPLATE = "{0} pts";
    
    void Start()
    {
        _scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        ArkanoidEvent.OnScoreUpdatedEvent += OnScoreUpdated;
         ArkanoidEvent.OnGameStartEvent += OnGameStart;
        ArkanoidEvent.OnGameOverEvent += OnGameOver;

    }   
    

    private void OnDestroy()
    {
        ArkanoidEvent.OnScoreUpdatedEvent -= OnScoreUpdated;
        ArkanoidEvent.OnGameStartEvent -= OnGameStart;
        ArkanoidEvent.OnGameOverEvent -= OnGameOver;
    }
    
    
    
    private void OnScoreUpdated(int score, int totalScore)
    {
        _scoreText.text = string.Format(SCORE_TEXT_TEMPLATE, totalScore);
    }

    private void OnGameStart()
    {
        _canvasGroup.alpha = 1;
    }
   
    private void OnGameOver()
    {
        _canvasGroup.alpha = 0;
    }
}
