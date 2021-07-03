using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECMSystem : MonoBehaviour
{
    TargetObject targetObject;

    [SerializeField]
    Transform ecmSystemTransform;

    [SerializeField]
    float evadeDistance;

    List<Missile> evadableMissiles = new List<Missile>();

    void EvadeMissile(Missile missileScript)
    {
        Vector3 randomDirection = transform.up;

        randomDirection = Quaternion.Euler(0, 0, Random.Range(0, 360)) * randomDirection;
        missileScript.EvadeByECM(transform.position + randomDirection * 100);
    }

    void CheckMissiles()
    {
        // Check Missiles' distance and angle
        foreach(Missile missile in targetObject.LockedMissiles)
        {
            float distance = Vector3.Distance(missile.transform.position, ecmSystemTransform.position);

            if(distance > evadeDistance) continue;

            float angle = Vector3.Angle(missile.transform.forward, transform.forward);
            if(angle < 90)
            {
                evadableMissiles.Add(missile);
            }
        }

        // Disable missile
        if(evadableMissiles.Count == 0) return;
        foreach(Missile missile in evadableMissiles)
        {
            EvadeMissile(missile);
        }
        evadableMissiles.Clear();
    }

    void Awake()
    {
        targetObject = GetComponent<TargetObject>();
    }

    void Update()
    {
        CheckMissiles();
    }
}
