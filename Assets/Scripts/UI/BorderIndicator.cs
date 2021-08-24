using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BorderIndicator : MonoBehaviour
{
    Transform target;
    SpriteRenderer spriteRenderer;
    Camera minimapCamera;
    MinimapController minimapController;
    
    [SerializeField]
    float iconSize;
    [SerializeField]
    float depth;

    float sizeReciprocal;

    public Transform Target
    {
        set { target = value; }
    }

    public void SetPosition()
    {
        if(target == null) return;
        
        float reciprocal;
        float rotation;
        Vector3 position = target.transform.position;
        Vector2 distance = new Vector3(minimapCamera.transform.position.x - position.x, minimapCamera.transform.position.z - position.z);

        // When the x, z positions are same
        if(distance.x == 0 || distance.y == 0)
            return;

        if(minimapController.GetMinimapIndex() == MinimapController.MinimapIndex.Small)
        {
            distance = Quaternion.Euler(0, 0, GameManager.AircraftController.transform.eulerAngles.y) * distance;
        }
        
        // X axis
        if(Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            reciprocal = -Mathf.Abs(minimapController.CameraSize.x / distance.x);
            rotation = (distance.x > 0) ? 90 : -90;
        }
        // Y axis
        else
        {
            reciprocal = -Mathf.Abs(minimapController.CameraSize.y / distance.y);
            rotation = (distance.y > 0) ? 180 : 0;
        }
        
        // change indicator
        float scale = sizeReciprocal * minimapCamera.orthographicSize;

        transform.localScale = new Vector3(scale, scale, scale);
        transform.localPosition = new Vector3(distance.x * reciprocal, distance.y * reciprocal, 1);
        transform.localEulerAngles = new Vector3(0, 0, rotation);
        
        if(gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        minimapController = GameManager.UIController.MinimapController;
        minimapCamera = minimapController.minimapCamera;
        sizeReciprocal = iconSize / minimapCamera.orthographicSize;
    }

    void Update()
    {
        SetPosition();
    }
}
