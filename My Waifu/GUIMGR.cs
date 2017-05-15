using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIMGR : MonoBehaviour {

	public float origX;
	public float origY;

	public Texture2D[] Crosshairs;
	private Text ClipSize;

	void Start() {
		Application.targetFrameRate = 60;
		ClipSize = GameObject.Find ("ClipSize").GetComponent<Text> ();
		transform.localScale = new Vector2 ((float)Screen.width / origX, (float)Screen.height / origY);
		SetCrosshair (0);
	}

	public void SetCrosshair (int index) {
		Cursor.SetCursor (Crosshairs [index], new Vector2 ((float) Crosshairs [index].width / 2, (float) Crosshairs [index].height / 2), CursorMode.Auto);
	}

	public void UpdateClipSize (int amount) {
		ClipSize.text = amount + "";
	}
}
