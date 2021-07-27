using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIController : MonoBehaviour
{
    [SerializeField]
    bool showStartMissionLabel;

    [Header("Timers")]
    [SerializeField]
    float warningBlinkTime = 0.7f;

    [Header("Warning/Alert Label Object")]
    [SerializeField]
    RawImage labelImage;

    [Space(10)]
    [SerializeField]
    LabelInfo startMission;
    [SerializeField]
    LabelInfo missionUpdated;
    [SerializeField]
    LabelInfo destroyed;
    [SerializeField]
    LabelInfo hit;
    [SerializeField]
    LabelInfo missed;
    [SerializeField]
    LabelInfo missionAccomplished;
    [SerializeField]
    LabelInfo missionFailed;

    [Header("Sounds")]
    [SerializeField]
    float voiceAlertRepeatTime = 1.5f;
    [SerializeField]
    float missileCautionAlertRepeatTime = 1.0f;
    [SerializeField]
    float missileWarningAlertRepeatTime = 0.2f;
    [SerializeField]
    float missileEmergencyAlertRepeatTime = 0.1f;

    [SerializeField]
    AudioClip warningBeepAlertClip;
    [SerializeField]
    AudioClip missileBeepAlertClip;
    [SerializeField]
    AudioClip missileVoiceAlertClip;
    [SerializeField]
    AudioClip stallVoiceAlertClip;

    [SerializeField]
    AudioSource voiceAudioSource;
    [SerializeField]
    AudioSource alertAudioSource;
    [SerializeField]
    AudioSource labelAudioSource;

    bool isPlayingVoiceAlert = false;

    int currentPriority;
    float labelTimer;
    PlayerAircraft.WarningStatus prevWarningStatus = PlayerAircraft.WarningStatus.NONE;

    public enum LabelEnum  // Used for Priority
    {
        StartMission = 1,
        Missed,
        Hit,
        Destroyed,
        MissionUpdated,
        MissionAccomplished,
        MissionFailed
    }

    Color transparentColor = new Color(0, 0, 0, 0);

    [Header("Attack Alerts")]
    // Attack
    [SerializeField]
    GameObject alertParent;

    [SerializeField]
    GameObject caution;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject missileAlert;

    [Header("Status Alerts")]
    // Status
    [SerializeField]
    GameObject pullUp;
    [SerializeField]
    GameObject stalling;
    [SerializeField]
    GameObject damaged;

    [Header("Misc.")]
    // Misc
    [SerializeField]
    GameObject autopilot;
    [SerializeField]
    GameObject fire;
    [SerializeField]
    GameObject missileReloading;

    // Category : Attack

    // Category : Status
    public void SetLabel(LabelEnum labelEnum)
    {
        LabelInfo labelInfo;
        switch(labelEnum)
        {
            case LabelEnum.StartMission:
                labelInfo = startMission;
                break;
            case LabelEnum.MissionUpdated:
                labelInfo = missionUpdated;
                break;
            case LabelEnum.Missed:
                labelInfo = missed;
                break;
            case LabelEnum.Hit:
                labelInfo = hit;
                break;
            case LabelEnum.Destroyed:
                labelInfo = destroyed;
                break;
            case LabelEnum.MissionFailed:
                labelInfo = missionFailed;
                break;
            case LabelEnum.MissionAccomplished:
                labelInfo = missionAccomplished;
                break;
                
            default:    // Error case
                labelInfo = missed;
                break;
        }

        int labelPriority = (int)labelEnum;

        // Show new label
        if(currentPriority < labelPriority)
        {
            currentPriority = labelPriority;
            labelTimer = labelInfo.VisibleTime;

            labelImage.texture = labelInfo.LabelTexture;
            labelImage.color = labelInfo.LabelColor;

            if(labelInfo.AudioClip != null)
            {
                labelAudioSource.PlayOneShot(labelInfo.AudioClip);
            }
        }
        // Initialize visible time
        else if(currentPriority == labelPriority)
        {
            labelTimer = labelInfo.VisibleTime;
        }
    }

    public IEnumerator ShowDamagedUI()
    {
        damaged.SetActive(true);
        GameManager.UIController.SetWarningUIColor(true);
        
        yield return new WaitForSeconds(0.2f);

        GameManager.UIController.SetWarningUIColor(false);

        yield return new WaitForSeconds(0.8f);

        damaged.SetActive(false);
    }

    // Misc.
    void ShowAutopilotUI()
    {
        if(GameManager.AircraftController.IsAutoPilot != autopilot.activeInHierarchy)
            autopilot.SetActive(GameManager.AircraftController.IsAutoPilot);
    }

    void ShowStallingUI()
    {
        if(GameManager.AircraftController.IsStalling != stalling.activeInHierarchy)
            stalling.SetActive(GameManager.AircraftController.IsStalling);
    }

    void HideAllAttackAlertUI()
    {
        Transform alertTransform = alertParent.transform;
        for(int i = 0; i < alertTransform.childCount; i++)
        {
            alertTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void BlinkAttackAlertUI()
    {
        alertParent.SetActive(!alertParent.activeInHierarchy);
    }

    void ShowAttackAlertUI()
    {
        if(GameManager.PlayerAircraft == null) return;
        
        PlayerAircraft.WarningStatus warningStatus = GameManager.PlayerAircraft.GetWarningStatus();
        if(prevWarningStatus == warningStatus) return;

        prevWarningStatus = warningStatus;
        CancelInvoke();
        HideAllAttackAlertUI();
        alertParent.SetActive(false);

        // Missile alert
        switch(warningStatus)
        {
            case PlayerAircraft.WarningStatus.MISSILE_ALERT_EMERGENCY:
                missileAlert.SetActive(true);
                InvokeRepeating("BlinkAttackAlertUI", 0, warningBlinkTime);

                GameManager.UIController.SetWarningUIColor(true);
                break;
                
            case PlayerAircraft.WarningStatus.MISSILE_ALERT:
                missileAlert.SetActive(true);
                InvokeRepeating("BlinkAttackAlertUI", 0, warningBlinkTime);

                GameManager.UIController.SetWarningUIColor(true);
                break;
                
            case PlayerAircraft.WarningStatus.WARNING:
                warning.SetActive(true);
                InvokeRepeating("BlinkAttackAlertUI", 0, warningBlinkTime);

                GameManager.UIController.SetWarningUIColor(false);
                break;
                
            case PlayerAircraft.WarningStatus.NONE:
                warning.SetActive(false);
                
                GameManager.UIController.SetWarningUIColor(false);
                break;
        }

        SetAlertAudio(warningStatus);
    }


    void SetAlertAudio(PlayerAircraft.WarningStatus warningStatus)
    {
        switch(warningStatus)
        {
            case PlayerAircraft.WarningStatus.MISSILE_ALERT_EMERGENCY:
                CancelInvoke("PlayMissileBeepAudio");
                CancelInvoke("PlayWarningBeepAudio");
                InvokeRepeating("PlayMissileBeepAudio", 0, missileWarningAlertRepeatTime);
                 if(isPlayingVoiceAlert == false) InvokeRepeating("PlayMissileVoiceAudio", 0, voiceAlertRepeatTime);
                break;
                
            case PlayerAircraft.WarningStatus.MISSILE_ALERT:
                CancelInvoke("PlayMissileBeepAudio");
                CancelInvoke("PlayWarningBeepAudio");
                InvokeRepeating("PlayMissileBeepAudio", 0, missileCautionAlertRepeatTime);
                if(isPlayingVoiceAlert == false) InvokeRepeating("PlayMissileVoiceAudio", 0, voiceAlertRepeatTime);
                break;
                
            case PlayerAircraft.WarningStatus.WARNING:
                CancelInvoke("PlayMissileBeepAudio");
                CancelInvoke("PlayMissileVoiceAudio");
                InvokeRepeating("PlayWarningBeepAudio", missileCautionAlertRepeatTime * 0.2f, missileCautionAlertRepeatTime);
                break;
                
            case PlayerAircraft.WarningStatus.NONE:
                CancelInvoke("PlayMissileBeepAudio");
                CancelInvoke("PlayWarningBeepAudio");
                CancelInvoke("PlayMissileVoiceAudio");
                isPlayingVoiceAlert = false;
                break;
        }
    }

    void PlayWarningBeepAudio()
    {
        alertAudioSource.PlayOneShot(warningBeepAlertClip);
    }

    void PlayMissileBeepAudio()
    {
        alertAudioSource.PlayOneShot(missileBeepAlertClip);
    }

    void PlayMissileVoiceAudio()
    {
        isPlayingVoiceAlert = true;
        voiceAudioSource.PlayOneShot(missileVoiceAlertClip);
    }

    public void OnGameOver()
    {
        CancelInvoke();
        HideAllAttackAlertUI();
        alertParent.SetActive(false);
        
        GameManager.UIController.SetWarningUIColor(false);
    }


    void Start()
    {
        labelImage.color = transparentColor;

        if(showStartMissionLabel == true)
        {
            SetLabel(LabelEnum.StartMission);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowAutopilotUI();
        ShowStallingUI();
        ShowAttackAlertUI();

        if(labelTimer > 0)
        {
            labelTimer -= Time.deltaTime;
            
            // Set Invisible
            if(labelTimer <= 0)
            {
                labelTimer = 0;
                currentPriority = 0;
                labelImage.color = transparentColor;
            }
        }
    }
}
