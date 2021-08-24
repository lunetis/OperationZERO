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

    BorderIndicator borderIndicator;

    [SerializeField]
    bool showBorderIndicator = false;
    
    [SerializeField]
    float blinkRepeatTime = 0.2f;

    float initSize;


    public void SetMinimapSpriteVisible(bool visible)
    {
        spriteRenderer.enabled = visible;
    }

    public void SetMinimapSpriteBlink(bool blink)
    {
        if(blink == true)
        {
            InvokeRepeating("Blink", blinkRepeatTime, blinkRepeatTime);
        }
        else
        {
            CancelInvoke();
            spriteRenderer.color = Color.white;
        }
    }

    void Blink()
    {
        spriteRenderer.color = (spriteRenderer.color == Color.white) ? Color.clear : Color.white;
    }

    void OnDestroy()
    {
        if(borderIndicator != null)
        {
            borderIndicator.gameObject.SetActive(false);
        }
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
                if(borderIndicator == null)
                {
                    GameObject borderIndicatorObject = GameManager.Instance.borderIncicatorObjectPool.GetPooledObject();
                    borderIndicator = borderIndicatorObject.GetComponent<BorderIndicator>();
                    borderIndicator.Target = transform.parent;
                    borderIndicatorObject.SetActive(true);
                }
                borderIndicator?.gameObject.SetActive(true);
            }
            else
            {
                borderIndicator?.gameObject.SetActive(false);
            }
        }
    }
}
