using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class MissionData : MonoBehaviour
{
    [SerializeField]
    TextAsset difficultyXMLFile;

    public static XmlDocument difficultyXMLData;

    void Awake()
    {
        difficultyXMLData = new XmlDocument();
        difficultyXMLData.LoadXml(difficultyXMLFile.text);
    }

    public static string GetDifficultyData(string key)
    {
        if(difficultyXMLData == null) return "";

        string difficultyKey = "data/difficulty/";
        switch(GameSettings.difficultySetting)
        {
            case GameSettings.Difficulty.EASY:
                difficultyKey += "easy/";
                break;
                
            case GameSettings.Difficulty.NORMAL:
                difficultyKey += "normal/";
                break;
                
            case GameSettings.Difficulty.HARD:
                difficultyKey += "hard/";
                break;
                
            case GameSettings.Difficulty.ACE:
                difficultyKey += "ace/";
                break;
        }

        return difficultyXMLData.SelectSingleNode(difficultyKey + key).InnerText;
    }

    public static float GetFloatFromDifficultyXML(string key, float defaultValue = 1.0f)
    {
        string text = GetDifficultyData(key);
        float value;
        if(text != "")
        {
            float.TryParse(text, out value);
        }
        else
        {
            return defaultValue;
        }

        return value;
    }
}
