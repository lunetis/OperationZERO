using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHitAudio : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        audioSource.PlayOneShot(SoundManager.Instance.GetGunHitClip());
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
