using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDown : MonoBehaviour {

	public bool DontKill;
	public float DeathTime;
	private Rigidbody RB;
	private Vector3 refSlowdown;
	private float A;
	private float refTransparency;

	void Start () {
		RB = gameObject.GetComponent<Rigidbody> ();
		A = 1;
		if (!DontKill) InvokeRepeating ("DeathN", DeathTime, 0.1f);
		InvokeRepeating ("SlowN", 0, 0.1f);
	}

	void SlowN () {
		RB.velocity = Vector3.SmoothDamp (RB.velocity, Vector3.zero, ref refSlowdown, 0.1f);
		if (RB.velocity.x <= 0.05f && RB.velocity.x >= -0.05f && RB.velocity.z <= 0.05f && RB.velocity.z >= -0.05f) {
			RB.constraints = RigidbodyConstraints.FreezeAll;
			CancelInvoke ("SlowN");
		}
	}

	void DeathN () {
		A = Mathf.SmoothDamp (A, 0, ref refTransparency, 1);
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, A);
		if (A <= 0.05f)
			Destroy (gameObject);
	}
}
