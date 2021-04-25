using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MinimapSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public MinimapController minimapController;
    public float iconSize;
    public float depth;

    public bool showBorderIndicator;

    float initSize;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initSize = iconSize / minimapController.GetCameraViewSize();
        depth *= 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        float scale = initSize * minimapController.GetIconResizeFactor();

        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
        transform.position = new Vector3(transform.parent.position.x, depth, transform.parent.position.z);
        transform.localScale = new Vector3(scale, scale, scale);

        if(showBorderIndicator == true)
        {
            if(spriteRenderer.isVisible == false)
            {
                minimapController.ShowBorderIndicator(transform.position);
            }
            else
            {
                minimapController.HideBorderIncitator();
            }
        }
    }
}
