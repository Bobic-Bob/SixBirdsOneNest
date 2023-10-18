using System.Linq;
using UnityEngine;

public class ScalesModel : MonoBehaviour
{
    [Header("Spawnpoints")]
    // массив всех спавнов птиц слева
    [SerializeField]
    private GameObject[] _leftBirdsSpawns;
    // масса птиц слева
    private float[] _leftBirdsMass;
    // массив всех спавнов птиц справа
    [SerializeField]
    private GameObject[] _rightBirdsSpawns;
    // масса птиц справа
    private float[] _rightBirdsMass;

    [Space, Header("Balance Settings")]
    // отклонение в градусах
    [SerializeField, Tooltip("Количество градусов на которое отклоняются весы, при разнице в 1 кг."), Min(0)]
    private float _degreeDeviation;
    // множитель веса
    [SerializeField, Tooltip("Коэффициент на который умножается вес объекта 1 == 100%"), Min(0)]
    private float masRate;
    // уменьшение коэффициента masRate с каждым шагом (следующим объектом)
    [SerializeField, Tooltip("Уменьшение коэффициента с каждым следующим объектом, кроме первого"), Min(0)]
    private float decrementor;
    [SerializeField, Tooltip("Плавность поворота"), Min(0)]
    private float smooth = 1;

    [Space, Header("Lose Settings")]
    [SerializeField, Tooltip("Меню, которое будет активироваться, после проигрыша")]
    private GameObject _gameOverMenu;
    [SerializeField, Tooltip("Меню, которое будет деактивироваться, после проигрыша")]
    private GameObject _gameUI;
    // градус проигрыша
    [SerializeField, Tooltip("Угол наклона влево"), Range(0, 180)]
    private float _leftDegree;

    public float LeftDegree
    {
        get { return _leftDegree; }
        set
        {
            if (value >= 0 && value <= 180)
            {
                _leftDegree = value;
            }
        }
    }

    [SerializeField, Tooltip("Угол наклона вправо"), Range(180, 360)]
    private float _rightDegree;

    public float RightDegree
    {
        get { return _rightDegree; }
        set { if (value >= 180 && value <= 360) _rightDegree = value; }
    }

    private bool _scalesCantMove;

    public bool ScalesCantMove
    {
        get { return _scalesCantMove; }
        private set { _scalesCantMove = value; }
    }
    
    private void Start()
    {
        RecalculateMass(_leftBirdsSpawns, _rightBirdsSpawns);
    }

    private void FixedUpdate()
    {
        // МБ СТОИТ УБРАТЬ ОТСЮДА ТК НАХ ЭТО КАЖДЫЙ ВСЁ ВРЕМЯ ПРОСЧИТЫВАТЬ?
        if (!ScalesCantMove)
        {
            SidesComparator(_leftBirdsMass, _rightBirdsMass);
            GameOverAngle(transform.rotation.eulerAngles.z);
        }
    }

    // ГЛАВНЫЕ ФУНКЦИИ
    // Получает (левый, правый) массивы родительских объектов (точек спавна) и возвращает массивы масс дочерних элементов
    private void RecalculateMass(GameObject[] leftParent, GameObject[] rightParent)
    {
        _leftBirdsMass = new float[leftParent.Length];
        _rightBirdsMass = new float[rightParent.Length];

        // Получение Rigidbody и запись массы в отдельный массив, если у объекта нет RB2D, то = 0
        // Левые птицы
        for (int i = 0; i < leftParent.Length; i++)
        {
            Rigidbody2D rigidbody = leftParent[i].transform.GetComponentInChildren<Rigidbody2D>();
            if (rigidbody != null)
            {
                _leftBirdsMass[i] = rigidbody.mass;
            }
            else
            {
                _leftBirdsMass[i] = 0f;
            }
        }

        // Правые птицы
        for (int i = 0; i < rightParent.Length; i++)
        {
            Rigidbody2D rigidbody = rightParent[i].transform.GetComponentInChildren<Rigidbody2D>();
            if (rigidbody != null)
            {
                _rightBirdsMass[i] = rigidbody.mass;
            }
            else
            {
                _rightBirdsMass[i] = 0f;
            }
        }
    }

    public void MassUpdated()
    {
        RecalculateMass(_leftBirdsSpawns, _rightBirdsSpawns);
    }

    // сравнивает ЛЕВЫЙ с ПРАВЫМ массивом и наклонять в зависимости от разницы в кг
    private void SidesComparator(float[] leftMas, float[] rightMas)
    {
        if (leftMas.Sum() == 0 || rightMas.Sum() == 0)
        {
            GameOver(_leftBirdsSpawns, _rightBirdsSpawns);
        }
        else
        {
            if (leftMas.Sum() == rightMas.Sum())
            {
                Quaternion a = gameObject.transform.rotation;
                Quaternion b = Quaternion.identity;
                gameObject.transform.rotation = Quaternion.Slerp(a, b, smooth * Time.deltaTime);
            }
            else
            {
                Quaternion a = gameObject.transform.rotation;
                float z = MasRater(leftMas, rightMas, decrementor, masRate) * _degreeDeviation;
                Quaternion b = Quaternion.Euler(0, 0, z);
                gameObject.transform.rotation = Quaternion.Slerp(a, b, smooth * Time.deltaTime);
            }
        }
    }

