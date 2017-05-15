using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public int WeaponType;
	public float FireRate;
	public int MaxClipSize;
	public GameObject Bullet;
	public GameObject Cartus;
	public float ReloadTime;
	public float RandomSpread;
	public int Damage;

	private float LastShot;
	public int Clips;
	private GameObject Barrel;
	private Animator Anim;
	private Animator PlayerAnim;

	//[HideInInspector]
	public bool Reloading;
	private GUIMGR GuiManager;

	//Sounds
	private AudioSource ReloadSound;

	public GameObject WPNDropped;

	void Start() {
		Barrel = transform.FindChild ("Barrel").gameObject;
		Clips = MaxClipSize;
		Anim = gameObject.GetComponent<Animator> ();
		GuiManager = GameObject.Find ("MGR").GetComponent<GUIMGR> ();
		ReloadSound = transform.FindChild ("Reload").GetComponent<AudioSource> ();
		if (transform.parent != null)
			PlayerAnim = transform.parent.parent.FindChild ("Model").GetComponent<Animator> ();
	}

	public void Shoot() {
		if (Time.time >= LastShot + FireRate) {
			if (Clips > 0) {
				ReloadSound.Stop ();
				CancelInvoke ("FillClips");
				CancelInvoke ("TurnOffReload");
				TurnOffReload ();
				Reloading = false;
				Clips--;
				GameObject Bull = Instantiate (Bullet, new Vector3 (Barrel.transform.position.x, 0.61f, Barrel.transform.position.z), Quaternion.Euler (90, 0, -(transform.rotation.eulerAngles.y + Random.Range (-RandomSpread, RandomSpread))), null);
				Bull.GetComponent<BulletInfo> ().Damage = Damage;
				LastShot = Time.time;
				PlayerAnim = transform.parent.parent.FindChild ("Model").GetComponent<Animator> ();
				PlayerAnim.SetBool ("Shoot", true);
				Anim.SetBool ("Shoot", true);
				Invoke ("TurnOffShoot", (float)FireRate / 2);
				if (Clips == 0 && !Reloading)
					Invoke ("Reload", FireRate);
				ThrowCartus ();

				Camera.main.GetComponent<Follow> ().CamShake (0.1f);
			}
		} else
			Invoke ("Shoot", FireRate - (Time.time - LastShot));
	}

	void TurnOffShoot () {
		PlayerAnim.SetBool ("Shoot", false);
		Anim.SetBool ("Shoot", false);
	}

	void TurnOffReload () {
		Anim.SetBool ("Reload", false);
	}

	public void Reload() {
		if (Clips < MaxClipSize) {
			ReloadSound.Play ();
			PlayerAnim.SetBool ("Shoot", true);
			Invoke ("UnfreezePlayerIdle", 0.75f);
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

	void ThrowCartus () {
		GameObject Thrown = Instantiate (Cartus, new Vector3(Barrel.transform.position.x, 0.05f, Barrel.transform.position.z) - Barrel.transform.up * 0.8f, Quaternion.Euler(90, 0 , Random.Range(0, 360)), null);
		Thrown.GetComponent<Rigidbody> ().velocity = transform.right * 5 + transform.up * Random.Range (-3, 3);
	}

	public void UnfreezePlayerIdle() {
		PlayerAnim.SetBool ("Shoot", false);
	}
}
