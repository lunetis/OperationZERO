using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftAI : MonoBehaviour
{
    [Header("Aircraft Settings")]
    [SerializeField]
    float maxSpeed = 300;
    [SerializeField]
    float minSpeed = 30;
    [SerializeField]
    float defaultSpeed = 60;

    float speed;
    
    [Header("Accel/Rotate Values")]
    [SerializeField]
    float speedLerpAmount = 1.0f;
    [SerializeField]
    float turningForce = 1.0f;

    float currentTurningForce;
    
    [Header("Z Rotate Values")]
    [SerializeField]
    float zRotateMaxThreshold = 0.3f;
    [SerializeField]
    float zRotateAmount = 135;
    [SerializeField]
    float zRotateLerpAmount = 1.5f;

    float turningTime;
    float currentTurningTime;

    [Header("Waypoint")]
    [SerializeField]
    List<Transform> initialWaypoints;
    Queue<Transform> waypointQueue;
    
    [SerializeField]
    float waypointMinHeight;
    [SerializeField]
    float waypointMaxHeight;

    [SerializeField]
    BoxCollider areaCollider;

    Vector3 currentWaypoint;
    
    float prevWaypointDistance;
    float waypointDistance;
    bool isComingClose;

    float prevRotY;
    float currRotY;
    float rotateAmount;
    float zRotateValue;

    [Header("DEBUG")]
    [SerializeField]
    GameObject waypointObject;

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    void CreateWaypoint()
    {
        if(areaCollider == null) return;
        
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
            waypointPosition.y += height + hit.distance;
        }

        if(waypointObject != null)
        {
            Instantiate(waypointObject, waypointPosition, Quaternion.identity);
        }
        currentWaypoint = waypointPosition;
    }

    void ChangeWaypoint()
    {
        if(waypointQueue.Count == 0)
        {
            CreateWaypoint();
        }
        else
        {
            currentWaypoint = waypointQueue.Dequeue().position;
        }
        
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);
        prevWaypointDistance = waypointDistance;
        isComingClose = false;

        currentTurningForce = Random.Range(0.5f * turningForce, turningForce);
        turningTime = 1 / currentTurningForce;
    }

    void CheckWaypoint()
    {
        if(currentWaypoint == null) return;
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);

        if(waypointDistance >= prevWaypointDistance) // Aircraft is going farther from the waypoint
        {
            if(isComingClose == true)
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
        zRotateValue = Mathf.Lerp(zRotateValue, Mathf.Clamp(diff / zRotateMaxThreshold, -1, 1), zRotateLerpAmount * Time.deltaTime);
    }

    void Move()
    {
        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }


    void Start()
    {
        speed = defaultSpeed;

        currentTurningForce = turningForce;
        turningTime = 1 / turningForce;
        currentTurningTime = turningTime;

        prevRotY = 0;
        currRotY = 0;

        waypointQueue = new Queue<Transform>();
        foreach(Transform t in initialWaypoints)
        {
            waypointQueue.Enqueue(t);
        }
        ChangeWaypoint();
    }

    void Update()
    {
        CheckWaypoint();
        ZAxisRotate();
        Rotate();
        Move();

        currentTurningTime = Mathf.Lerp(currentTurningTime, turningTime, 1);
    }
}
