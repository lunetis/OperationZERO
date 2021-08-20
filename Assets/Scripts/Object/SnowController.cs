using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SnowController : MonoBehaviour
{
    Camera currentActiveCamera;

    [SerializeField]
    ParticleSystem snowParticle;
    
    [SerializeField]
    float heightOffset = 20;
    Vector3 offsetVector;

    [SerializeField]
    Rigidbody followTargetRigidbody;

    [SerializeField]
    CinemachineBrain cinemachine;
    
    bool isPlayingCutscene = false;
    public bool IsPlayingCutscene
    {
        set
        {
            isPlayingCutscene = value;

            if(isPlayingCutscene == true)
            {
                var velocity = snowParticle.velocityOverLifetime;
                velocity.x = 0;
                velocity.y = -50;
                velocity.z = 0;

            }
            var collision = snowParticle.collision;
            collision.enabled = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentActiveCamera = Camera.main;
        offsetVector = new Vector3(0, heightOffset, 0);
    }


    // Update is called once per frame
    void Update()
    {
        if(isPlayingCutscene)
        {
            Vector3 cameraPosition = cinemachine.ActiveVirtualCamera.VirtualCameraGameObject.transform.position;
            transform.position = cameraPosition + offsetVector;
        }
        else if(followTargetRigidbody != null)
        {
            Vector3 targetVelocity = followTargetRigidbody.velocity;

            // Particle Velocity
            var velocity = snowParticle.velocityOverLifetime;
            velocity.x = -targetVelocity.x;
            velocity.y = -targetVelocity.y - 50;
            velocity.z = -targetVelocity.z;

            targetVelocity *= 0.5f;
            targetVelocity.y += heightOffset;

            transform.position = followTargetRigidbody.transform.position + targetVelocity;
        }
    }
}
