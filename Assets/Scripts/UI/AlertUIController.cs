using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIController : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField]
    float warningBlinkTime = 0.7f;

    [Header("Warning/Alert Label Object")]
    [SerializeField]
    RawImage labelImage;

    [Space(10)]
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

    int currentPriority;
    float labelTimer;
    PlayerAircraft.WarningStatus prevWarningStatus = PlayerAircraft.WarningStatus.NONE;


    public enum LabelEnum  // Used for Priority
    {
        Missed = 1,
        Hit,
        Destroyed,
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

        if(currentPriority < labelPriority)
        {
            currentPriority = labelPriority;
            labelTimer = labelInfo.VisibleTime;

            labelImage.texture = labelInfo.LabelTexture;
            labelImage.color = labelInfo.LabelColor;
        }
        else if(currentPriority == labelPriority)
        {
            labelTimer = labelInfo.VisibleTime;
        }
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
    }

    void Start()
    {
        labelImage.color = transparentColor;
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
