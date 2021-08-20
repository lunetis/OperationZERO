using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    Gamepad gamepad;

    // Move Inputs
    float accelValue; 
    float brakeValue;
    float throttle;

    float rollValue;
    float pitchValue;
    float yawRValue;
    float yawLValue;

    // Aircraft Values
    float speed;

    // public values
    [Header("Aircraft Settings")]
    [SerializeField]
    float maxSpeed = 301.7f;
    [SerializeField]
    float minSpeed = 15;
    [SerializeField]
    float defaultSpeed = 60;

    [Header("Move Variables")]
    [SerializeField]
    float throttleAmount;

    [SerializeField]
    float accelAmount;
    [SerializeField]
    float brakeAmount;
    [SerializeField]
    float calibrateAmount;  // Speed Calibration

    [SerializeField]
    float rollAmount;
    [SerializeField]
    float pitchAmount;
    [SerializeField]
    float yawAmount;

    [SerializeField]
    float rotateLerpAmount;

    Vector3 rotateValue;
    
    [Header("High-G Turn")]
    [SerializeField]
    float highGFactor = 1.5f;
    [SerializeField]
    float highGTurnTime = 2.0f;

    float highGCooldown;
    float highGReciprocal;
    bool isHighGPressed;
    bool isHighGEnabled;

    bool isHighGTurning;

    [Header("Misc.")]
    [SerializeField]
    float stallSpeed;
    [SerializeField]
    float stallHeight = 4000;
    [SerializeField]
    float gravityFactor;
    [SerializeField]
    float lowAititudeThreshold;

    [Header("Model/Engine Control")]
    [SerializeField]
    AircraftModelController modelController;

    [SerializeField]
    List<JetEngineController> jetEngineControllers;

    Rigidbody rb;
    float speedReciprocal;
    Vector3 rotateReciprocal;

    // Controllers
    CameraController cameraController;
    UIController uiController;

    // public gets
    public float Speed
    {
        get { return speed; }
    }

    public float Throttle
    {
        get { return throttle; }
    }

    public Vector3 RotateValue
    {
        get { return rotateValue; }
    }

    bool isAutoPilot;
    public bool IsAutoPilot
    {
        get { return isAutoPilot; }
    }

    bool isStalling;
    public bool IsStalling
    {
        get { return isStalling; }
    }
    
    bool lowAltitude;
    public bool LowAltitude
    {
        get { return lowAltitude; }
    }


    // Input Action Event Callbacks
    // Movement Callbacks
    public void Accelerate(InputAction.CallbackContext context)
    {
        accelValue = context.ReadValue<float>();
    }

    public void Brake(InputAction.CallbackContext context)
    {
        brakeValue = context.ReadValue<float>();
    }

    public void Roll(InputAction.CallbackContext context)
    {
        rollValue = context.ReadValue<float>();
    }

    public void Pitch(InputAction.CallbackContext context)
    {
        pitchValue = context.ReadValue<float>();
    }

    public void YawL(InputAction.CallbackContext context)
    {
        yawLValue = context.ReadValue<float>();
    }

    public void YawR(InputAction.CallbackContext context)
    {
        yawRValue = context.ReadValue<float>();
    }
    
    void SetUI()
    {
        uiController.SetSpeed((int)(speed * 10));
        uiController.SetAltitude((int)(transform.position.y * 5));
        uiController.SetThrottle(throttle);
        uiController.SetHeading(transform.eulerAngles.y);
    }

    void CheckHighGTurn(ref float accel, ref float brake, ref float highGPitchFactor)
    {
        isHighGTurning = false;
        
        // Factor decreases 2 to 1
        if(accelValue == 1 && brakeValue == 1) // Button
        {
            if(pitchValue < -0.7f)
            {
                if(isHighGEnabled == true)
                {
                    isHighGEnabled = false;
                    isHighGPressed = true;
                }

                if(highGCooldown < 0) isHighGPressed = false;

                if(isHighGEnabled == true || isHighGPressed == true)
                {
                    accel = 0;
                    brake *= highGFactor * (1 + highGCooldown * highGReciprocal);
                    highGPitchFactor = highGFactor * (1 + highGCooldown * highGReciprocal);

                    highGCooldown -= Time.fixedDeltaTime;
                    isHighGTurning = true;
                }
            }
        }
        else // Button Released
        {
            isHighGPressed = false;
            isHighGEnabled = true;
        }

        if(isHighGPressed == false)
        {
            highGCooldown += Time.fixedDeltaTime * 2;
            if(highGCooldown >= highGTurnTime)
            {
                highGCooldown = highGTurnTime;
            }
        }
    }

    void Autopilot(out Vector3 rotateVector)
    {
        rotateVector = -transform.eulerAngles;
        if(rotateVector.x < -180) rotateVector.x += 360;
        if(rotateVector.z < -180) rotateVector.y += 360;
        if(rotateVector.z < -180) rotateVector.z += 360;

        rotateVector.x = Mathf.Clamp(rotateVector.x * 2, -pitchAmount, pitchAmount);
        rotateVector.z = Mathf.Clamp(rotateVector.z * 2, -rollAmount, rollAmount);
        rotateVector.y = 0;
    }

    void Stall()
    {
        Quaternion targetRotation = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);
        Quaternion diffQuaternion = Quaternion.Inverse(transform.rotation) * targetRotation;
        
        Vector3 diffAngle = diffQuaternion.eulerAngles;

        // Adjustment
        if(diffAngle.x > 180) diffAngle.x -= 360;
        if(diffAngle.y > 180) diffAngle.y -= 360;
        if(diffAngle.z > 180) diffAngle.z -= 360;
        diffAngle.x = Mathf.Clamp(diffAngle.x, -pitchAmount, pitchAmount);
        diffAngle.y = Mathf.Clamp(diffAngle.y, -yawAmount, yawAmount);
        diffAngle.z = Mathf.Clamp(diffAngle.z, -rollAmount, rollAmount);
        
        rotateValue = Vector3.Lerp(rotateValue, diffAngle, rotateLerpAmount * Time.fixedDeltaTime);
    }

    void MoveAircraft()
    {
        float accel = accelValue;
        float brake = brakeValue;
        float highGPitchFactor = 1;

        // High-G Turn
        CheckHighGTurn(ref accel, ref brake, ref highGPitchFactor);
        
        // === Rotation ===
        Vector3 rotateVector;
        if(speed < stallSpeed || transform.position.y > stallHeight)
        {
            // Ignore all rotation input and head to the ground
            isStalling = true;
            Stall();
        }
        else
        {
            isStalling = false;

            // Autopilot (Press L Shoulder + R Shoulder)
            if(yawLValue == 1 && yawRValue == 1)
            {
                isAutoPilot = true;
                Autopilot(out rotateVector);
            }
            // Rotation
            else
            {
                isAutoPilot = false;
                rotateVector = new Vector3(pitchValue * pitchAmount * highGPitchFactor, (yawRValue - yawLValue) * yawAmount, -rollValue * rollAmount);
            }
            rotateValue = Vector3.Lerp(rotateValue, rotateVector, rotateLerpAmount * Time.fixedDeltaTime);
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));

        // === Move ===
        throttle = Mathf.Lerp(throttle, accel - brake, throttleAmount * Time.fixedDeltaTime);

        if(throttle > 0)
        {
            float accelEase = (maxSpeed + (transform.position.y * 0.01f) - speed) * speedReciprocal;
            speed += throttle * accelAmount * accelEase * Time.fixedDeltaTime;
        }
        else if(throttle < 0)
        {
            float brakeEase = (speed - minSpeed) * speedReciprocal;
            speed += throttle * brakeAmount * brakeEase * Time.fixedDeltaTime;
        }

        float release = 1 - Mathf.Abs(throttle);
        speed += release * (defaultSpeed - speed) * speedReciprocal * calibrateAmount * Time.fixedDeltaTime;
        
        // Gravity
        float gravityFallByPitch = gravityFactor * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);
        speed += gravityFallByPitch * Time.fixedDeltaTime;

        rb.velocity = transform.forward * speed;
    }

    void CheckLowAltitude()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, lowAititudeThreshold, 1 << LayerMask.NameToLayer("Ground"));
        // DebugText("Low : " + hit);
    }

    void PassCameraControl()
    {
        float zoomValue = accelValue - brakeValue;
        cameraController.AdjustCameraValue(zoomValue, rollValue, pitchValue);
    }

    void JetEngineControl()
    {
        foreach(JetEngineController jet in jetEngineControllers)
        {
            jet.InputValue = throttle;
        }
    }

    void ModelControl()
    {
        modelController.SetElevatorAngle(rotateValue.x * rotateReciprocal.x);       // Pitch
        modelController.SetRudderAngle(rotateValue.y * rotateReciprocal.y);         // Yaw
        modelController.SetAileronAndFlapAngle(rotateValue.z * rotateReciprocal.z); // Roll
        modelController.SetBrakeStatus(brakeValue > 0.9f);
    }

    void OnDisable()
    {
        foreach(JetEngineController jet in jetEngineControllers)
        {
            jet.enabled = false;
        }
        CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
        foreach(CapsuleCollider collider in colliders)
        {
            collider.enabled = false;
        }
        modelController.ResetStatus();
    }

    void OnEnable()
    {
        foreach(JetEngineController jet in jetEngineControllers)
        {
            jet.enabled = true;
        }
        CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
        foreach(CapsuleCollider collider in colliders)
        {
            collider.enabled = true;
        }
    }

    void Start()
    {
        uiController = GameManager.UIController;

        accelValue = 0;
        brakeValue = 0;
        rollValue = 0;
        pitchValue = 0;
        yawLValue = 0;
        yawRValue = 0;

        highGCooldown = highGTurnTime;
        highGReciprocal = 1 / highGCooldown;
        isHighGPressed = false;
        isHighGEnabled = true;

        rb = GetComponent<Rigidbody>();
        cameraController = GetComponent<CameraController>();

        speedReciprocal = 1 / maxSpeed;
        speed = defaultSpeed;

        rotateReciprocal.x = 1 / pitchAmount;
        rotateReciprocal.y = 1 / yawAmount;
        rotateReciprocal.z = 1 / rollAmount;

        // UI
        SetUI();
    }

    void Update()
    {
        SetUI();
        CheckLowAltitude();
        JetEngineControl();

        ModelControl();
    }
    
    void FixedUpdate()
    {
        MoveAircraft();
        PassCameraControl();
    }
}
