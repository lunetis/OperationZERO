using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum Language
    {
        EN,
        KR
    }
    public enum Difficulty
    {
        EASY,
        NORMAL,
        HARD,
        ACE
    }
    
    public static Language languageSetting = Language.EN;
    public static Difficulty difficultySetting = Difficulty.NORMAL;
}