    // НАПИСАТЬ ФУНКЦИИ ПОВОРОТА ОБЪЕКТА С ОГРАНИЧЕНИЯМИ
    // ВПРАВО 300 (60) \ 315 (45)
    // ВЛЕВО 60 (60) \ 45 (45)
    // 0 -> 180 - поворот влево
    // 0 -> (-180) - вправо
    // -180 по факту 360 - 180
    // Проверка достижения угла проигрыша
    private void GameOverAngle(float angle)
    {
        if (angle >= LeftDegree && angle < 180 || angle <= _rightDegree && angle > 180)
        {
            GameOver(_leftBirdsSpawns, _rightBirdsSpawns);
        }
    }

    // МЕТОД ПЕРЕГРУЖЕН И БЕРЁТ НА СЕБЯ ОТВЕТСТВЕННОСТЬ ПТИЦЫ, РАЗГРУЗИ ЭТО
    // Падение птиц, запрет респавна и движения весов СДЕЛАТЬ ВЫЗОВ МЕНЮ
    private void GameOver(GameObject[] leftSpawners, GameObject[] rightSpawners)
    {
        if (_gameOverMenu && _gameUI)
        {
            _gameOverMenu.GetComponent<GameOverPanel>().ShowGameOverMenuAfterTime(2f);
            _gameUI.SetActive(false);
        }
        else
        {
            throw new System.ArgumentException("Menu wasn't found");
        }
        Debug.Log("SHIT");
        // ВСРАТО МБ ПОТОМ В ОТДЕЛЬНУЮ ПЕРЕМЕННУЮ ЭТИ 2 СТРОЧКИ
        // отключение респавна объектов
        if (FindAnyObjectByType<FallingSpawner>())
        {
            FindAnyObjectByType<FallingSpawner>().CantSpawnAnymore = true;
            FindAnyObjectByType<FallingSpawner>().DestroySpawner();
        }
        // переключение свойств на весах и птицах, запрет респавна
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        for (int i = 0; i < leftSpawners.Length; i++)
        {
            leftSpawners[i].GetComponent<BirdSpawner>().CantSpawnAnymore = true;
            if (leftSpawners[i].transform.childCount > 0)
            {
                leftSpawners[i].GetComponentInChildren<Rigidbody2D>().freezeRotation = false;
                leftSpawners[i].transform.GetComponentInChildren<Collider2D>().isTrigger = false;
                Destroy(leftSpawners[i].transform.GetComponentInChildren<TargetJoint2D>());
            }
        }
        for (int i = 0; i < rightSpawners.Length; i++)
        {
            rightSpawners[i].GetComponent<BirdSpawner>().CantSpawnAnymore = true;
            if (rightSpawners[i].transform.childCount > 0)
            {
                rightSpawners[i].GetComponentInChildren<Rigidbody2D>().freezeRotation = false;
                rightSpawners[i].transform.GetComponentInChildren<Collider2D>().isTrigger = false;
                Destroy(rightSpawners[i].transform.GetComponentInChildren<TargetJoint2D>());
            }
        }
        ScalesCantMove = true;
    }

    // вычисление разницы веса сторон
    // + -> левая стороная больше
    // - -> правая сторона больше
    private float MasDifference(float leftMas, float rightMas)
    {
        float difference = leftMas - rightMas;
        return difference;
    }

    // ДОПОЛНИТЕЛЬНЫЕ ФУНКЦИИ
    // вычисление разницы веса с учётом близости объекта к центру (чем ближе к центру, тем менее значим коэффициент)
    private float MasRater(float[] leftMas, float[] rightMas, float decremLeft, float masRate)
    {
        float[] newLeftMas;
        float[] newRightMas;
        newLeftMas = new float[leftMas.Length];
        newRightMas = new float[rightMas.Length];
        // сохраняет параметр декремента
        float decremParam = decremLeft;
        // декремент для правой стороны тк там в обратную сторону мб
        float decremRight = decremLeft;
        // вычисление левой массы с учётом коэффициента
        // первый элемент без декремента
        newLeftMas[0] = leftMas[0] * masRate;
        for (int i = 1; i < leftMas.Length; i++)
        {
            newLeftMas[i] = leftMas[i] * (masRate - decremLeft);
            decremLeft += decremParam;
        }
        // последний элемент без декремента
        newRightMas[rightMas.Length - 1] = rightMas[rightMas.Length - 1] * masRate;
        // вычисление правой массы с учётом коэффициента
        for (int i = rightMas.Length - 2; i >= 0; i--)
        {
            newRightMas[i] = rightMas[i] * (masRate - decremRight);
            decremRight += decremParam;
        }
        return MasDifference(newLeftMas.Sum(), newRightMas.Sum());
    }
}
