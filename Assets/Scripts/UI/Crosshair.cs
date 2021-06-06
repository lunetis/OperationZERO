using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    protected Vector2 offset;
    [SerializeField]
    protected float lerpAmount;
    protected float zDistance;

    protected virtual void Start()
    {
        zDistance = transform.localPosition.z;
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        Vector2 aircraftRotation = GameManager.AircraftController.RotateValue;
        Vector3 convertedPosition = new Vector3(-aircraftRotation.y * offset.x, aircraftRotation.x * offset.y, zDistance);
        transform.localPosition = Vector3.Lerp(transform.localPosition, convertedPosition, lerpAmount);
    }
}
