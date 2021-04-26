using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public ObjectPool missileObjectPool;
    public ObjectPool specialWeaponObjectPool;
    public ObjectPool explosionEffectObjectPool;
    public ObjectPool smokeTrailEffectObjectPool;

    public ObjectPool bulletObjectPool;
    public ObjectPool bulletHitEffectObjectPool;
    public ObjectPool groundHitEffectObjectPool;

    public int timeLimit;
    int score;
    public UIController uiController;
    public AircraftController playerAircraft;
    public CameraController cameraController;
    public TargetController targetController;

    Vector3 zeroVec = new Vector3(0, 0, 0);
    
    List<TargetObject> objects = new List<TargetObject>();

    public static CameraController CameraController
    {
        get
        {
            return Instance.cameraController;
        }
    }

    public static AircraftController PlayerAircraft
    {
        get
        {
            return Instance.playerAircraft;
        }
    }

    public static UIController UIController
    {
        get
        {
            return Instance.uiController;
        }
    }

    public static TargetController TargetController
    {
        get
        {
            return Instance.targetController;
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        uiController.SetRemainTime(timeLimit);
    }

    public float GetDistanceFromPlayer(Transform otherTransform)
    {
        return Vector3.Distance(otherTransform.position, playerAircraft.transform.position);
    }


    // Enemy Target Control
    public void AddEnemy(TargetObject targetObject)
    {
        objects.Add(targetObject);
    }

    public void RemoveEnemy(TargetObject targetObject)
    {
        objects.Remove(targetObject);
    }

    public List<TargetObject> GetTargetsWithinDistance(float distance, float searchAngle = 0, bool getNearestTarget = false)
    {
        List<TargetObject> objectsWithinDistance = new List<TargetObject>();
        TargetObject nearestTarget = null;
        float minDistance = distance;

        foreach(TargetObject targetObject in objects)
        {
            // Search within searchAngle
            if(searchAngle != 0)
            {
                Vector3 direction = playerAircraft.transform.forward;
                Vector3 diff = targetObject.transform.position - playerAircraft.transform.position;
                if(Vector3.Angle(diff, direction) > searchAngle) continue;
            }

            float targetDistance = Vector3.Distance(targetObject.transform.position, playerAircraft.transform.position);
            
            if(targetDistance < distance)
            {
                if(getNearestTarget == true && targetDistance < minDistance)
                {
                    nearestTarget = targetObject;
                    minDistance = targetDistance;
                }
                objectsWithinDistance.Add(targetObject);
            }
            else
            {
                targetObject.isNextTarget = false;
            }
        }

        if(getNearestTarget == true)
        {
            objectsWithinDistance.Clear();
            objectsWithinDistance.Add(nearestTarget);
        }
        return objectsWithinDistance;
    }
}
