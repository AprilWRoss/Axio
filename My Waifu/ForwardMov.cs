using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMov : MonoBehaviour {

	public float Speed;

	void Update () {
		transform.position += transform.up * Speed;
	}
}
