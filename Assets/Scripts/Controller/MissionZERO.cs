using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionZERO : MissionManager
{
    [Header("Phase System")]
    [Header("Phase 1")]
    [SerializeField]
    List<string> onPhase1StartScripts;
    [SerializeField]
    List<string> phase1AdditionalScripts;
    [SerializeField]
    List<string> onPhase1EndScripts;
    
    [Header("Phase 2")]
    [SerializeField]
    UnityEvent onPhase2StartEvents;
    [SerializeField]
    List<string> onPhase2StartScripts;
    [SerializeField]
    List<string> onPhase2EndScripts;

    [SerializeField]
    List<string> phase2Scripts;

    [Header("Phase 3")]
    [SerializeField]
    UnityEvent onPhase3StartEvents;
    [SerializeField]
    List<string> onPhase3StartScripts;
    [SerializeField]
    List<string> onPhase3EndScripts;

    [SerializeField]
    List<string> onPhase3FailScripts;

    Queue<string> currentScriptQueue;

    [Space(10)]
    [SerializeField]
    PixyScript pixy;

    [SerializeField]
    CutsceneController cutsceneController;
    
    [SerializeField]
    Transform phase3PixyTransform;
    [SerializeField]
    Transform phase3CipherTransform;
    [SerializeField]
    RedTimer redTimer;

    [SerializeField]
    int phase3TimeLimit;

    ResultData resultData;

    public void CheckElapsedTimeBeforePhase3()
    {
        ResultData.elapsedTime += GameManager.UIController.StopCountAndGetElapsedTime();
    }

    public void CheckElapsedTimeAfterPhase3()
    {
        ResultData.elapsedTime += GameManager.UIController.StopCountAndGetElapsedTime();
        ResultData.score = GameManager.PlayerAircraft.Score;
    }

    // Start is called before the first frame update
    public void OnPhaseEnd()
    {
        switch(phase)
        {
            case 1:
                GameManager.ScriptManager.AddScript(onPhase1EndScripts);
                break;

            case 2:
                GameManager.ScriptManager.AddScript(onPhase2EndScripts);
                break;

            case 3:
                GameManager.ScriptManager.AddScript(onPhase3EndScripts);
                break;
        }

        ++phase;
    }

    public void OnPhaseStart()
    {
        switch(phase)
        {
            case 1:
                GameManager.ScriptManager.AddScript(onPhase1StartScripts);
                break;

            case 2:
                GameManager.ScriptManager.AddScript(onPhase2StartScripts);
                onPhase2StartEvents.Invoke();
                break;

            case 3:
                GameManager.ScriptManager.AddScript(onPhase3StartScripts);
                onPhase3StartEvents.Invoke();
                break;
        }
    }

    public override void OnGameOver(bool isDead)
    {
        if(phase == 3 && isDead == false)
        {
            GameManager.ScriptManager.AddScript(onPhase3FailScripts);
        }
        else
        {
            base.OnGameOver(isDead);
        }
    }

    public static void Shuffle<T>(ref List<T> list)  
    {
        if(list.Count <= 1) return;

        int n = list.Count;
        while(n > 0)
        {
            int i = Random.Range(0, --n);
            T temp = list[n];
            list[n] = list[i];
            list[i] = temp;
        }
    }

    public void AddPhase1Scripts()
    {
        // Execute when Pixy is still alive
        if(pixy.IsAttackable == true)
        {
            GameManager.ScriptManager.AddScript(phase1AdditionalScripts);
        }
    }

    public void AddPhase2Scripts()
    {
        if(phase2Scripts.Count > 0)
        {
            Shuffle<string>(ref phase2Scripts);
            currentScriptQueue = new Queue<string>(phase2Scripts);
            Invoke("PrintPhase2Script", Random.Range(30, 60));
        }
    }

    public void PrintPhase2Script()
    {
        GameManager.ScriptManager.AddScript(currentScriptQueue.Dequeue());
        
        if(currentScriptQueue.Count > 0)
        {
            Invoke("PrintPhase2Script", Random.Range(30, 60));
        }
    }

    public void SetPhase3Position()
    {
        GameManager.UIController.SetLabel(AlertUIController.LabelEnum.MissionUpdated);

        GameManager.AircraftController.transform.SetPositionAndRotation(phase3CipherTransform.position, phase3CipherTransform.rotation);
        pixy.transform.SetPositionAndRotation(phase3PixyTransform.position, phase3PixyTransform.rotation);

        pixy.ForceChangeWaypoint(phase3CipherTransform.position);
    }

    public void SetTimer()
    {
        redTimer.RemainTime = phase3TimeLimit;
        redTimer.gameObject.SetActive(true);
    }

    public void PlayPhase3Cutscene()
    {
        cutsceneController.PlayPhase3Cutscene();
    }

    public void PlayEndingCutscene()
    {
        cutsceneController.PlayEndingCutscene();
    }


    public override void SetupForRestartFromCheckpoint()
    {
        Debug.Log("Phase : " + phase);
        if(phase < 3)
        {
            phase = 1;
            ResultData.elapsedTime = 0;
        }
        else
        {
            phase = 3;
        }
    }

    public void PlayAtCheckpoint()
    {
        pixy.SetPhase3();
        OnPhaseStart();
    }

    protected override void Start()
    {
        Debug.Log("Phase : " + phase);
        if(phase == 3)
        {
            SetResultData();
            PlayAtCheckpoint();
        }
        else
        {
            base.Start();
        }
    }
}
