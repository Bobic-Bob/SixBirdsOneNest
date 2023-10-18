using UnityEngine;

public class AdaptiveScalesPosition : MonoBehaviour
{
    void Start()
    {
        Vector3 newDownPos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.075f));
        newDownPos = new Vector3(newDownPos.x, newDownPos.y, 0);
        gameObject.transform.position = newDownPos;
    }
}
