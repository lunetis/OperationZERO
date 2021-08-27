using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

    void GoToMainMenu()
    {
        InputAction submitAction = MainMenuController.PlayerInput.actions.FindAction("Submit");
        submitAction.started -= Confirm;
        InputAction backAction = MainMenuController.PlayerInput.actions.FindAction("Back");
        backAction.started -= Back;
        MainMenuController.Instance.ShowMainMenu();
    }

    IEnumerator ConfirmCoroutine()
    {
        MainMenuController.PlayerInput.enabled = false;
        yield return new WaitForSeconds(0.3f);
        MainMenuController.PlayerInput.enabled = true;
        GoToMainMenu();
    }

    public void Confirm(InputAction.CallbackContext context)
    {
        MainMenuController.Instance.PlayConfirmAudioClip();
        StartCoroutine(ConfirmCoroutine());
    }
    
    public void Back(InputAction.CallbackContext context)
    {
        MainMenuController.Instance.PlayBackAudioClip();
        GoToMainMenu();
    }


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

        int min = (int)ResultData.elapsedTime / 60;
        int sec = (int)ResultData.elapsedTime % 60;
        int millisec = (int)((ResultData.elapsedTime * 100) % 100);
        elapsedTimeText.text = string.Format("{0:00}'{1:00}\"{2:00}", min, sec, millisec);

        timeBonusText.text = string.Format("{0:n0}", ResultData.GetTimeBonusScore());
        scoreText.text = string.Format("{0:n0}", ResultData.score);

        int totalScore = (ResultData.GetTimeBonusScore() + ResultData.score);
        totalScoreText.text = string.Format("{0:n0}", totalScore);

        rankText.text = GetRank(totalScore).ToString();

        // Input
        InputAction submitAction = MainMenuController.PlayerInput.actions.FindAction("Submit");
        submitAction.started += Confirm;
        InputAction backAction = MainMenuController.PlayerInput.actions.FindAction("Back");
        backAction.started += Back;
    }
}
