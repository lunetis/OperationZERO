using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSignController : MonoBehaviour
{
    [SerializeField]
    GameObject warningSign;
    [SerializeField]
    float repeatTime = 0.3f;

    void OnEnable()
    {
        warningSign.SetActive(true);
        InvokeRepeating("Blink", repeatTime, repeatTime);
    }

    void OnDisable()
    {
        warningSign.SetActive(false);
        CancelInvoke("Blink");
    }

    void Blink()
    {
        warningSign.SetActive(!warningSign.activeInHierarchy);
    }
}
