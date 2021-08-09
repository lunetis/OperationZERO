using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    public void SetDescriptionText()
    {
        string text = "Current Language : ";

        if(GameSettings.languageSetting == GameSettings.Language.EN)
        {
            text += "ENGLISH";
        }
        else if(GameSettings.languageSetting == GameSettings.Language.KR)
        {
            text += "한국어";
        }
        MainMenuController.Instance?.SetDescriptionText(text);
    }

    void OnEnable()
    {
        SetDescriptionText();
    }

    void OnDisable()
    {
        MainMenuController.Instance?.SetDescriptionText("");
    }
}
