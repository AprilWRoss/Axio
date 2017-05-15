using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartus : MonoBehaviour {
	void OnCollisionEnter (Collision obj) {
		if (obj.gameObject.tag == "Cartus")
			Physics.IgnoreCollision (gameObject.GetComponent<Collider> (), obj.gameObject.GetComponent<Collider> ());
	}
}
