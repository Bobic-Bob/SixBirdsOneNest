using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllGamesResultsHolder : MonoBehaviour
{
    private TextMeshProUGUI _allGamesResultsText;
    private SaveLoadGameResultData _saveLoadData;

    [SerializeField]
    private List<ScoreDataToSerialize> _scoreDataToSerialize;

    public List<ScoreDataToSerialize> ScoreDataToSerialize
    {
        get { return _scoreDataToSerialize; }
        private set { _scoreDataToSerialize = value; }
    }

    private void Awake()
    {
        _saveLoadData = FindObjectOfType<SaveLoadGameResultData>();
        if (_saveLoadData == null)
        {
            throw new System.ArgumentNullException("Can't find SaveLoadData class");
        }
        else
        {
            ScoreDataToSerialize = _saveLoadData.LoadData();
            if (ScoreDataToSerialize == null)
            {
                ScoreDataToSerialize = new List<ScoreDataToSerialize>();
            }
        }
    }

    private void Start()
    {
        _allGamesResultsText = GetComponent<TextMeshProUGUI>();
        if (_allGamesResultsText)
        {
            ShowAllGamesResults();
        }
    }

    private ScoreDataToSerialize ConvertToScoreDataToSerialize(ScoreHolder data)
    {
        ScoreDataToSerialize convertedData = new ScoreDataToSerialize();
        convertedData.ScoreValue = data.ScoreValue;
        convertedData.BirdsFlewAway = data.BirdsFlewAway;
        return convertedData;
    }

    public ScoreDataToSerialize GetGameScore(int gameResultIndex)
    {
        if (gameResultIndex >= 0 && gameResultIndex < ScoreDataToSerialize.Count)
        {
            return ScoreDataToSerialize[gameResultIndex];
        }
        else
        {
            throw new System.ArgumentException("Incorrect game result ID");
        }
    }

    public void AddGameScore(ScoreHolder gameResult)
    {
        ScoreDataToSerialize.Add(ConvertToScoreDataToSerialize(gameResult));
        SaveLastGameScore();
    }

    private void SaveLastGameScore()
    {
        _saveLoadData.SaveData(ScoreDataToSerialize);
    }

    private void ShowAllGamesResults()
    {
        if (ScoreDataToSerialize == null || ScoreDataToSerialize.Count == 0)
        {
            if (Language.Instance.CurrentLanguage == "ru")
            {
                _allGamesResultsText.text = "Ќет сыгранных игр, чтобы отобразить рекорды";
            }
            else
            {
                _allGamesResultsText.text = "No played games to show records";
            }
        }
        else
        {
            int bestGameID = 0;
            for (int i = 1; i < ScoreDataToSerialize.Count; i++)
            {
                if (GetGameScore(bestGameID).ScoreValue < GetGameScore(i).ScoreValue)
                {
                    bestGameID = i;
                }
            }
            if (Language.Instance.CurrentLanguage == "ru")
            {
                _allGamesResultsText.text = $"<b>Ћ”„Ўјя »√–ј</b> \r\n—чет: {GetGameScore(bestGameID).ScoreValue}.\r\nѕтиц выросло: {GetGameScore(bestGameID).BirdsFlewAway}.\r\n\r\n";
                _allGamesResultsText.text += $"<b>ѕќ—Ћ≈ƒЌяя »√–ј</b> \r\n—чет: {GetGameScore(ScoreDataToSerialize.Count - 1).ScoreValue}.\r\nѕтиц выросло: {GetGameScore(ScoreDataToSerialize.Count - 1).BirdsFlewAway}.\r\n\r\n";
            }
            else
            {
                _allGamesResultsText.text = $"<b>BEST GAME</b> \r\nScore: {GetGameScore(bestGameID).ScoreValue}.\r\nBirds Grown: {GetGameScore(bestGameID).BirdsFlewAway}.\r\n\r\n";
                _allGamesResultsText.text += $"<b>LAST GAME</b> \r\nScore: {GetGameScore(ScoreDataToSerialize.Count - 1).ScoreValue}.\r\nBirds Grown: {GetGameScore(ScoreDataToSerialize.Count - 1).BirdsFlewAway}.\r\n\r\n";
            }
            ScoreDataToSerialize.RemoveRange(0, ScoreDataToSerialize.Count - 1);
        }
    }
}
