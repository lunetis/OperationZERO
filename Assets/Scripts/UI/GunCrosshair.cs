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

    Transform gunTransform;
    int objectLayer;

    float reciprocal;

    public void SetTarget(Transform target)
    {
        if(target == null)
        {
            crosshairUI.SetActive(false);
        }
        this.target = target;
    }

    public TargetObject CheckGunHit()
    {
        RaycastHit hit;
        Vector3 direction = transform.position - gunTransform.position;
        if(Physics.Raycast(transform.position, direction, out hit, visibleDistance, objectLayer) == true)
        {
            return hit.collider.GetComponent<TargetObject>();
        }
        else
        {
            return null;
        }
    }


    protected override void Start()
    {
        base.Start();
        reciprocal = 1 / visibleDistance;
        gunTransform = GameManager.WeaponController.GunTransform;
        objectLayer = 1 << LayerMask.NameToLayer("Object");
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(target == null) return;

        float distance = Vector3.Distance(GameManager.AircraftController.transform.position, target.position);
        float fillAmount = distance * reciprocal;
        Vector2 aircraftRotation = GameManager.AircraftController.RotateValue;
        Vector3 convertedPosition = new Vector3(-aircraftRotation.y * offset.x * fillAmount, aircraftRotation.x * offset.y * fillAmount, zDistance);

        convertedPosition *= fillAmount;
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
