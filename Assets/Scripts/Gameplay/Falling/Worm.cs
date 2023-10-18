using UnityEngine;

public class Worm : FallingObject
{
    [Space, Header("Other")]
    [SerializeField, Min(0)]
    private int _minMass;

    private int MinMass
    {
        get { return _minMass; }
        set { if (value >= 0 && value <= _maxMass) _minMass = value; }
    }

    [SerializeField, Min(0)]
    private int _maxMass;

    private int MaxMass
    {
        get { return _maxMass; }
        set { if (value >= 0 && value >= _minMass) _maxMass = value; }
    }

    [SerializeField]
    private int _mass;

    private int Mass
    {
        get { return _mass; }
        set
        {
            if (value >= 0)
            {
                _mass = value;
            }
            else
            {
                _mass = 0;
            }
        }
    }

    protected Worm(float speed, float maxSpeed) : base(speed, maxSpeed) { }

    protected override void Start()
    {
        Mass = Random.Range(MinMass, MaxMass + 1);
        base.Start();
    }

    protected override void OnTriggerBird(Collider2D other)
    {
        if (other.GetComponent<Bird>().CanEatGoodThings)
        {
            base.OnTriggerBird(other);
            other.transform.GetComponent<Rigidbody2D>().mass += Mass;
            other.GetComponent<Bird>().SizeUp();
            other.GetComponent<Bird>().Growup();
            Dead();
        }
    }
}
