using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

	private GameObject Player;
	public float Speed;

	void Start () {
		Player = GameObject.Find ("Player");
	}

	void Update () {
		//transform.position += transform.forward * Speed;
		transform.LookAt (Player.transform.position);
		transform.rotation = Quaternion.Euler (new Vector3 (0, transform.rotation.eulerAngles.y, 0));

		//EXTRAS
		gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed;
	}
}
