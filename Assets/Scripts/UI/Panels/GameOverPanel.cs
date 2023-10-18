using UnityEngine;

public class GameOverPanel : MonoBehaviour
{

    public void ShowGameOverMenuAfterTime(float time)
    {
        Invoke(nameof(ShowGameOverMenu), time);
    }

    private void ShowGameOverMenu()
    {
        gameObject.SetActive(true);
    }
}
