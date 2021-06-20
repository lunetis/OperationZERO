using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    [SerializeField]
    float lerpAmount = 5.0f;
    [SerializeField]
    float maxVolume = 0.2f;
    [SerializeField]
    float minPitch = 0.5f;

    [SerializeField]
    AudioSource gunFireAudioSource;

    bool isFiring = false;
    public bool IsFiring
    {
        set
        {
            if(value == true)
            {
                gunFireAudioSource.volume = maxVolume;
                gunFireAudioSource.pitch = minPitch;
            }

            isFiring = value;
        }
    }

    void Awake()
    {
        gunFireAudioSource = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isFiring == false) gunFireAudioSource.volume = Mathf.Lerp(gunFireAudioSource.volume, 0, lerpAmount * Time.deltaTime);
        gunFireAudioSource.pitch = Mathf.Lerp(gunFireAudioSource.pitch, 1, lerpAmount * Time.deltaTime);
    }
}
