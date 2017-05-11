using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	public float TimeUntilDeath;

	void Start () {
		Invoke ("Destructo", TimeUntilDeath);
	}

	void Destructo () {
		Destroy (gameObject);
	}
}
