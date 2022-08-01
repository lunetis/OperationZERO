using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    [Header("Attack Preferences")]
    [SerializeField]
    Missile missile;

    [SerializeField]
    Transform missileLaunchTransform;
    
    [SerializeField]
    [Range(0.1f, 1)]
    float lockSpeedPenalty = 0.1f;
    
    [SerializeField]
    float fireCheckDelay = 5.0f;

    TargetObject targetObject;

    // From Missile Data
    float targetSearchSpeed;
    float boresightAngle;
    float lockDistance;

    // Status
    float lockProgress;
    bool isLocked;
    AircraftAI aircraftAI;

    void ResetLock()
    {
        isLocked = false;
        lockProgress = 0;

        if(targetObject != null)
        {
            targetObject.IsLocking = false;
        }
    }

    float GetAngleBetweenTransform(Transform otherTransform)
    {
        Vector3 direction = transform.forward;
        Vector3 diff = otherTransform.position - transform.position;
        return Vector3.Angle(diff, direction);
    }
    
    void CheckTargetLock()
    {
        // No target
        if(targetObject == null)
        {
            ResetLock();
            return;
        }

        float distance = Vector3.Distance(targetObject.transform.position, transform.position);

        // Exceed lockable distance
        if(distance > lockDistance)
        {
            ResetLock();
            return;
        }
        
        float targetAngle = GetAngleBetweenTransform(targetObject.transform);
        
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
            targetObject.IsLocking = true;

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
            }
            // Still Locking...
            else
            {
                isLocked = false;
            }
        }
    }


    void LaunchMissile()
    {
        if(GameManager.Instance.IsGameOver == true)
        {
            CancelInvoke();
            return;
        }
        
        if(isLocked == false) return;
        
        // Get from Object Pool and Launch
        GameObject missile = GameManager.Instance.enemyMissileObjectPool.GetPooledObject();
        missile.transform.position = missileLaunchTransform.position;
        missile.transform.rotation = missileLaunchTransform.rotation;
        missile.SetActive(true);

        Missile missileScript = missile.GetComponent<Missile>();
        missileScript.Launch(targetObject, aircraftAI.Speed + 15, gameObject.layer);
    }

    void OnDisable()
    {
        if(targetObject != null)
        {
            targetObject.IsLocking = false;
        }
        CancelInvoke();
    }

    void OnEnable()
    {
        InvokeRepeating("LaunchMissile", 1, fireCheckDelay);
    }

    // Start is called before the first frame update
    void Start()
    {
        boresightAngle = missile.boresightAngle;
        targetSearchSpeed = missile.targetSearchSpeed * lockSpeedPenalty;
        lockDistance = missile.lockDistance;
        aircraftAI = GetComponent<AircraftAI>();

        ResetLock();

        targetObject = GameManager.PlayerAircraft;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargetLock();
    }
}
