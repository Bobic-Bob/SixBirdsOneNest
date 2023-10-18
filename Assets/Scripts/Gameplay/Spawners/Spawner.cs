using UnityEngine;

// ЗАРАНДОМИТЬ НА ШАНС (пока работает, хоть не совсем как хотелось бы, а именно писать проценты, а не расчитывать спавн относительно друг друга)

[System.Serializable]
public struct SpawnData
{
    [SerializeField]
    private GameObject _prefab;

    public GameObject Prefab
    {
        get
        {
            if (_prefab != null)
            {
                return _prefab;
            }
            else
            {
                throw new System.ArgumentException("No prefab to get");
            }
        }
        set
        {
            if
                (value != null)
            {
                _prefab = value;
            }
            else
            {
                throw new System.ArgumentException("No prefab to set");
            }
        }
    }

    [SerializeField, Range(0f, 1f), Tooltip("Шанс спавна относительно других объектов 1 = 100%")]
    private float _spawnChance;

    public float SpawnChance
    {
        get { return _spawnChance; }
        set
        {
            if (value >= 0 && value <= 1)
            {
                _spawnChance = value;
            }
            else
            {
                _spawnChance = 0;
            }
        }
    }
}

// ДОБАВИТЬ ОГРАНИЧЕНИЯ SPAWNDATA[]
public abstract class Spawner : MonoBehaviour
{
    public SpawnData[] spawnDatas;

    [Space, SerializeField, Min(0f)]
    private float _respawnTime;

    public float RespawnTime
    {
        get { return _respawnTime; }
        set
        {
            if (value >= 0)
            {
                _respawnTime = value;
            }
            else
            {
                _respawnTime = 0;
            }
        }
    }

    [SerializeField]
    private float _currentRespawnTime;

    public float CurrentRespawnTime
    {
        get { return _currentRespawnTime; }
        set
        {
            if (value > 0)
            {
                _currentRespawnTime = value;
            }
            else
            {
                _currentRespawnTime = 0;
            }
        }
    }

    [SerializeField, Tooltip("Если true, то значение нельзя изменить и спавнер больше не будет создавать объекты")]
    private bool _cantSpawnAnymore = false;

    public bool CantSpawnAnymore
    {
        get { return _cantSpawnAnymore; }
        set
        {
            if (value) _cantSpawnAnymore = value;
            else throw new System.ArgumentException("_cantSpawnAnymore can't be false after true");
        }
    }

    protected Spawner(float respTime, float curRespTime)
    {
        RespawnTime = respTime;
        CurrentRespawnTime = curRespTime;
    }

    protected virtual void Start()
    {
        CurrentRespawnTime = RespawnTime;
    }

    protected virtual void FixedUpdate()
    {
        if (!CantSpawnAnymore) Respawn();
    }

    // Респавн по истечении времени и если нет ребенка
    protected virtual void Respawn()
    {
        if (CanSpawn())
        {
            SpawnMoveRandom();
            CurrentRespawnTime = RespawnTime;
        }
        else if (CurrentRespawnTime > 0)
        {
            CurrentRespawnTime -= Time.deltaTime;
        }
        // else есть ребенок и перезарядка прошла
    }

    // Проверка на наличие дочернего объекта
    // TRUE - ЕСТЬ
    // FALSE - НЕТ
    protected virtual bool HasChild()
    {
        if (gameObject.transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Если нет дочерних и прошёл откат - true
    protected virtual bool CanSpawn()
    {
        if (!HasChild() && CurrentRespawnTime <= 0 && Time.timeScale > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Спавн и смещение рандомного префаба (разбить на разные методы 1) рандом + выбор 2) смещение
    protected virtual void SpawnMoveRandom()
    {
        // Генерируем случайное число для определения префаба
        float randomValue = Random.value;

        float cumulativeProbability = 0f;
        foreach (SpawnData data in spawnDatas)
        {
            cumulativeProbability += data.SpawnChance;
            // Если сгенерированное значение меньше текущего кумулятивного шанса спавна,
            // то выбираем соответствующий префаб и спавним его
            if (randomValue < cumulativeProbability)
            {
                // случайное смещение относителньо ширины экрана
                Vector3 rangeToSpawn = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth * 0.9f, 0));
                Vector3 spawnPosition = new Vector3(Random.Range(-rangeToSpawn.x, rangeToSpawn.x), gameObject.transform.position.y, 0);
                // создание
                GameObject newObject = Instantiate(data.Prefab, spawnPosition, gameObject.transform.rotation);
                newObject.transform.SetParent(gameObject.transform);
                break;
            }
        }
    }

    protected virtual void SpawnFirstPrefabOnly()
    {
        foreach (SpawnData data in spawnDatas)
        {
            GameObject newObject = Instantiate(data.Prefab, gameObject.transform.position, gameObject.transform.rotation);
            newObject.transform.SetParent(gameObject.transform);
            break;
        }
    }

    public virtual void NewRespawnTime(float newTime)
    {
        RespawnTime = newTime;
        CurrentRespawnTime = newTime;
    }

    public virtual void DestroySpawner()
    {
        Destroy(gameObject);
    }
}
