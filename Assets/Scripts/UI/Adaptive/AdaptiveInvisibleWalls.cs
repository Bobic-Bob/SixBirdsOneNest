using UnityEngine;

public class AdaptiveInvisibleWalls : MonoBehaviour
{
    [SerializeField]
    private bool _isLeftWall;

    private void Start()
    {
        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight * 0.5f));
        if (_isLeftWall)
        {
            gameObject.transform.position = new Vector3(bounds.x * -1.18f, bounds.y);
        }
        else
        {
            gameObject.transform.position = new Vector3 (bounds.x * 1.18f, bounds.y);
        }
    }
}
