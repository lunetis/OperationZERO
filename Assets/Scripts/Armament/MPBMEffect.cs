using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBMEffect : MonoBehaviour
{
    [SerializeField]
    float damage;

    [SerializeField]
    float damageDistance;

    [SerializeField]
    float damageRepeatTime = 0.3f;
    float damageTimer;

    AudioSource audioSource;

    void ApplyDamage()
    {
        if(GameManager.PlayerAircraft == null) return;

        float distance = Vector3.Distance(transform.position, GameManager.PlayerAircraft.transform.position);
        if(distance < damageDistance && damageTimer <= 0)
        {
            GameManager.PlayerAircraft.OnDamage(damage, gameObject.layer);
            damageTimer = damageRepeatTime;
        }
    }

    void OnEnable()
    {
        audioSource.Play();
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        damageTimer = 0;
    }

    void Update()
    {
        ApplyDamage();

        if(damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }
}
