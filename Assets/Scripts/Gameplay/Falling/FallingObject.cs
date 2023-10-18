using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class FallingObject : MonoBehaviour
{
    private InGameScore _scoreObj;
    private GlobalTimer _currentGameTime;
    private Rigidbody2D _rb;

    private HoldButton[] _buttons;
    private HoldButton _leftButton;
    private HoldButton _rightButton;


    [Header("Speed")]
    [SerializeField, Tooltip("Максимальная скорость падения"), Min(0f)]
    private float _maxSpeed;

    public float MaxSpeed
    {
        get { return _maxSpeed; }
        set { if (value >= 0) _maxSpeed = value; }
    }

    [SerializeField, Tooltip("Скорость падения"), Min(0f)]
    private float _fallSpeed;

    public float FallSpeed
    {
        get { return _fallSpeed; }
        set
        {
            if (value > 0)
            {
                if (value > _maxSpeed)
                {
                    _fallSpeed = _maxSpeed;
                }
                else
                {
                    _fallSpeed = value;
                }
            }
        }
    }

    [SerializeField, Tooltip("Время (сек), спустя которое полностью текущая скорость + SpeedBooster"), Min(0f)]
    private float _secondsToFullSpeedUp;

    public float SecondsToFullSpeedUp
    {
        get { return _secondsToFullSpeedUp; }
        set { if (value >= 0f) _secondsToFullSpeedUp = value; }
    }

    [SerializeField, Tooltip("Число на которое увеличится скорость относительно времени"), Min(0f)]
    private float _speedUp;

    public float SpeedUp
    {
        get { return _speedUp; }
        set { if (value >= 0f) _speedUp = value; }
    }


    [SerializeField, Tooltip("Горизонтальная скорость движения"), Min(0)]
    private float _leftRightSpeed;

    public float LeftRightSpeed
    {
        get { return _leftRightSpeed; }
        set { if (value >= 0) _leftRightSpeed = value; }
    }

    [Space, Header("Destroy")]
    [SerializeField, Tooltip("Можно ли разрушить объект при клике в любое место")]
    private bool _destroyable = false;

    public bool Destroyable
    {
        get { return _destroyable; }
    }

    private bool _isTriggered = false;

    protected FallingObject(float spd, float maxSpeed)
    {
        FallSpeed = spd;
        MaxSpeed = maxSpeed;
    }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _scoreObj = FindObjectOfType<InGameScore>();
        _currentGameTime = FindObjectOfType<GlobalTimer>();
        if (DeviceType.Device == "mobile")
        {
            _buttons = FindObjectsOfType<HoldButton>();
            if (_buttons.Length == 2)
            {
                if (_buttons[0].name == "Left")
                {
                    _leftButton = _buttons[0];
                    _rightButton = _buttons[1];
                }
                else
                {
                    _leftButton = _buttons[1];
                    _rightButton = _buttons[0];
                }
            }
        }

        StartSpeedBoost();
    }

    protected virtual void OnMouseDown()
    {
        if (Destroyable && Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            DestroyByClick();
        }
    }

    protected virtual void Update()
    {
        if (DeviceType.Device == "mobile")
        {
            if (LeftRightSpeed > 0 && _leftButton && _rightButton)
            {
                MoveByButtonsMobile();
            }
        }
        else
        {
            if (LeftRightSpeed > 0)
            {
                MoveByButtonsPC();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        Fall();
    }

    protected virtual void Fall()
    {
        _rb.AddForce(new Vector2(0f, -FallSpeed));
    }

    // Увеличение начальной скорости, относительно времени
    protected virtual void StartSpeedBoost()
    {
        FallSpeed += SpeedUp * Mathf.Round(_currentGameTime.CurrentGameTime) / Mathf.Round(SecondsToFullSpeedUp);
    }

    protected virtual void MoveByButtonsMobile()
    {
        if (LeftRightSpeed > 0)
        {
            if (_leftButton.IsHolding)
            {
                MoveLeft();
            }
            else if (_rightButton.IsHolding)
            {
                MoveRight();
            }
        }
    }

#if !UNITY_ANDROID
    protected virtual void MoveByButtonsPC()
    {
        if (LeftRightSpeed > 0)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                MoveRight();
            }
        }
    }
#endif

    protected virtual void MoveLeft()
    {
        _rb.AddForce(new Vector2(LeftRightSpeed * -1, 0f));
    }

    protected virtual void MoveRight()
    {
        _rb.AddForce(new Vector2(LeftRightSpeed * 1, 0f));
    }

    // ВЗАИМОДЕЙСТВИЕ
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bird>() && !_isTriggered)
        {
            OnTriggerBird(other);
        }
        else
        {
            OnTriggerOther(other);
        }
    }

    protected virtual void OnTriggerBird(Collider2D other)
    {
        _isTriggered = true;
        _scoreObj.GetComponent<InGameScore>().ChangeScoreOnBird(gameObject);
    }

    protected virtual void OnTriggerOther(Collider2D other)
    {
        if (other.GetComponent<OutScreenDestroy>())
        {
            _scoreObj.GetComponent<InGameScore>().ChangeScoreFallOut(gameObject);
            Dead();
        }
    }

    // ДОП. ВЗАИМОДЕЙСТВИЕ

    public virtual void DestroyByClick()
    {
        if (Destroyable && Time.timeScale > 0)
        {
            _scoreObj.GetComponent<InGameScore>().ChangeScoreClickDestroy(gameObject);
            Dead();
        }
    }

    public virtual void Dead()
    {
        Destroy(gameObject);
    }
}

