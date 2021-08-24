using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixyTLS : MonoBehaviour
{
    [Header("TLS Properties")]
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Transform laserTransform;
    [SerializeField]
    float laserActivateTime;
    [SerializeField]
    float laserCooldownTime;

    [SerializeField]
    float laserRotateLerpAmount = 1;
    
    [SerializeField]
    float distance = 2000;

    [SerializeField]
    float damage = 50;

    
    [Header("Sounds")]
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource playerAudioSource;

    [SerializeField]
    List<AudioClip> tlsDamageAudioClips;

    [SerializeField]
    float audioLerpAmount = 5;
    
    [SerializeField]
    float lineWidthLerpAmount = 2;

    bool isActivated;
    float lineWidth;

    Vector3 laserTargetPosition;
    TargetObject laserHitTargetObject;
    

    void ActivateTLS()
    {
        isActivated = true;

        lineWidth = 0.5f;
        laserTargetPosition = laserTransform.position + laserTransform.right * distance;
        lineRenderer.SetPosition(1, laserTargetPosition);
        Invoke("DeactivateTLS", laserActivateTime);
        InvokeRepeating("DamageToTargetObject", 0, 0.1f);
    }

    void DeactivateTLS()
    {
        isActivated = false;
        
        Invoke("ActivateTLS", laserCooldownTime);
        CancelInvoke("DamageToTargetObject");
    }

    void DamageToTargetObject()
    {
        if(laserHitTargetObject == null)
        {
            return;
        }
        laserHitTargetObject.OnDamage(damage, gameObject.layer);
        playerAudioSource.PlayOneShot(tlsDamageAudioClips[Random.Range(0, tlsDamageAudioClips.Count)]);
    }

    void RotateTLS()
    {
        if(GameManager.PlayerAircraft == null) return;
        
        Vector3 launchPosition = laserTransform.position;
        laserTargetPosition = Vector3.Lerp(laserTargetPosition, GameManager.PlayerAircraft.transform.position, laserRotateLerpAmount * Time.deltaTime);
        Vector3 directionVector = (laserTargetPosition - launchPosition).normalized;

        // Damage
        RaycastHit hit;
        Physics.Raycast(lineRenderer.GetPosition(0), directionVector, out hit, distance);

        float lineDistance = distance;

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                lineDistance = hit.distance;
            }

            if(isActivated == true && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(laserHitTargetObject == null)
                {
                    laserHitTargetObject = hit.collider.GetComponent<TargetObject>();
                }
            }
            else
            {
                laserHitTargetObject = null;
            }
        }
        else
        {
            laserHitTargetObject = null;
        }

        lineRenderer.SetPosition(0, launchPosition);
        lineRenderer.SetPosition(1, launchPosition + directionVector * lineDistance);
    }

    void AdjustWidth()
    {
        float targetWidth = (isActivated == true) ? 1 : 0;
        lineWidth = Mathf.Lerp(lineWidth, targetWidth, lineWidthLerpAmount * Time.deltaTime);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    void AdjustVolume()
    {
        float targetVolume = (isActivated == true) ? 1 : 0;
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, audioLerpAmount * Time.deltaTime);
    }

    public void DisableTLS()
    {
        CancelInvoke();
        isActivated = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        DeactivateTLS();
    }

    // Update is called once per frame
    void Update()
    {
        RotateTLS();

        AdjustWidth();
        AdjustVolume();
    }
}
