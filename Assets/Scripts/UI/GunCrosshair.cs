using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunCrosshair : Crosshair
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float visibleDistance;


    [SerializeField]
    GameObject crosshairUI;
    [SerializeField]
    Image fillImage;

    float reciprocal;

    protected override void Start()
    {
        base.Start();
        reciprocal = 1 / visibleDistance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float distance = Vector3.Distance(GameManager.PlayerAircraft.transform.position, target.position);
        Vector2 aircraftRotation = GameManager.PlayerAircraft.RotateValue;
        Vector3 convertedPosition = new Vector3(-aircraftRotation.y * offset.x, aircraftRotation.x * offset.y, zDistance);
        convertedPosition *= distance * reciprocal;
        transform.localPosition = Vector3.Lerp(transform.localPosition, convertedPosition, lerpAmount);

        if(distance < visibleDistance)
        {
            crosshairUI.SetActive(true);
            fillImage.fillAmount = distance * reciprocal;
        }
        else
        {
            crosshairUI.SetActive(false);
        }
    }
}
