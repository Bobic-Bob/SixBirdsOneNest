[System.Serializable]
public struct ScoreDataToSerialize
{
    private int _score;

    public int ScoreValue
    {
        get { return _score; }
        set { _score = value; }
    }

    private int _birdsFlyAway;

    public int BirdsFlewAway
    {
        get { return _birdsFlyAway; }
        set { if (value >= 0) _birdsFlyAway = value; }
    }
}
