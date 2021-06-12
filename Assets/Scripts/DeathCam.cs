using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeathCam : MonoBehaviour
{
    CinemachineOrbitalTransposer orbitalTransposer;

    [SerializeField]
    AnimationClip deathCamInstantAnimation;
    [SerializeField]
    AnimationClip deathCamDelayedAnimation;
    Animation anim;
    
    CinemachineVirtualCamera vCam;
    float initCameraHeight;

    [SerializeField]
    float minimumOffset = 5;

    public void PlayAnimation(bool isInstantAnimation)
    {
        anim.clip = isInstantAnimation ? deathCamInstantAnimation : deathCamDelayedAnimation;
        anim.Play();
    }

    void AdjustHeight()
    {
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        
        Vector3 raycastPos = transform.position;
        raycastPos.y += 100;
        Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, layerMask);

        float minimumHeight = transform.position.y - (hit.distance - 100) + minimumOffset;
        float calculatedOffset = minimumHeight - orbitalTransposer.FollowTarget.position.y;

        if(orbitalTransposer.m_FollowOffset.y < calculatedOffset) orbitalTransposer.m_FollowOffset.y = calculatedOffset;
    }

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        anim = GetComponent<Animation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        orbitalTransposer = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        orbitalTransposer.m_XAxis.Value = Random.Range(0, 360);
        initCameraHeight = transform.position.y;
    }

    void Update()
    {
        orbitalTransposer.m_XAxis.m_InputAxisValue = 1;
        AdjustHeight();
    }
}
