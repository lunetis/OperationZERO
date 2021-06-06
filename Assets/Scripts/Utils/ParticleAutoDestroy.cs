using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    public float duration;
    void OnEnable()
    {
        // When the duration is undefined, get ParticleSystem's duration
        if(duration == 0)
        {
            duration = GetComponent<ParticleSystem>().main.duration;
        }
        Invoke("Disable", duration);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        
        CancelInvoke();
    }
}
