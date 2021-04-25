using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixUIRotation : MonoBehaviour
{
    RectTransform rectTransform;
    public RectTransform parentTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.rotation = Quaternion.Euler(0, 0, parentTransform.rotation.z);
    }
}
