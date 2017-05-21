using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIMGR : MonoBehaviour {

	public float origX;
	public float origY;

	public Texture2D[] Crosshairs;
	private Text ClipSize;
	private Text ClipSizeLeft;

	void Start() {
		Application.targetFrameRate = 60;
		if (SceneManager.GetActiveScene ().name != "Menu") {
			ClipSize = GameObject.Find ("ClipSize").GetComponent<Text> ();
			ClipSizeLeft = GameObject.Find ("ClipSizeLeft").GetComponent<Text> ();
		}
		transform.localScale = new Vector2 ((float)Screen.width / origX, (float)Screen.height / origY);
		SetCrosshair (0);
	}

	void Update () {
		if (SceneManager.GetActiveScene ().name == "Main") {
			if (Input.GetKeyDown (KeyCode.Escape))
				SceneManager.LoadScene ("Menu");
		}
	}

	public void SetCrosshair (int index) {
		Cursor.SetCursor (Crosshairs [index], new Vector2 ((float) Crosshairs [index].width / 2, (float) Crosshairs [index].height / 2), CursorMode.Auto);
	}

	public void UpdateClipSize (int amount) {
		ClipSize.text = amount + "";
	}

	public void UpdateLeftClipSize (int amount) {
		ClipSizeLeft.text = amount + "";
	}

	public void UpdateLeftClipSize (string newText) {
		ClipSizeLeft.text = newText;
	}

	public void StartGame () {
		SceneManager.LoadScene ("Main");
	}

	public void ExitGameN () {
		Application.Quit ();
	}
}
