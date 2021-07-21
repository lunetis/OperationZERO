using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
class TimeScript
{
    public int time;
    public string scriptKey;
}

public class RedTimer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI timeText;

    [SerializeField]
    List<TimeScript> remainTimeScripts;
    List<string> removableScriptKeys;

    [SerializeField]
    AudioClip beepSingleClip;
    [SerializeField]
    AudioClip beepDoubleClip;
    AudioSource audioSource;

    float remainTime;
    bool isTimeLow;

    public int RemainTime
    {
        set
        {
            GameManager.UIController.SetRemainTime(value);
            remainTime = value;
        }
    }

    void SetTime()
    {
        remainTime -= Time.deltaTime;

        if(isTimeLow == false && remainTime <= 31)
        {
            isTimeLow = true;
            InvokeRepeating("PlayTimeLowAudioClip", 0, 1);
        }
        
        if(remainTime <= 0)
        {
            GameManager.Instance.GameOver(false);
            remainTime = 0;
        }
        CheckTimeScripts();

        int seconds = (int)remainTime;
        

        int min = seconds / 60;
        int sec = seconds % 60;
        int millisec = (int)((remainTime - seconds) * 100);
        string text = string.Format("<mspace=13>{0:00}</mspace>:<mspace=13>{1:00}</mspace>:<mspace=13>{2:00}</mspace>", min, sec, millisec);
        timeText.text = text;
    }

    void PlayTimeLowAudioClip()
    {
        if(GameManager.Instance.IsGameOver == true)
        {
            CancelInvoke();
            return;
        }
        
        AudioClip audioClip = (remainTime > 10) ? beepSingleClip : beepDoubleClip;
        audioSource.PlayOneShot(audioClip);
    }


    void CheckTimeScripts()
    {
        foreach(TimeScript timeScript in remainTimeScripts)
        {
            if(remainTime < timeScript.time)
            {
                GameManager.ScriptManager.AddScript(timeScript.scriptKey);
                removableScriptKeys.Add(timeScript.scriptKey);
            }
        }

        if(removableScriptKeys.Count > 0)
        {
            remainTimeScripts.RemoveAll(script => removableScriptKeys.Contains(script.scriptKey));
            removableScriptKeys.Clear();
        }
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        removableScriptKeys = new List<string>();
    }

    void OnEnable()
    {
        GameManager.UIController.IsRedTimerActive = true;
        isTimeLow = false;

        // Remove timed out scripts
        remainTimeScripts.RemoveAll(script => script.time >= remainTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(remainTime > 0) SetTime();
    }
}
