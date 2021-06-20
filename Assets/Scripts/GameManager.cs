using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [Header("Object Pools")]
    public ObjectPool missileObjectPool;
    public ObjectPool specialWeaponObjectPool;
    public ObjectPool explosionEffectObjectPool;
    public ObjectPool smokeTrailEffectObjectPool;

    [SerializeField]
    ObjectPool smokeTrailDamagePool;

    public ObjectPool bulletObjectPool;
    public ObjectPool bulletHitEffectObjectPool;
    public ObjectPool groundHitEffectObjectPool;

    [Header("Game Properties")]
    public int timeLimit;
    int score;

    [Header("Controllers")]
    [SerializeField]
    UIController uiController;
    [SerializeField]
    AircraftController aircraftController;
    [SerializeField]
    PlayerAircraft playerAircraft;
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    TargetController targetController;
    [SerializeField]
    WeaponController weaponController;

    [Header("Game Over Control")]
    [SerializeField]
    List<GameObject> disableOnGameOver;
    [SerializeField]
    List<GameObject> disableOnGameOverObjects;
    [SerializeField]
    List<GameObject> enableOnGameOverObjects;

    [SerializeField]
    DeathCam deathCam;

    public DebugText debugText;

    Vector3 zeroVec = new Vector3(0, 0, 0);
    
    List<TargetObject> objects = new List<TargetObject>();

    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color warningColor;

    public static Color NormalColor
    {
        get { return Instance.normalColor; }
    }
    public static Color WarningColor
    {
        get { return Instance.warningColor; }
    }

    public static CameraController CameraController
    {
        get { return Instance?.cameraController; }
    }

    public static AircraftController AircraftController
    {
        get { return Instance?.aircraftController; }
    }

    public static PlayerAircraft PlayerAircraft
    {
        get { return Instance?.playerAircraft; }
    }

    public static UIController UIController
    {
        get { return Instance?.uiController; }
    }

    public static TargetController TargetController
    {
        get { return Instance?.targetController; }
    }

    public static WeaponController WeaponController
    {
        get { return Instance?.weaponController; }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
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

    public static float GetAngleBetweenTransform(Transform otherTransform)
    {
        Vector3 direction = PlayerAircraft.transform.forward;
        Vector3 diff = otherTransform.position - PlayerAircraft.transform.position;
        return Vector3.Angle(diff, direction);
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
                if(GetAngleBetweenTransform(targetObject.transform) > searchAngle) continue;
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

    public void CreateDamageSmokeEffect(Transform transform)
    {
        GameObject smokeTrailObject = smokeTrailDamagePool.GetPooledObject();
        if(smokeTrailObject != null)
        {
            smokeTrailObject.SetActive(true);
            smokeTrailObject.GetComponent<SmokeTrail>()?.SetFollowTransform(transform);
        }
    }

    public void GameOver(bool isDead, bool isInstantDeath = false)
    {
        // Set UI
        UIController.SetLabel(AlertUIController.LabelEnum.MissionFailed);
        
        foreach(TargetObject obj in objects)
        {
            obj.DeleteMinimapSprite();
        }
        targetController.RemoveAllTargetUI();
        objects.Clear();
        weaponController.ChangeTarget();

        foreach(GameObject obj in disableOnGameOver)
        {
            obj.SetActive(false);
        }

        if(isDead)
        {
            foreach(GameObject obj in disableOnGameOverObjects)
            {
                obj.SetActive(false);
            }
            foreach(GameObject obj in enableOnGameOverObjects)
            {
                obj.SetActive(true);
            }

            aircraftController.DisableControl();
            deathCam.PlayAnimation(isInstantDeath);
        }
    }

    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    public void Restart()
    {
        Debug.Log("restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    // DEBUG
    public static void PrintDebugText(string text)
    {
        Instance.debugText.AddText(text);
    }
}
