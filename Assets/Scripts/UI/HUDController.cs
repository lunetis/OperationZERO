using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Transform target;

    public RectTransform hudRotationTransform;
    public RectTransform hudPositionTransform;
    RectTransform rectTransform;
    public float HUDHeight;
    float HUDPositionFactor;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        HUDPositionFactor = HUDHeight / 90;
    }

    // Update is called once per frame
    void Update()
    {
        float convertedRotation = (target.eulerAngles.x > 180) ? (target.eulerAngles.x - 360) : target.eulerAngles.x;
        hudPositionTransform.localPosition = new Vector3(0, (convertedRotation * HUDPositionFactor), 0);
        hudRotationTransform.rotation = Quaternion.Euler(0, 0, -target.eulerAngles.z);
    }
}
