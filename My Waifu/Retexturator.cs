using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retexturator : MonoBehaviour {

	private Material mis;
	private float origX;
	private float origY;

	public bool AffectX;
	public bool AffectY;

	void Start () {
		mis = gameObject.GetComponent<Renderer> ().material;
		origX = mis.mainTextureScale.x;
		origY = mis.mainTextureScale.y;

		if (AffectX && AffectY)
			mis.mainTextureScale = new Vector2 (origX * transform.lossyScale.x, origY * transform.lossyScale.z);

		if (AffectX && !AffectY)
			mis.mainTextureScale = new Vector2 (origX * transform.lossyScale.x, origY);
		
		if (!AffectX && AffectY)
			mis.mainTextureScale = new Vector2 (origX, origY * transform.lossyScale.z);
	}
}
