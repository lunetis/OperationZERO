using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertUIController : MonoBehaviour
{
    [Header("Warning/Alert Label Objects")]

    List<GameObject> attackAlerts;
    List<GameObject> statusAlerts;

    // Attack
    [SerializeField]
    GameObject caution;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject missileAlert;

    // Status
    [SerializeField]
    GameObject pullUp;
    [SerializeField]
    GameObject stalling;
    [SerializeField]
    GameObject damaged;

    // Misc
    [SerializeField]
    GameObject autopilot;
    [SerializeField]
    GameObject fire;
    [SerializeField]
    GameObject missileReloading;

    // Category : Attack

    // Category : Status

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
        // Index : Priority
        attackAlerts = new List<GameObject>(new GameObject[]{caution, warning, missileAlert});
        statusAlerts = new List<GameObject>(new GameObject[]{pullUp, stalling, damaged});
    }

    // Update is called once per frame
    void Update()
    {
        ShowAutopilotUI();
        ShowStallingUI();
    }
}
