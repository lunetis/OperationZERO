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
}
