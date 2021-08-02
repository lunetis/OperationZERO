using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2ParticleController : MonoBehaviour
{
    [SerializeField]
    ParticleSystem smokeEffect;

    public void RemoveSmokeEffect()
    {
        smokeEffect.Clear();
    }
}
