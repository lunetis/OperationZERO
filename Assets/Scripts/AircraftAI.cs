using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftAI : MonoBehaviour
{
    [Header("Aircraft Settings")]
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    float minSpeed;
    [SerializeField]
    float defaultSpeed;

    float speed;
    
    [Header("Accel/Rotate Values")]
    [SerializeField]
    float speedLerpAmount;
    [SerializeField]
    float turningForce;
    
    [Header("Z Rotate Values")]
    [SerializeField]
    float zRotateMaxThreshold = 0.5f;
    [SerializeField]
    float zRotateAmount = 90;

    float turningTime;

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
    DebugText debugText;
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

        Instantiate(waypointObject, waypointPosition, Quaternion.identity);

        currentWaypoint = waypointPosition;
    }

    void ChangeWaypoint()
    {
        if(waypointQueue.Count == 0)
        {
            CreateWaypoint();
            Debug.Log("new waypoint");
        }
        else
        {
            currentWaypoint = waypointQueue.Dequeue().position;
        }
        
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);
        prevWaypointDistance = waypointDistance;
        isComingClose = false;
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
            float lerpAmount = Mathf.SmoothDampAngle(delta, 0.0f, ref rotateAmount, turningTime);
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
        zRotateValue = Mathf.Lerp(zRotateValue, Mathf.Clamp(diff / zRotateMaxThreshold, -1, 1), turningForce * Time.deltaTime);
    }

    void Move()
    {
        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }


    void Start()
    {
        speed = defaultSpeed;
        turningTime = 1 / turningForce;

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
    }
}
