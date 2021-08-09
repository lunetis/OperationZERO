using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformUI : MonoBehaviour
{
    [SerializeField]
    protected Transform targetTransform;

    [SerializeField]
    protected Camera cam;
    protected RectTransform rectTransform;

    [SerializeField]
    protected bool trackCurrentCamera = true;

    protected Vector2 screenSize;
    protected float screenAdjustFactor;

    // Recursive search
    Canvas GetCanvas(Transform parentTransform)
    {
        if(parentTransform.GetComponent<Canvas>() != null)
        {
            return parentTransform.GetComponent<Canvas>();
        }
        else
        {
            return GetCanvas(parentTransform.parent);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        screenSize = new Vector2(Screen.width, Screen.height);
        screenAdjustFactor = Mathf.Max((1920.0f / Screen.width), (1080.0f / Screen.height));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(targetTransform == null) return;
        
        if(trackCurrentCamera == true)
        {
            cam = GameManager.CameraController.GetActiveCamera();
        }

        Vector3 screenPosition = cam.WorldToScreenPoint(targetTransform.position);
        float distance = GameManager.Instance.GetDistanceFromPlayer(targetTransform);

        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, targetTransform.position);
            Vector2 position = screenPoint - screenSize * 0.5f;
            position *= screenAdjustFactor;
            rectTransform.anchoredPosition = position;
        }
    }
}
