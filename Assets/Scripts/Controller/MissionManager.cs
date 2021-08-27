using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static int phase = 1;

    [Header("Game Properties")]
    [SerializeField]
    MissionInfo missionInfo;

    [Header("Common Scripts")]
    [SerializeField]
    List<string> onMissionStartScripts;
    [SerializeField]
    List<string> onMissionAccomplishedScripts;
    [SerializeField]
    List<string> onMissionFailedScripts;
    [SerializeField]
    List<string> onDeadScripts;

    public MissionInfo MissionInfo
    {
        get { return missionInfo; }
    }
    
    public void InvokeMethod(string methodName, float delay)
    {
        Invoke(methodName, delay);
    }

    public virtual void OnGameOver(bool isDead)
    {
        if(isDead == true)
        {
            if(onDeadScripts.Count == 0) return;
            int index = UnityEngine.Random.Range(0, onDeadScripts.Count);
            GameManager.ScriptManager.AddScript(onDeadScripts[index]);
        }
        else
        {
            if(onMissionFailedScripts.Count == 0) return;
            GameManager.ScriptManager.AddScript(onMissionFailedScripts);
        }
    }

    public virtual void SetupForRestartFromCheckpoint() {}

    protected void SetResultData()
    {
        ResultData.missionName = missionInfo.MissionName;
        ResultData.maxTime = missionInfo.TimeLimit;
        ResultData.timeBonusPerSecond = missionInfo.TimeBonusPerSecond;
        ResultData.rankScoreCutoff = missionInfo.RankScoreCutoff;
    }

    protected virtual void Start()
    {
        SetResultData();
        
        GameManager.UIController.SetRemainTime(missionInfo.TimeLimit);
        GameManager.ScriptManager.AddScript(onMissionStartScripts);
    }
}
