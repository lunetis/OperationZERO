using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIController : MonoBehaviour
{
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
        if(GameManager.PlayerAircraft.IsAutoPilot != autopilot.activeInHierarchy)
            autopilot.SetActive(GameManager.PlayerAircraft.IsAutoPilot);
    }

    void ShowStallingUI()
    {
        if(GameManager.PlayerAircraft.IsStalling != stalling.activeInHierarchy)
            stalling.SetActive(GameManager.PlayerAircraft.IsStalling);
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
