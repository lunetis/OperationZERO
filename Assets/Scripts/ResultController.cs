using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI missionNameText;

    [SerializeField]
    TextMeshProUGUI elapsedTimeText;
    [SerializeField]
    TextMeshProUGUI timeBonusText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI totalScoreText;
    [SerializeField]
    TextMeshProUGUI rankText;

    char GetRank(int totalScore)
    {
        if(totalScore > ResultData.rankScoreCutoff[0])      return 'S';
        else if(totalScore > ResultData.rankScoreCutoff[1]) return 'A';
        else if(totalScore > ResultData.rankScoreCutoff[2]) return 'B';
        else if(totalScore > ResultData.rankScoreCutoff[3]) return 'C';
        return 'D';
    }
    
    void Start()
    {
        missionNameText.text = string.Format("[ {0} ]", ResultData.missionName);

        int min = ResultData.elapsedTime / 60;
        int sec = ResultData.elapsedTime % 60;
        elapsedTimeText.text = string.Format("{0:00}:{1:00}", min, sec);

        timeBonusText.text = string.Format("{0:n0}", ResultData.GetTimeBonusScore());
        scoreText.text = string.Format("{0:n0}", ResultData.score);

        int totalScore = (ResultData.GetTimeBonusScore() + ResultData.score);
        totalScoreText.text = string.Format("{0:n0}", totalScore);

        rankText.text = GetRank(totalScore).ToString();
    }
}
