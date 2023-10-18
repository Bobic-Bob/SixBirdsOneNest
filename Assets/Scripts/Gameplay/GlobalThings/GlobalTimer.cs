using UnityEngine;

public class GlobalTimer : MonoBehaviour
{
    [SerializeField]
    private float _currentGameTime;

    public float CurrentGameTime
    {
        get { return _currentGameTime; }
        private set { if (value > 0) _currentGameTime = value; }
    }

    private void FixedUpdate()
    {
        CurrentGameTime += Time.fixedDeltaTime;
    }
}
