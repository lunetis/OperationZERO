using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    [SerializeField]
    List<AudioClip> missileLaunchClips;

    [SerializeField]
    List<AudioClip> explosionClips;
    
    [SerializeField]
    List<AudioClip> gunHitClips;

    [SerializeField]
    float distanceMultiplier = 0.001f;

    public float DistanceMultiplier
    {
        get { return distanceMultiplier; }
    }

    public AudioClip GetMissileLaunchClip()
    {
        return missileLaunchClips[Random.Range(0, missileLaunchClips.Count)];
    }
    
    public AudioClip GetExplosionClip()
    {
        return explosionClips[Random.Range(0, explosionClips.Count)];
    }

    public AudioClip GetGunHitClip()
    {
        return gunHitClips[Random.Range(0, gunHitClips.Count)];
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
