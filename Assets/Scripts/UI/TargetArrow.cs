using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetArrow : MonoBehaviour
{
    TargetObject targetObject;

	[Header("Arrow Transforms")]
	[SerializeField]
    Transform cameraAttachedTransform;
	[SerializeField]
    Transform arrowTransform;

	[Header("Arrow Properties")]
	[SerializeField]
    int lineWidth = 3;
	[SerializeField]
	Material lineMaterial;
	[SerializeField]
    Transform[] vertexTransforms;

	[Header("Text UI")]
	[SerializeField]
	Transform textTransform;
	[SerializeField]
	RectTransform textUITransform;

	[SerializeField]
	TextMeshProUGUI targetNameText;
	[SerializeField]
	TextMeshProUGUI targetNicknameText;
	[SerializeField]
	TextMeshProUGUI mainTargetText;

	bool drawLines = false;
	bool isLocked;
	private Camera cam;

    Vector2 screenSize;
    float screenAdjustFactor;

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

	public void SetTarget(TargetObject target)
	{
		if(target == null)
		{
			SetArrowVisible(false);
			return;
		}

		targetObject = target;
		targetNameText.text = targetObject.Info.ObjectName;
		targetNicknameText.text = targetObject.Info.ObjectNickname;
		mainTargetText.gameObject.SetActive(targetObject.Info.MainTarget);
	}

	public void SetArrowVisible(bool visible)
	{
		if(targetObject == null)
			return;

		drawLines = visible;
		targetNameText.gameObject.SetActive(visible);
		targetNicknameText.gameObject.SetActive(visible);
		mainTargetText.gameObject.SetActive(visible && targetObject.Info.MainTarget);
	}


	// Draw Arrow
	void OnPostRender()
	{
		if (!drawLines || vertexTransforms == null || vertexTransforms.Length < 2)
			return;
 
		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = vertexTransforms.Length - 1;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
 
		lineMaterial.SetPass(0);

		if (lineWidth == 1)
		{
	        GL.Begin(GL.LINES);
	        for (int i = 0; i < end; ++i)
			{
                Vector2 linePoint = cam.WorldToViewportPoint(vertexTransforms[i].position);
                Vector2 nextlinePoint = cam.WorldToViewportPoint(vertexTransforms[i + 1].position);
                
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoint.x, linePoint.y, nearClip)));
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(nextlinePoint.x, nextlinePoint.y, nearClip)));
        	}
    	}
    	else
		{
	        GL.Begin(GL.QUADS);
	        for (int i = 0; i < end; ++i)
			{
                Vector2 linePoint = cam.WorldToViewportPoint(vertexTransforms[i].position);
                Vector2 nextlinePoint = cam.WorldToViewportPoint(vertexTransforms[i + 1].position);

	            Vector3 perpendicular = (new Vector3(nextlinePoint.y, linePoint.x, nearClip) -
	                                 new Vector3(linePoint.y, nextlinePoint.x, nearClip)).normalized * thisWidth;
	            Vector3 v1 = new Vector3(linePoint.x, linePoint.y, nearClip);
	            Vector3 v2 = new Vector3(nextlinePoint.x, nextlinePoint.y, nearClip);
	            GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
        	}
    	}
    	GL.End();
	}

 
	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	void Start()
	{
        screenSize = new Vector2(Screen.width, Screen.height);
        screenAdjustFactor = Mathf.Max((1920.0f / Screen.width), (1080.0f / Screen.height));
	}
    
    // Update is called once per frame
    void Update()
    {
		if(targetObject == null)
		{
			SetArrowVisible(false);
			return;
		}
			
        cameraAttachedTransform.LookAt(targetObject.transform);
        arrowTransform.eulerAngles = cameraAttachedTransform.localEulerAngles;

		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, textTransform.position);
		Vector2 position = screenPoint - screenSize * 0.5f;
		position *= screenAdjustFactor;
		textUITransform.anchoredPosition = position;
    }
}
