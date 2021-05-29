using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEngineController : MonoBehaviour
{
    ParticleSystem.MainModule particleMainModule;
    float initAlpha;
    float throttleAmount;
    Color particleColor;

    [SerializeField]
    float accelLerpAmount;
    [SerializeField]
    float brakeLerpAmount;

    float inputValue;
    public float InputValue
    {
        set { inputValue = value; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        particleMainModule = GetComponent<ParticleSystem>().main;
        particleColor = particleMainModule.startColor.color;
        initAlpha = particleColor.a;
        throttleAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float lerpAmount = (throttleAmount > inputValue) ? brakeLerpAmount : accelLerpAmount;
        throttleAmount = Mathf.Lerp(throttleAmount, inputValue, lerpAmount * Time.deltaTime);
        particleColor.a = throttleAmount * initAlpha;
        particleMainModule.startColor = particleColor;
    }
}
