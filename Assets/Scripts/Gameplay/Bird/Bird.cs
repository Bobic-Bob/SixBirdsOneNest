using System.Collections;
using UnityEngine;

[System.Serializable]
public struct BirdStage
{
    [Space, Header("Stage settings")]
    [SerializeField]
    private Sprite _stageSprite;

    public Sprite StageSprite
    {
        get { return _stageSprite; }
        set
        {
            if (value != null)
            {
                _stageSprite = value;
            }
            else
            {
                throw new System.ArgumentException("No sprite at stage");
            }
        }
    }

    [SerializeField, Tooltip("Масса для перехода на следующую стадию"), Min(0f)]
    private float _massToStage;

    public float MassToStage
    {
        get { return _massToStage; }
        set { _massToStage = value >= 0f ? value : 0f; }
    }

    [SerializeField]
    private Sprite _stageDeadSprite;

    public Sprite StageDeadSprite
    {
        get { return _stageDeadSprite; }
        set
        {
            if (value != null)
            {
                _stageDeadSprite = value;
            }
            else
            {
                throw new System.ArgumentException("No dead sprite at stage");
            }
        }
    }

    [Space, Header("Sounds")]
    [SerializeField]
    private AudioClip _stageStartSound;

    public readonly AudioClip StageStartSound
    {
        get { return _stageStartSound; }
    }

    [SerializeField]
    private AudioClip _deadFallOutSound;

    public readonly AudioClip DeadFallOutSound
    {
        get { return _deadFallOutSound; }
    }

    [SerializeField]
    private AudioClip _deadByRockSound;

