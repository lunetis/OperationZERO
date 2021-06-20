using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudio : MonoBehaviour
{
    AudioSource audioSource;

    void PlayExplosionClip()
    {
        audioSource.PlayOneShot(SoundManager.Instance.GetExplosionClip());
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        float distance = GameManager.Instance.GetDistanceFromPlayer(transform);
        Invoke("PlayExplosionClip", distance * SoundManager.Instance.DistanceMultiplier);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
