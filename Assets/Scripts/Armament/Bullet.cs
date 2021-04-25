using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform parent;
    TrailRenderer trailRenderer;
    Rigidbody rb;

    public float speed;
    public float lifetime;

    public void Fire(float launchSpeed, int layer)
    {
        speed += launchSpeed;
        gameObject.layer = layer;
        rb.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision other)
    {
        ObjectPool effectPool;
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            effectPool = GameManager.Instance.groundHitEffectObjectPool;
        }
        else
        {
            effectPool = GameManager.Instance.bulletHitEffectObjectPool;
        }
        CreateHitEffect(effectPool);
        DisableBullet();
    }

    void CreateHitEffect(ObjectPool effectPool)
    {
        // Instantiate in world space
        GameObject effect = effectPool.GetPooledObject();
        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
        effect.SetActive(true);
    }

    void DisableBullet()
    {
        transform.parent = parent;
        gameObject.SetActive(false);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        parent = transform.parent;
    }
    
    void OnEnable()
    {
        trailRenderer.Clear();
        Invoke("DisableBullet", lifetime);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        CancelInvoke("DisableBullet");
    }
}
