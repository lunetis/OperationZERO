using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrail : MonoBehaviour
{
    Transform followTransform;
    ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void SetFollowTransform(Transform followTransform)
    {
        this.followTransform = followTransform;
        particle.Play();
    }

    public void StopFollow()
    {
        followTransform = null;
        particle.Stop();
    }

    void Update()
    {
        if(followTransform != null)
        {
            transform.position = followTransform.position;
        }
    }
}
