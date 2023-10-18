using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameOverScore : Score
{
    private TextMeshProUGUI _finalScoreText;
    private ScoreHolder _scoreHolder;
    private AllGamesResultsHolder _totalGamesScores;

    protected GameOverScore(int scoreValue, int birdsFlewAway) : base(scoreValue, birdsFlewAway) { }

    private void Start()
    {
        _finalScoreText = GetComponent<TextMeshProUGUI>();
        _scoreHolder = FindObjectOfType<ScoreHolder>();
        if (_scoreHolder)
        {
            ScoreValue = _scoreHolder.ScoreValue;
            BirdsFlewAway = _scoreHolder.BirdsFlewAway;
            UpdateGameOverScoreText();
        }
        else
        {
            throw new System.NullReferenceException("Can't find ScoreHolder");
        }
        _totalGamesScores = FindFirstObjectByType<AllGamesResultsHolder>();
        if (_totalGamesScores)
        {
            ScoreTransmission();
        }
        else
        {
            throw new System.NullReferenceException("Can't find TotalGamesScores");
        }
    }

    private void UpdateGameOverScoreText()
    {
        if (Language.Instance.CurrentLanguage == "ru")
        {
            _finalScoreText.text = $"Итоговый счет:\r\n{ScoreValue}\r\nВсего птиц выросло:\r\n{BirdsFlewAway}";
        }
        else
        {
            _finalScoreText.text = $"Total Score:\r\n{ScoreValue}\r\nTotal Birds Grown:\r\n{BirdsFlewAway}";
        }
    }

    private void ScoreTransmission()
    {
        _totalGamesScores.AddGameScore(_scoreHolder);
    }
}
