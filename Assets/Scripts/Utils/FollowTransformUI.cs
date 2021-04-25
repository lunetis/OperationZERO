using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformUI : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;
    Camera cam;

    RectTransform rectTransform;
    RectTransform canvasRect;

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
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetCanvas(transform.parent);
        if(canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        cam = GameManager.Instance.cameraController.GetActiveCamera();
        Vector3 screenPosition = cam.WorldToScreenPoint(targetTransform.position);
        float distance = GameManager.Instance.GetDistanceFromPlayer(targetTransform);

        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, targetTransform.position);
            rectTransform.anchoredPosition = screenPoint - canvasRect.sizeDelta * 0.5f;
        }
    }
}
