using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MinimapSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    MinimapController minimapController;
    public float iconSize;
    public float depth;

    public bool showBorderIndicator;

    float initSize;

    public void SetMinimapSpriteVisible(bool visible)
    {
        spriteRenderer.enabled = visible;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        minimapController = GameManager.UIController.MinimapController;
        initSize = iconSize / minimapController.smallViewSize;
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
