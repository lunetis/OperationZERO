using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetLock : FollowTransformUI
{
    RawImage rawImage;

    [SerializeField]
    Texture mslLockTexture;
    [SerializeField]
    Texture spwLockTexture;
    [SerializeField]
    RectTransform crosshair;

    // From Missile Data
    float targetSearchSpeed;
    float boresightAngle;
    float lockDistance;

    // Status
    float lockProgress;
    bool isLocked;

    public bool IsLocked
    {
        get { return isLocked; }
    }

    // Calculated Target Screen Position
    Vector2 targetScreenPosition;

    // Set image invisible
    void ResetLock()
    {
        isLocked = false;
        lockProgress = 0;
        rawImage.color = GameManager.NormalColor;
        rawImage.enabled = false;

        GameManager.TargetController.SetTargetUILock(false);
    }

    public void SetTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
        ResetLock();
    }

    public void SwitchWeapon(Missile missile)
    {
        // Change missile's angle and search speed
        boresightAngle = missile.boresightAngle;
        targetSearchSpeed = missile.targetSearchSpeed;
        lockDistance = missile.lockDistance;

        // Progress needs to be reset
        ResetLock();

        rawImage.texture = (missile.isSpecialWeapon == true) ? spwLockTexture : mslLockTexture;
    }


    void CheckTargetLock()
    {
        // No target
        if(targetTransform == null)
        {
            ResetLock();
            return;
        }

        float distance = Vector3.Distance(targetTransform.position, GameManager.AircraftController.transform.position);

        // Exceed lockable distance
        if(distance > lockDistance)
        {
            ResetLock();
            return;
        }
        
        cam = GameManager.CameraController.GetActiveCamera();
        Vector3 screenPosition = cam.WorldToScreenPoint(targetTransform.position);
        
        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, targetTransform.position);
            targetScreenPosition = screenPoint - canvasRect.sizeDelta * 0.5f;
        }
        
        float targetAngle = GameManager.GetAngleBetweenTransform(targetTransform);
        

        // When the target exists, increase lockProgress
        // if lockProgress >= targetAngle, it means the target is locked

        // Missed the Target
        if(targetAngle > boresightAngle)
        {
            ResetLock();
        }

        // Locking...
        else
        {
            rawImage.enabled = true;

            // Lock Progress
            if(isLocked == false)
            {
                lockProgress += targetSearchSpeed * Time.deltaTime;
            }

            // Locked!
            if(lockProgress >= targetAngle)
            {
                isLocked = true;
                lockProgress = boresightAngle;
                rawImage.color = GameManager.WarningColor;

                GameManager.TargetController.SetTargetUILock(true);
            }
            // Still Locking...
            else
            {
                isLocked = false;
                rawImage.color = GameManager.NormalColor;
            }

            rectTransform.anchoredPosition = Vector2.Lerp(crosshair.anchoredPosition, targetScreenPosition, lockProgress / targetAngle);
        }
    }

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Start is called before the first frame update
    protected override void Start() 
    {
        base.Start();
        ResetLock();
    }

    // Update is called once per frame
    protected override void Update()
    {
        CheckTargetLock();
    }
}