    public readonly AudioClip DeadByRockSound
    {
        get { return _deadByRockSound; }
    }
}

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Bird : MonoBehaviour
{
    // (ДОП умер навсегда) нельзя респавнить -> появляется надгробие или ещё что-то такое (в спавнере)

    private InGameScore _scoreObj;
    private SpriteRenderer _birdSpriteRenderer;
    private Animator _birdAnimator;
    private ScalesModel _scaleModel;
    private Rigidbody2D _birdRigidBody2D;
    private AudioSource _birdAudioSource;

    [Header("Stages"), SerializeField]
    private BirdStage[] birdStages;

    [Space, Header("Respawn rools")]
    [SerializeField, Min(0f)]
    private float _respawnTimeAfterDeath;


    public float RespawnTimeAfterDeath
    {
        get { return _respawnTimeAfterDeath; }
        set
        {
            if (value >= 0f)
            {
                _respawnTimeAfterDeath = value;
            }
            else
            {
                throw new System.ArgumentException("_respawnTimeAfterDeath can't be less 0");
            }
        }
    }

    [SerializeField, Min(0f)]
    private float _respawnTimeAfterGrown;

    public float RespawnTimeAfterGrown
    {
        get { return _respawnTimeAfterGrown; }
        set
        {
            if (value >= 0f)
            {
                _respawnTimeAfterGrown = value;
            }
            else
            {
                throw new System.ArgumentException("_respawnTimeAfterGrown can't be less 0");
            }
        }
    }

    [Space, Header("Grow rools")]
    [SerializeField, Tooltip("Может ли есть хорошие вещи")]
    private bool _canEatGoodThings;

    public bool CanEatGoodThings
    {
        get { return _canEatGoodThings; }
        private set { _canEatGoodThings = value; }
    }

    [SerializeField, Tooltip("Может ли есть плохие вещи")]
    private bool _canEatBadThings;

    public bool CanEatBadThings
    {
        get { return _canEatBadThings; }
        set { _canEatBadThings = value; }
    }

    [SerializeField, Tooltip("Увеличение спрайта по localscale"), Min(0f)]
    private float _growBy;

    public float GrowBy
    {
        get { return _growBy; }
        set
        {
            if (value >= 0) _growBy = value;
            else _growBy = 0f;
        }
    }

    [SerializeField, Tooltip("Время для перехода из первой стадии на вторую"), Min(0f)]
    private float _timeToGrowupSecondStage;

    public float TimeToGrowupSecondStage
    {
        get { return _timeToGrowupSecondStage; }
        set { if (value >= 0f) _timeToGrowupSecondStage = value; }
    }

    [Space, Header("Sounds")]
    [SerializeField]
    private AudioClip _poisonedSound;

    public AudioClip PoisonedSound
    {
        get { return _poisonedSound; }
    }

    [SerializeField]
    private AudioClip[] _eatSounds;

    public AudioClip[] EatSounds
    {
        get { return _eatSounds; }
    }

    private AudioSource _holderAudioSource;

    private void Start()
    {
        _scoreObj = FindObjectOfType<InGameScore>();
        _birdSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _birdAnimator = gameObject.GetComponent<Animator>();
        _scaleModel = FindObjectOfType<ScalesModel>();
        _birdRigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        _birdAudioSource = gameObject.GetComponent<AudioSource>();
        // МОЖЕТ БЫТЬ ВЫБРАН НЕ ТОТ ОБЪЕКТ И ПОЛОМАТЬ ЕМУ ЗВУК
        _holderAudioSource = FindFirstObjectByType<AudioSource>();
        if (!_holderAudioSource)
        {
            throw new System.Exception("Can't find any AudioSource object");
        }
        Growup();
    }

    private void NewRespawnTimeAfterDeadInParent()
    {
        if (gameObject.transform.parent)
        {
            GetComponentInParent<BirdSpawner>().NewRespawnTime(RespawnTimeAfterDeath);
        }
    }

    private void LoseParent()
    {
        gameObject.transform.parent = null;
    }

    // ИСПОЛЬЗУЕТСЯ В АНИМАЦИИ ПЕРЕХОДА СТАДИИ
    private void ChangeAllEatRools()
    {
        CanEatGoodThings = !CanEatGoodThings;
        CanEatBadThings = !CanEatBadThings;
    }

    // РОСТ
    private int CurrentStage()
    {
        for (int i = 0; i < birdStages.Length - 1; i++)
        {
            // first stage
            if (_birdRigidBody2D.mass <= birdStages[0].MassToStage)
            {
                return i;
            }
            // last stage
            else if (_birdRigidBody2D.mass >= birdStages[^1].MassToStage)
            {
                return i;
            }
            // medium stage
            else if (_birdRigidBody2D.mass >= birdStages[i].MassToStage && _birdRigidBody2D.mass < birdStages[i + 1].MassToStage)
            {
                return i;
            }
        }
        throw new System.ArgumentNullException("Can't identify stage");
    }

    // ПЕРЕДЕЛАТЬ ПОД НОВЫЙ МЕТОД ОПРЕДЕЛНИЯ СТАДИЙ
    public void Growup()
    {
        for (int i = 0; i < birdStages.Length - 1; i++)
        {
            if (_birdRigidBody2D.mass <= birdStages[0].MassToStage)
            {
                FirstStage(TimeToGrowupSecondStage);
                break;
            }
            else if (_birdRigidBody2D.mass >= birdStages[^1].MassToStage)
            {
                LastStage();
                break;
            }
            else if (_birdRigidBody2D.mass >= birdStages[i].MassToStage && _birdRigidBody2D.mass < birdStages[i + 1].MassToStage)
            {
                MediumStages(i);
            }
        }
    }

    private void Grown()
    {
        GetComponentInParent<BirdSpawner>().NewRespawnTime(RespawnTimeAfterGrown);
        GetComponent<Collider2D>().enabled = false;
    }

    public void SizeUp()
    {
        PlayRandomSound(_eatSounds);
        gameObject.transform.localScale += new Vector3(GrowBy, GrowBy, 0f);
    }

    // ОСОБЕННОСТИ СТАДИЙ: ПЕРВОЙ, ПРОМЕЖУТОЧНЫХ И ПОСЛЕДНЕЙ
    private void FirstStage(float time)
    {
        PlaySound(birdStages[0].StageStartSound);
        _birdRigidBody2D.mass = birdStages[0].MassToStage;
        UpdateScaleMass();
        CanEatBadThings = false;
        CanEatGoodThings = false;
        Invoke(nameof(FirstStageToSecond), time);
    }

    private void FirstStageToSecond()
    {
        PlaySound(birdStages[1].StageStartSound);
        _birdSpriteRenderer.sortingOrder++;
        _birdAudioSource.Play();
        _birdRigidBody2D.mass = birdStages[1].MassToStage;
        Growup();
    }

    private void MediumStages(int i)
    {
        PlaySound(birdStages[i].StageStartSound);
        _birdSpriteRenderer.sortingOrder++;
        _birdAnimator.SetInteger("birdLifeStage", i);
        _birdSpriteRenderer.sprite = birdStages[i].StageSprite;
        if (gameObject.GetComponent<CapsuleCollider2D>())
        {
            CapsuleCollider2D capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, capsuleCollider.size.y + 0.1f);
        }
        _scaleModel.MassUpdated();
    }

    private void LastStage()
    {
        PlaySound(birdStages[^1].StageStartSound, true);
        _birdSpriteRenderer.sortingOrder++;
        _birdSpriteRenderer.sortingOrder = Random.Range(_birdSpriteRenderer.sortingOrder, 31);
        CanEatGoodThings = false;
        CanEatBadThings = false;
        _birdSpriteRenderer.sprite = birdStages[^1].StageSprite;
        _birdAnimator.SetInteger("birdLifeStage", birdStages.Length - 1);
        Grown();
    }

    // СМЕРТЬ
    // FlyAway вызывается в аниматоре
    private void FlyAway()
    {
        _scoreObj.GetComponent<InGameScore>().ChangeScoreBirdFlyAway();
        Destroy(gameObject);
    }

    private void UpdateScaleMass()
    {
        _scaleModel.MassUpdated();
    }

    public void FallDown()
    {
        _birdAnimator.SetBool("birdDead", true);
        NewRespawnTimeAfterDeadInParent();
        LoseParent();
        UpdateScaleMass();
        float birdMass = gameObject.GetComponent<Rigidbody2D>().mass;
        for (int i = 0; i < birdStages.Length - 1; i++)
        {
            if (birdMass >= birdStages[i].MassToStage && birdMass < birdStages[i + 1].MassToStage)
            {
                PlaySound(birdStages[i].DeadByRockSound);
                gameObject.GetComponent<SpriteRenderer>().sprite = birdStages[i].StageDeadSprite;
            }
        }
        Destroy(gameObject.GetComponent<TargetJoint2D>());
    }

    public void Dead()
    {
        PlaySound(birdStages[CurrentStage()].DeadFallOutSound);
        _birdAnimator.SetBool("birdDead", true);
        NewRespawnTimeAfterDeadInParent();
        LoseParent();
        UpdateScaleMass();
        Destroy(gameObject, 2f);
    }

    // ДЕБАФФЫ
    public void Poisoned(float time)
    {
        PlaySound(_poisonedSound);
        StartCoroutine(PoisonedTime(time));
    }

    private IEnumerator PoisonedTime(float time)
    {
        CanEatGoodThings = false;
        _birdSpriteRenderer.color = new Color32(80, 255, 1, 255);
        yield return new WaitForSeconds(time);
        CanEatGoodThings = true;
        _birdSpriteRenderer.color = Color.white;
    }

    // ЗВУКИ
    private void PlaySound(AudioClip clip, bool destroyed = false)
    {
        if (clip && _holderAudioSource)
        {
            if (destroyed)
            {
                _holderAudioSource.clip = clip;
                _holderAudioSource.PlayOneShot(_holderAudioSource.clip);
            }
            else
            {
                _birdAudioSource.clip = clip;
                _birdAudioSource.PlayOneShot(_birdAudioSource.clip);
            }
        }
    }

    private void PlayRandomSound(AudioClip[] clips, bool destroyed = false)
    {
        if (clips.Length > 0)
        {
            if (destroyed)
            {
                _holderAudioSource.clip = clips[Random.Range(0, clips.Length)];
                _holderAudioSource.PlayOneShot(_holderAudioSource.clip);
            }
            else
            {
                _birdAudioSource.clip = clips[Random.Range(0, clips.Length)];
                _birdAudioSource.PlayOneShot(_birdAudioSource.clip);
            }
        }
        else
        {
            throw new System.ArgumentException("List of clips is empty");
        }
    }
}
