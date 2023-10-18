using System.Runtime.InteropServices;
using UnityEngine;

public class Language : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern string GetLanguage();

    public string CurrentLanguage { get; private set; }

    public static Language Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentLanguage = GetLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
