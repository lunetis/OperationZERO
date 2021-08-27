using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData
{
    public static string missionName = "";
    
    public static int timeBonusPerSecond = 0;
    public static int maxTime = 0;
    public static float elapsedTime = 0;
    public static int score = 0;
    public static int[] rankScoreCutoff;

    public static int GetTimeBonusScore()
    {
        return (maxTime - (int)elapsedTime) * timeBonusPerSecond;
    }
}
