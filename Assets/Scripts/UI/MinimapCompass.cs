using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCompass : MonoBehaviour
{
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    public void SetCompass(float eulerAngle)
    {
        rectTransform.rotation = Quaternion.Euler(0, 0, eulerAngle);
    }
}
