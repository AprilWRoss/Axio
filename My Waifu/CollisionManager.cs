using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

	private HealthManager HM;
	private GameObject SoundBoard;
	public bool AliveObject;
	public bool ActiveHitMarker;

	void Start() {
		if (AliveObject)
			HM = gameObject.GetComponentInChildren<HealthManager> ();
		if (ActiveHitMarker)
			SoundBoard = GameObject.Find ("Sounds");
	}

	void OnTriggerEnter(Collider obj) {
		if (obj.gameObject.tag == "Projectile") {
			if (AliveObject)
				HM.SubtractHealth(obj.gameObject.GetComponent<BulletInfo> ().Damage);
			obj.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			obj.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
			obj.gameObject.transform.GetChild (0).gameObject.SetActive (false);
			if (ActiveHitMarker)
				SoundBoard.transform.FindChild ("HitMarker").GetComponent<AudioSource> ().Play ();
		}
	}
}
