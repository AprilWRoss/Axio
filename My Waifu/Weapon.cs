using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public int WeaponType;
	public float FireRate;
	public int MaxClipSize;
	public GameObject Bullet;
	public float ReloadTime;
	public float RandomSpread;

	private float LastShot;
	public int Clips;
	private GameObject Barrel;
	private Animator Anim;
	//[HideInInspector]
	public bool Reloading;
	private GUIMGR GuiManager;

	//Sounds
	private AudioSource ReloadSound;

	void Start() {
		Barrel = transform.FindChild ("Barrel").gameObject;
		Clips = MaxClipSize;
		Anim = gameObject.GetComponent<Animator> ();
		GuiManager = GameObject.Find ("MGR").GetComponent<GUIMGR> ();
		ReloadSound = transform.FindChild ("Reload").GetComponent<AudioSource> ();
	}

	public void Shoot() {
		if (Time.time >= LastShot + FireRate) {
			if (Clips == 0 && !Reloading)
				Reload ();

			if (Clips > 0) {
				CancelInvoke ("FillClips");
				CancelInvoke ("TurnOffReload");
				TurnOffReload ();
				Reloading = false;
				Clips--;
				Instantiate (Bullet, new Vector3(Barrel.transform.position.x, 0.05f, Barrel.transform.position.z), Quaternion.Euler(90, 0 , -(transform.rotation.eulerAngles.y + Random.Range (-RandomSpread, RandomSpread))), null);
				LastShot = Time.time;
				Anim.SetBool ("Shoot", true);
				Invoke ("TurnOffShoot", (float)FireRate / 2);
			}
		}
	}

	void TurnOffShoot () {
		Anim.SetBool ("Shoot", false);
	}

	void TurnOffReload () {
		Anim.SetBool ("Reload", false);
	}

	public void Reload() {
		if (Clips < MaxClipSize) {
			ReloadSound.Play ();
			Anim.SetBool ("Reload", true);
			Reloading = true;
			Invoke ("FillClips", ReloadTime);
			Invoke ("TurnOffReload", (float) FireRate / (1.4f));
		}
	}

	void FillClips () {
		Clips = MaxClipSize;
		Reloading = false;
		GuiManager.UpdateClipSize (Clips);
	}
}
