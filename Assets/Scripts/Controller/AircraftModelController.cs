using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftModelController : MonoBehaviour
{
    [Header("Roll")]
    [SerializeField]
    Transform[] ailerons;
    [SerializeField]
    float aileronAngle;
    float initAileronAngle;

    [SerializeField]
    Transform[] flaps;
    [SerializeField]
    float flapAngle;
    float initFlapAngle;

    [Header("Pitch")]
    [SerializeField]
    Transform[] elevators;
    [SerializeField]
    float elevatorAngle;
    float initElevatorAngle;

    [Header("Yaw")]
    [SerializeField]
    Transform[] rudders;
    [SerializeField]
    float rudderAngle;
    float initRudderAngle;
    
    
    [Header("Brake")]
    [SerializeField]
    Transform airBrake;
    [SerializeField]
    float brakeAngle;
    float initBrakeAngle;
    
    [SerializeField]
    Transform airBrakeRod;
    [SerializeField]
    float brakeRodAngle;
    float initBrakeRodAngle;

    float brakeStatus;
    float brakeValue;
    [SerializeField]
    float brakeLerpAmount = 0.8f;


    public void SetAileronAndFlapAngle(float value)
    {
        // Each ailerons and flaps' angle must be reversed
        // Aileron
        float angle = value * aileronAngle + initAileronAngle;
        for(int i = 0; i < ailerons.Length; i++)
        {
            angle *= -1;
            Vector3 aileronEulerAngles = ailerons[i].localEulerAngles;
            aileronEulerAngles.x = angle;
            ailerons[i].localEulerAngles = aileronEulerAngles;
        }

        // Flap
        angle = value * flapAngle + initFlapAngle;
        for(int i = 0; i < flaps.Length; i++)
        {
            angle *= -1;
            Vector3 flapEulerAngles = flaps[i].localEulerAngles;
            flapEulerAngles.x = angle;
            flaps[i].localEulerAngles = flapEulerAngles;
        }
    }

    public void SetRudderAngle(float value)
    {
        float angle = value * rudderAngle + initRudderAngle;
        for(int i = 0; i < rudders.Length; i++)
        {
            Vector3 rudderEulerAngles = rudders[i].localEulerAngles;
            rudderEulerAngles.z = angle;
            rudders[i].localEulerAngles = rudderEulerAngles;
        }
    }

    public void SetElevatorAngle(float value)
    {
        float angle = value * elevatorAngle + initElevatorAngle;
        for(int i = 0; i < elevators.Length; i++)
        {
            Vector3 elevatorEulerAngles = elevators[i].localEulerAngles;
            elevatorEulerAngles.x = angle;
            elevators[i].localEulerAngles = elevatorEulerAngles;
        }
    }

    public void SetBrakeStatus(bool isEnabled)
    {
        brakeStatus = (isEnabled == true) ? 1 : 0;
    }

    public void ResetStatus()
    {
        SetAileronAndFlapAngle(0);
        SetRudderAngle(0);
        SetElevatorAngle(0);
        brakeValue = 0;
        brakeStatus = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        initAileronAngle = ailerons[0].localEulerAngles.x;
        initFlapAngle = flaps[0].localEulerAngles.x;
        initRudderAngle = rudders[0].localEulerAngles.z;
        initElevatorAngle = elevators[0].localEulerAngles.x;
        initBrakeAngle = airBrake.localEulerAngles.x;
        initBrakeRodAngle = airBrakeRod.localEulerAngles.z;
        brakeValue = 0;
        brakeStatus = 0;
    }

    void Update()
    {
        brakeValue = Mathf.Lerp(brakeValue, brakeStatus, brakeLerpAmount * Time.deltaTime);

        // Air Brake
        float airBrakeAngle = brakeValue * brakeAngle + initBrakeAngle;
        Vector3 eulerAngles = airBrake.localEulerAngles;
        eulerAngles.x = airBrakeAngle;
        airBrake.localEulerAngles = eulerAngles;

        // Brake Rod
        float rodAngle = brakeValue * brakeRodAngle + initBrakeRodAngle;
        eulerAngles = airBrakeRod.localEulerAngles;
        eulerAngles.z = rodAngle;
        airBrakeRod.localEulerAngles = eulerAngles;
    }
}
