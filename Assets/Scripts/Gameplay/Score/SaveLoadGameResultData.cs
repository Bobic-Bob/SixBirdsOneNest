using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveLoadGameResultData : MonoBehaviour
{
    private string _filePath;

    private void Awake()
    {
        _filePath = Application.persistentDataPath + "/save.results";
        Debug.Log("Путь" + _filePath);
    }

    public void SaveData(List<ScoreDataToSerialize> gamesResults)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(_filePath, FileMode.Create))
        {
            binaryFormatter.Serialize(fileStream, gamesResults);
        };
    }

    public List<ScoreDataToSerialize> LoadData()
    {
        if (File.Exists(_filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Open))
            {
                List<ScoreDataToSerialize> gamesResults = (List<ScoreDataToSerialize>)binaryFormatter.Deserialize(fileStream);
                return gamesResults;
            };
        }
        else
        {
            return null;
        }
    }
}
