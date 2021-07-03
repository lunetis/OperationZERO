using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBM : MonoBehaviour
{
    Rigidbody rb;
    Vector3 targetPosition;

    [Header("Missile Properties")]
    [SerializeField]
    float speed = 100;
    [SerializeField]
    float turningForce = 1;

    [Header("MPBM Properties")]
    [SerializeField]
    GameObject effectPrefab;
    [SerializeField]
    float explosionMinTime = 1.0f;
    [SerializeField]
    float explosionMaxTime = 2.0f;
    
    // Effect
    [SerializeField]
    Transform smokeTrailPosition;
    GameObject smokeTrailEffect;


    public void Launch(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        float explosionTimer = Random.Range(explosionMinTime, explosionMaxTime);
        Invoke("Explode", explosionTimer);
        rb.velocity = transform.forward * speed;

        smokeTrailEffect = GameManager.Instance.smokeTrailEffectObjectPool.GetPooledObject();
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<SmokeTrail>()?.SetFollowTransform(smokeTrailPosition);
            smokeTrailEffect.SetActive(true);
        }
    }

    void Explode()
    {
        GameObject mpbmEffect = GameManager.Instance.mpbmEffectObjectPool.GetPooledObject();
        mpbmEffect.transform.position = transform.position;
        mpbmEffect.transform.rotation = transform.rotation;
        mpbmEffect.SetActive(true);

        gameObject.SetActive(false);
    }

    void LookAtTarget()
    {
        Vector3 targetDir = targetPosition - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void DisableMissile()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<SmokeTrail>().StopFollow();
        }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        CancelInvoke();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtTarget();
    }
}
