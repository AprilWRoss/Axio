using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retexturator : MonoBehaviour {

	private Material mis;
	private float origX;
	private float origY;

	void Start () {
		mis = gameObject.GetComponent<Renderer> ().material;
		origX = mis.mainTextureScale.x;
		origY = mis.mainTextureScale.y;
		mis.mainTextureScale = new Vector2 (origX * transform.localScale.x, origY * transform.localScale.z);
	}
}
