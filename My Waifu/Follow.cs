using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public float CamHeight;
	private GameObject Player;
	public float OffsetMult;
	public float ScPercent;
	private float Xproc;
	private float Yproc;
	private float folSpeed;
	private float folSpeedSmoothing;
	private Vector3 refVt;
	void Start () {
		Player = GameObject.Find ("Player");
	}

	void Update () {
		float newWidth = (float) Screen.width / 2;
		float newHeight = (float) Screen.height / 2;
		if ((Input.mousePosition.x >= Screen.width - (float) (Screen.width * ScPercent) / 100 || Input.mousePosition.x <= (float) (Screen.width * ScPercent) / 100)
			|| (Input.mousePosition.y >= Screen.height - (float) (Screen.height * ScPercent) / 100 || Input.mousePosition.y <= (float) (Screen.height * ScPercent) / 100)) {
			Xproc = (float)(Input.mousePosition.x - newWidth) / newWidth;
			Yproc = (float)(Input.mousePosition.y - newHeight) / newHeight;
			folSpeed = 0.3f;
		} else {
			Xproc = 0;
			Yproc = 0;
			folSpeed = Mathf.SmoothDamp (folSpeed, 0.05f, ref folSpeedSmoothing, 0.3f);
		}

		if (Xproc > 1)
			Xproc = 1;
		if (Xproc < -1)
			Xproc = -1;

		if (Yproc > 1)
			Yproc = 1;
		if (Yproc < -1)
			Yproc = -1;
		
		transform.position = Vector3.SmoothDamp(transform.position,Player.transform.position + new Vector3 (Xproc * OffsetMult, CamHeight - Player.transform.position.y, Yproc * OffsetMult), ref refVt, folSpeed);
	}
}
