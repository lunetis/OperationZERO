using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Transform parent;
    Rigidbody rb;

    TargetObject target;
    float speed;
    public string missileName;

    [Header("Properties")]

    [SerializeField]
    float damage;

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

    bool isHit = false;
    bool isDisabled = false;
    bool hasWarned = false;

    public bool HasWarned
    {
        get { return hasWarned; }
        set { hasWarned = value; }
    }

    public void Launch(TargetObject target, float launchSpeed, int layer)
    {
        this.target = target;

        // Send Message to object that it is locked on
        target?.AddLockedMissile(this);

        speed = launchSpeed;
        gameObject.layer = layer;
        
        smokeTrailEffect = GameManager.Instance.smokeTrailEffectObjectPool.GetPooledObject();
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.SetActive(true);
            smokeTrailEffect.GetComponent<SmokeTrail>()?.SetFollowTransform(smokeTrailPosition);
        }
        
        Invoke("DisableMissile", lifetime);
    }

    void LookAtTarget()
    {
        if(target == null)
            return;

        Vector3 targetDir = target.transform.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if(angle > boresightAngle)
        {
            GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Missed);
            isDisabled = true;
            
            // Send Message to object that it is no more locked on
            target.RemoveLockedMissile(this);
            target = null;
            
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turningForce * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if(target != null && other.gameObject == target.gameObject)
        {
            isHit = true;
        }
        other.gameObject.GetComponent<TargetObject>()?.OnDamage(damage, gameObject.layer);

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
        hasWarned = false;
        
        // Send Message to object that it is no more locked on
        if(target != null)
        {
            target.RemoveLockedMissile(this);

            if(isDisabled == false && isHit == false)
            {
                GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Missed);
            }
        }
        
        transform.parent = parent;
        gameObject.SetActive(false);
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        parent = transform.parent;
    }
    

    private void OnDisable()
    {
        if(smokeTrailEffect != null)
        {
            smokeTrailEffect.GetComponent<SmokeTrail>().StopFollow();
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
}
