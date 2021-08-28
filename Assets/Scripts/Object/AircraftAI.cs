using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftAI : TargetObject
{
    [Header("Aircraft Settings")]
    [SerializeField]
    float maxSpeed = 200;
    [SerializeField]
    float minSpeed = 60;
    [SerializeField]
    float defaultSpeed = 90;

    float speed;
    float targetSpeed;
    bool isAcceleration;
    
    [Header("Accel/Rotate Values")]
    [SerializeField]
    float accelerateLerpAmount = 1.0f;
    [SerializeField]
    float accelerateAmount = 20.0f;
    float currentAccelerate;
    float accelerateReciprocal;

    [SerializeField]
    float turningForce = 1.0f;
    float currentTurningForce;
    
    [Header("Z Rotate Values")]
    [SerializeField]
    float zRotateMaxThreshold = 0.3f;
    [SerializeField]
    float zRotateAmount = 90;
    [SerializeField]
    float zRotateLerpAmount = 1.5f;

    float turningTime;
    float currentTurningTime;

    [Header("Waypoint")]
    [SerializeField]
    List<Transform> initialWaypoints;
    Queue<Transform> waypointQueue;
    
    [SerializeField]
    protected float waypointMinHeight = 250;
    [SerializeField]
    protected float waypointMaxHeight = 1000;

    [SerializeField]
    BoxCollider areaCollider;

    protected Vector3 currentWaypoint;
    
    float prevWaypointDistance;
    float waypointDistance;
    bool isComingClose;

    float prevRotY;
    float currRotY;
    float rotateAmount;
    float zRotateValue;

    [Header("Misc.")]
    [SerializeField]
    [Range(0, 1)]
    float evasionRate = 0.5f;
    
    [SerializeField]
    float newWaypointDistance = 500;

    [SerializeField]
    float minimumWaypointChangeDelay = 2.0f;
    float waypointChangeDelay = 0;


    [SerializeField]
    List<JetEngineController> jetEngineControllers;

    [Space(10)]    
    [Header("DEBUG")]
    [SerializeField]
    GameObject waypointDebugObject;
    [SerializeField]
    DebugText debugText;

    Rigidbody rb;

    public float Speed
    {
        get { return speed; }
    }

    public void ForceChangeWaypoint(Vector3 waypoint)
    {
        currentWaypoint = waypoint;
        Invoke("Phase3ChangeWaypoint", 0.5f);
    }

    void Phase3ChangeWaypoint()
    {
        currentWaypoint = GameManager.PlayerAircraft.transform.position;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    // If you want to change waypoint selection, override this function
    protected virtual Vector3 CreateWaypoint()
    {
        if(areaCollider != null)
            return CreateWaypointWithinArea();
        else
            return CreateWaypointAroundItself();
    }

    void RandomizeSpeedAndTurn()
    {
        // Speed
        targetSpeed = Random.Range(minSpeed, maxSpeed);
        isAcceleration = (speed < targetSpeed);

        // TurningForce
        currentTurningForce = Random.Range(0.5f * turningForce, turningForce);
        turningTime = 1 / currentTurningForce;
    }

    Vector3 CreateWaypointWithinArea()
    {
        if(areaCollider == null) return currentWaypoint;
        
        float height = Random.Range(waypointMinHeight, waypointMaxHeight);
        Vector3 waypointPosition = RandomPointInBounds(areaCollider.bounds);

        RaycastHit hit;
        Physics.Raycast(waypointPosition, Vector3.down, out hit);

        if(hit.distance != 0)
        {
            waypointPosition.y += height - hit.distance;
        }
        // New waypoint is below ground
        else
        {
            Physics.Raycast(waypointPosition, Vector3.up, out hit);
            
            if(hit.distance == 0)
            {
                waypointPosition.y = height;
            }
            else
            {
                waypointPosition.y += height + hit.distance;
            }
        }

        return waypointPosition;
    }

    Vector3 CreateWaypointAroundItself()
    {
        float distance = Random.Range(newWaypointDistance * 0.7f, newWaypointDistance);
        float height = Random.Range(waypointMinHeight, waypointMaxHeight);
        float angle = Random.Range(0, 360);
        Vector3 directionVector = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        Vector3 waypointPosition = transform.position + directionVector * distance;
        Vector3 raycastPosition = waypointPosition;
        raycastPosition.y = 5000;

        RaycastHit hit;
        Physics.Raycast(raycastPosition, Vector3.down, out hit);

        if(hit.distance != 0)
        {
            waypointPosition.y += height - (5000 - hit.distance);
        }
        // New waypoint is outside of the map
        else
        {
            waypointPosition.y = height;
        }

        return waypointPosition;
    }

    protected void ChangeWaypoint()
    {
        waypointChangeDelay = minimumWaypointChangeDelay;
        
        if(waypointQueue.Count == 0)
        {
            currentWaypoint = CreateWaypoint();
        }
        else
        {
            currentWaypoint = waypointQueue.Dequeue().position;
        }

        if(waypointDebugObject != null)
        {
            Instantiate(waypointDebugObject, currentWaypoint, Quaternion.identity);
        }
        
        currentWaypoint.y = Mathf.Clamp(currentWaypoint.y, waypointMinHeight, waypointMaxHeight);
        
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);
        prevWaypointDistance = waypointDistance;
        isComingClose = false;

        RandomizeSpeedAndTurn();
    }

    void CheckWaypoint()
    {
        if(currentWaypoint == null) return;
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);

        if(debugText)
        {
            debugText.AddText("Dist : " + waypointDistance + " // prev : " + prevWaypointDistance);
            debugText.AddText("Waypoint : " + currentWaypoint);
            debugText.AddText("isComingClose : " + isComingClose);
        }

        if(waypointDistance > prevWaypointDistance) // Aircraft is going farther from the waypoint
        {
            if(isComingClose == true && waypointChangeDelay <= 0)
            {
                ChangeWaypoint();
            }
        }
        else
        {
            isComingClose = true;
        }

        prevWaypointDistance = waypointDistance;
    }

    void Rotate()
    {
        if(currentWaypoint == Vector3.zero)
            return;

        Vector3 targetDir = currentWaypoint - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);

        float delta = Quaternion.Angle(transform.rotation, lookRotation);
        if (delta > 0f)
        {
            float lerpAmount = Mathf.SmoothDampAngle(delta, 0.0f, ref rotateAmount, currentTurningTime);
            lerpAmount = 1.0f - (lerpAmount / delta);
            
            Vector3 eulerAngle = lookRotation.eulerAngles;
            eulerAngle.z += zRotateValue * zRotateAmount;
            lookRotation = Quaternion.Euler(eulerAngle);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lerpAmount);
        }
    }

    void ZAxisRotate()
    {
        currRotY = transform.eulerAngles.y;
        float diff = prevRotY - currRotY;

        if(diff > 180) diff -= 360;
        if(diff < -180) diff += 360;
        
        prevRotY = transform.eulerAngles.y;
        zRotateValue = Mathf.Lerp(zRotateValue, Mathf.Clamp(diff / zRotateMaxThreshold, -1, 1), zRotateLerpAmount * Time.fixedDeltaTime);
    }


    void AdjustSpeed()
    {
        currentAccelerate = 0;
        if(isAcceleration == true && speed < targetSpeed)
        {
            currentAccelerate = accelerateAmount;
        }
        else if(isAcceleration == false && speed > targetSpeed)
        {
            currentAccelerate = -accelerateAmount;
        }
        speed += currentAccelerate * Time.fixedDeltaTime;

        currentTurningTime = Mathf.Lerp(currentTurningTime, turningTime, Time.fixedDeltaTime);
    }

    void Move()
    {
        rb.velocity = transform.forward * speed;
    }

    void JetEngineControl()
    {
        foreach(JetEngineController jet in jetEngineControllers)
        {
            jet.InputValue = currentAccelerate * accelerateReciprocal;
        }
    }


    public override void OnMissileAlert()
    {
        float rate = Random.Range(0.0f, 1.0f);
        if(rate <= evasionRate)
        {
            ChangeWaypoint();
        }
    }

    protected override void AdjustValuesByDifficulty()
    {
        float evasionRateMultiplyFactor = MissionData.GetFloatFromDifficultyXML("evasionRateMultiplyFactor");
        evasionRate = Mathf.Clamp01(evasionRate * evasionRateMultiplyFactor);
        float turningForceFactor = MissionData.GetFloatFromDifficultyXML("aiAircraftTurningForceFactor");
        turningForce *= turningForceFactor;
    }

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();

        speed = targetSpeed = defaultSpeed;

        accelerateReciprocal = 1 / accelerateAmount;

        currentTurningForce = turningForce;
        currentTurningTime = turningTime = 1 / turningForce;

        prevRotY = 0;
        currRotY = 0;

        waypointQueue = new Queue<Transform>();

        if(initialWaypoints.Count > 0)
        {
            foreach(Transform t in initialWaypoints)
            {
                waypointQueue.Enqueue(t);
            }
        }
        ChangeWaypoint();
    }

    protected virtual void Update()
    {
        CheckWaypoint();
        JetEngineControl();
        CheckMissileDistance();

        if(waypointChangeDelay > 0) waypointChangeDelay -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ZAxisRotate();
        AdjustSpeed();
        Move();
        Rotate();
    }
}
