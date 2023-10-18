using UnityEngine;

public class ToxicWorm : FallingObject
{
    [Space, Header("Other")]
    [SerializeField, Min(0f)]
    private float _toxicTime;

    public float ToxicTime
    {
        get { return _toxicTime; }
        set
        {
            if (value >= 0f) _toxicTime = value;
            else _toxicTime = 0f;
        }
    }

    protected ToxicWorm(float speed, float maxSpeed) : base(speed, maxSpeed) { }

    protected override void OnTriggerBird(Collider2D other)
    {
        if (other.GetComponent<Bird>().CanEatBadThings)
        {
            base.OnTriggerBird(other);
            other.GetComponent<Bird>().StopAllCoroutines();
            other.GetComponent<Bird>().Poisoned(ToxicTime);
            Dead();
        }
    }
}
