using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceType : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetDevice();

    public static string Device { get; private set; }

    public static DeviceType Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Device = GetDevice();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
