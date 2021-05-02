using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Transform parent;
    Rigidbody rb;

    Transform target;
    float speed;
    public string missileName;

    [Header("Properties")]
    public bool isSpecialWeapon;
    public float maxSpeed;
    public float accelAmount;
    public float turningForce;

    [Space(10)]
    public float boresightAngle;
    public float lifetime;

    [Space(10)]
    public float targetSearchSpeed;
    public float lockDistance;

    [Space(10)]
    public float cooldown;
    public int payload;

    // UI
    [Header("UI")]
    public Sprite missileFrameSprite;
    public Sprite missileFillSprite;
    
    // Effect
    GameObject smokeTrailEffect;
    public Transform smokeTrailPosition;

    public void Launch(Transform target, float launchSpeed, int layer)
    {
        this.target = target;
        speed = launchSpeed;
        gameObject.layer = layer;
    }

    void LookAtTarget()
    {
        if(target == null)
            return;

        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if(angle > boresightAngle)
        {
            target = null;
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        Explode();
        DisableMissile();
    }

    void Explode()
    {
        // Instantiate in world space
        GameObject effect = GameManager.Instance.explosionEffectObjectPool.GetPooledObject();
        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
        effect.SetActive(true);
    }

    void DisableMissile()
    {
        transform.parent = parent;
        gameObject.SetActive(false);
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        parent = transform.parent;
    }
    
    void OnEnable()
    {
        smokeTrailEffect = GameManager.Instance.smokeTrailEffectObjectPool.GetPooledObject();
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.transform.position = smokeTrailPosition.position;
            smokeTrailEffect.GetComponent<ParticleSystem>().Play();
            smokeTrailEffect.SetActive(true);
        }
        
        Invoke("DisableMissile", lifetime);
    }

    private void OnDisable()
    {
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<ParticleSystem>().Stop();
        }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        CancelInvoke("DisableMissile");
    }

    void FixedUpdate()
    {
        LookAtTarget();
        if(speed < maxSpeed)
        {
            speed += accelAmount * Time.fixedDeltaTime;
        }

        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.transform.position = smokeTrailPosition.position;
        }
    }
}
