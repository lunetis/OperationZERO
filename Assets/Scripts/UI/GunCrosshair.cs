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

    public void SetTarget(Transform target)
    {
        if(target == null)
        {
            crosshairUI.SetActive(false);
        }
        this.target = target;
    }

    protected override void Start()
    {
        base.Start();
        reciprocal = 1 / visibleDistance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(target == null) return;

        float distance = Vector3.Distance(GameManager.PlayerAircraft.transform.position, target.position);
        float fillAmount = distance * reciprocal;
        Vector2 aircraftRotation = GameManager.PlayerAircraft.RotateValue;
        Vector3 convertedPosition = new Vector3(-aircraftRotation.y * offset.x * fillAmount, aircraftRotation.x * offset.y * fillAmount, zDistance);
        convertedPosition *= distance * reciprocal;
        transform.localPosition = Vector3.Lerp(transform.localPosition, convertedPosition, lerpAmount);

        if(distance < visibleDistance)
        {
            crosshairUI.SetActive(true);
            fillImage.fillAmount = fillAmount;
        }
        else
        {
            crosshairUI.SetActive(false);
        }
    }
}
