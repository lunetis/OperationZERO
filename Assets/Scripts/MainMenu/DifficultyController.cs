using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    public void SetDescriptionText()
    {
        
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
