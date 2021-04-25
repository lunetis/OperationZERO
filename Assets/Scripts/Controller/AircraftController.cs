using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftController : TargetObject
{
    Gamepad gamepad;

    // Move Inputs
    float accelValue;
    float brakeValue;
    float throttle;

    public float Throttle
    {
        get
        {
            return throttle;
        }
    }

    float rollValue;
    float pitchValue;
    float yawValue;

    // Aircraft Values
    float speed;
    public float Speed
    {
        get
        {
            return speed;
        }
    }
    int damage = 0;
    int score = 0;

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
    float calibrateAmount;

    [SerializeField]
    float rollAmount;
    [SerializeField]
    float pitchAmount;
    [SerializeField]
    float yawAmount;

    Vector3 rotateValue;

    public Vector3 RotateValue
    {
        get { return rotateValue; }
    }

    // rotate lerp
    [SerializeField]
    float rotateLerpAmount;
    Rigidbody rb;
    
    float speedReciprocal;

    // Controllers
    CameraController cameraController;
    UIController uiController;

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

    public void Yaw(InputAction.CallbackContext context)
    {
        yawValue = context.ReadValue<float>();
    }

    void SetUI()
    {
        uiController.SetSpeed((int)(speed * 10));
        uiController.SetAltitude((int)(transform.position.y * 5));
        uiController.SetThrottle(throttle);
        uiController.SetHeading(transform.eulerAngles.y);
    }

    void MoveAircraft()
    {
        // Rotation
        Vector3 lerpVector = new Vector3(pitchValue * pitchAmount, yawValue * yawAmount, -rollValue * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, rotateLerpAmount * Time.deltaTime);

        transform.Rotate(rotateValue * Time.deltaTime);

        // Move
        throttle = Mathf.Lerp(throttle, accelValue - brakeValue, throttleAmount * Time.deltaTime);

        if(throttle > 0)
        {
            float accelEase = (maxSpeed + (transform.position.y * 0.01f) - speed) * speedReciprocal;
            speed += throttle * accelAmount * accelEase * Time.deltaTime;
        }
        else if(throttle < 0)
        {
            float brakeEase = (speed - minSpeed) * speedReciprocal;
            speed += throttle * brakeAmount * brakeEase * Time.deltaTime;
        }

        float release = 1 - Mathf.Abs(throttle);
        speed += release * (defaultSpeed - speed) * speedReciprocal * calibrateAmount * Time.deltaTime;
        
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }

    void OnDamage(int damage)
    {
        this.damage += damage;
        uiController.SetDamageText(this.damage);
    }

    void OnScore(int score)
    {
        this.score += score;
        uiController.SetScoreText(this.score);
    }

    void PassCameraControl()
    {
        float zoomValue = accelValue - brakeValue;
        cameraController.AdjustCameraValue(zoomValue, rollValue, pitchValue);
    }

    void Start()
    {
        uiController = GameManager.Instance.uiController;
        gamepad = Gamepad.current;

        accelValue = 0;
        brakeValue = 0;
        rollValue = 0;
        pitchValue = 0;
        yawValue = 0;

        rb = GetComponent<Rigidbody>();
        cameraController = GetComponent<CameraController>();

        speedReciprocal = 1 / maxSpeed;
        speed = defaultSpeed;

        // UI
        SetUI();
        OnDamage(0);
        OnScore(0);
    }
    
    void Update()
    {
        MoveAircraft();
        PassCameraControl();
        SetUI();
    }
}
