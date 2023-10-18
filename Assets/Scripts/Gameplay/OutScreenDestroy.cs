using UnityEngine;

public class OutScreenDestroy : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bird>())
        {
            other.GetComponent<Bird>().Dead();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
