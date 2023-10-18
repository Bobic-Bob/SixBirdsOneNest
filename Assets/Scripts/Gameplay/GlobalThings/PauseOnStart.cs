using UnityEngine;

public class PauseOnStart : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0f;
    }
}
