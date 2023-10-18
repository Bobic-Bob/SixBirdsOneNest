using TMPro;
using UnityEngine;

[System.Serializable]
public struct ScoreRules
{
    [SerializeField]
    private GameObject _objName;

    public GameObject ObjName
    {
        get
        {
            if (_objName != null)
            {
                return _objName;
            }
            else
            {
                throw new System.ArgumentException("No prefab to score");
            }
        }
        set { if (value != null) _objName = value; }
    }

    [SerializeField, Tooltip("—чЄт при соприкосновении с птицей")]
    private int _onBird;

    public int OnBird
    {
        get { return _onBird; }
        private set { if (value > 0) _onBird = value; }
    }

    [SerializeField, Tooltip("—чЄт при выпадении за экран")]
    private int _fallOut;

    public int FallOut
    {
        get { return _fallOut; }
        private set { if (value > 0) _fallOut = value; }
    }

    [SerializeField, Tooltip("—чЄт при уничтожении кликом")]
    private int _clickDestroy;

    public int ClickDestroy
    {
        get { return _clickDestroy; }
        private set { if (value > 0) _clickDestroy = value; }
    }
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class InGameScore : Score
{
    private TextMeshProUGUI _thisScoreText;

    [SerializeField]
    private ScoreRules[] scoreDatas;

    private ScoreHolder _scoreHolder;

    protected InGameScore(int scoreValue, int birdsFlewAway) : base(scoreValue, birdsFlewAway) { }

    private void Start()
    {
        _scoreHolder = FindObjectOfType<ScoreHolder>();
        if (_scoreHolder)
        {
            _thisScoreText = GetComponent<TextMeshProUGUI>();
            UpdateInGameScoreText();
        }
        else
        {
            throw new System.NullReferenceException("Can't find ScoreHolder");
        }
    }

    private void UpdateScoreValue(int num, bool isBirdFlyAway = false)
    {
        if (isBirdFlyAway)
        {
            _scoreHolder.BirdsFlewAway += num;
            BirdsFlewAway += num;
        }
        else
        {
            _scoreHolder.ScoreValue += num;
            ScoreValue += num;
        }
        UpdateInGameScoreText();
    }

    private void UpdateInGameScoreText()
    {
        if (Language.Instance.CurrentLanguage == "ru")
        {
            _thisScoreText.text = $"—чет:\r\n{ScoreValue}\r\nѕтиц выросло:\r\n{BirdsFlewAway}";
        }
        else
        {
            _thisScoreText.text = $"Score:\r\n{ScoreValue}\r\nBirds Grown:\r\n{BirdsFlewAway}";
        }
    }

    public void ChangeScoreOnBird(GameObject obj)
    {
        foreach (ScoreRules item in scoreDatas)
        {
            if (item.ObjName.name + "(Clone)" == obj.name)
            {
                UpdateScoreValue(item.OnBird);
                break;
            }
        }
    }

    public void ChangeScoreFallOut(GameObject obj)
    {
        foreach (ScoreRules item in scoreDatas)
        {
            if (item.ObjName.name + "(Clone)" == obj.name)
            {
                UpdateScoreValue(item.FallOut);
                break;
            }
        }
    }

    public void ChangeScoreClickDestroy(GameObject obj)
    {
        foreach (ScoreRules item in scoreDatas)
        {
            if (item.ObjName.name + "(Clone)" == obj.name)
            {
                UpdateScoreValue(item.ClickDestroy);
                break;
            }
        }
    }

    public void ChangeScoreBirdFlyAway()
    {
        UpdateScoreValue(1, true);
        UpdateScoreValue(70);
    }
}
