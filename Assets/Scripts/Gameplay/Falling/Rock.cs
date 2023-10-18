using UnityEngine;

public class Rock : FallingObject
{
    protected Rock(float speed, float maxSpeed) : base(speed, maxSpeed) { }

    protected override void OnTriggerBird(Collider2D other)
    {
        base.OnTriggerBird(other);
        other.GetComponent<Bird>().FallDown();
        Dead();
    }

}
