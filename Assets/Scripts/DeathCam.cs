using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeathCam : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed = 1;
    [SerializeField]
    float zoomOutValue = -20;

    CinemachineOrbitalTransposer orbitalTransposer;

    // Start is called before the first frame update
    void Start()
    {
        CinemachineVirtualCamera vCam = GetComponent<CinemachineVirtualCamera>();
        orbitalTransposer = vCam?.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        orbitalTransposer.m_XAxis.Value = Random.Range(0, 360);
    }

    // Update is called once per frame
    void Update()
    {
        orbitalTransposer.m_XAxis.Value += Time.deltaTime * rotateSpeed;
    }
}
