using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadSettings : MonoBehaviour
{
    private string _filePath;

    private void Awake()
    {
        _filePath = Application.persistentDataPath + "/save.settings";
        Debug.Log("Путь настроек " + _filePath);
    }

    public void SaveSettings()
    {
        bool[] settings = new bool[4];
        settings[0] = UIButtonsHolder.MusicEnabled;
        settings[1] = UIButtonsHolder.GameVolumeEnabled;
        settings[2] = UIButtonsHolder.ShowLegend;
        settings[3] = UIButtonsHolder.ShowTutorial;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(_filePath, FileMode.Create))
        {
            binaryFormatter.Serialize(fileStream, settings);
        };
    }

    public bool[] LoadSettings()
    {
        if (File.Exists(_filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Open))
            {
                bool[] settings = (bool[])binaryFormatter.Deserialize(fileStream);
                return settings;
            };
        }
        else
        {
            return null;
        }
    }
}
