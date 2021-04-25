using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    private System.Text.StringBuilder str = new System.Text.StringBuilder();

	[SerializeField]
	private Text text;

	// Update is called once per frame
	void Update () {
		text.text = str.ToString();
		str.Clear();
	}

	public void AddText(string addText)
	{
		str.Append(addText);
		str.Append("\n");
	}
}
