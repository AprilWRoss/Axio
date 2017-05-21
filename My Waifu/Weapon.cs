using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public bool DualWielded;
	public int Automatic;
	public string Name;
	public int WeaponType;
	public float FireRate;
	public int MaxClipSize;
	private GameObject Bullet;
	private GameObject Cartus;
	public float ReloadTime;
	public float RandomSpread;
	public int Damage;

	[HideInInspector]
	public float LastShot;
	[HideInInspector]
	public int Clips;
	private GameObject Barrel;
	private Animator Anim;
	private Animator PlayerAnim;

	[HideInInspector]
	public bool Reloading;
	private GUIMGR GuiManager;

	//Sounds
	private AudioSource ReloadSound;
	private AudioSource OutOfAmmo;

	private GameObject WPNDropped;
	public GameObject Magazie;

	void Start() {
		Barrel = transform.FindChild ("Barrel").gameObject;
		Cartus = (GameObject)Resources.Load ("Prefabs/Weapons/" + Name + "/Cartus");
		Bullet = (GameObject)Resources.Load ("Prefabs/Weapons/" + Name + "/Bullet");
		WPNDropped = (GameObject)Resources.Load ("Prefabs/Weapons/" + Name + "/Dropped");
		Anim = gameObject.GetComponent<Animator> ();
		GuiManager = GameObject.Find ("MGR").GetComponent<GUIMGR> ();
		ReloadSound = transform.FindChild ("Reload").GetComponent<AudioSource> ();
		OutOfAmmo = transform.FindChild ("OutOfAmmo").GetComponent<AudioSource> ();
		if (transform.parent != null)
			PlayerAnim = transform.parent.parent.FindChild ("Model").GetComponent<Animator> ();
	}

	public void Shoot() {
		if (Time.time >= LastShot + FireRate) {
			if (Clips > 0) {
				ReloadSound.Stop ();
				CancelInvoke ("FillClips");
				CancelInvoke ("TurnOffReload");
				CancelInvoke ("ThrowEmptyWeapon");
				TurnOffReload ();
				Reloading = false;
				Clips--;
				GameObject Bull = Instantiate (Bullet, new Vector3 (Barrel.transform.position.x, 0.8f, Barrel.transform.position.z), Quaternion.Euler (90, 0, -(transform.rotation.eulerAngles.y + Random.Range (-RandomSpread, RandomSpread))), null);
				Bull.GetComponent<BulletInfo> ().Damage = Damage;
				LastShot = Time.time;
				PlayerAnim = transform.parent.parent.FindChild ("Model").GetComponent<Animator> ();
				PlayerAnim.SetBool ("Shoot", true);

				for (int i = 0; i < transform.parent.childCount; i++)
					if (!transform.parent.GetChild (i).GetComponent<Animator> ().GetBool("Reload"))
						transform.parent.GetChild (i).GetComponent<Animator> ().SetBool ("Walks", true);
				
				Anim.SetBool ("Walks", false);
				Anim.SetBool ("Shoot", true);
				Invoke ("TurnOffShoot", (float)FireRate / 2);
				ThrowCartus ();

				Camera.main.GetComponent<Follow> ().CamShake (0.1f);
			}

			if (Clips == 0 && !Reloading) {
				if (transform.parent.childCount == 1) {
					Invoke ("Reload", FireRate);
				} else {
					CancelInvoke ("ThrowEmptyWeapon");
					OutOfAmmo.Play ();
					Instantiate ((GameObject)Resources.Load ("Prefabs/GUI/EmptyCasing"), Barrel.transform.position + new Vector3 (-2.42f, 2.5f, 0), Quaternion.identity, null);
				}
				LastShot = Time.time;
			}
		} else if (Automatic == 1) {
			CancelInvoke ("Shoot");
			Invoke ("Shoot", FireRate - (Time.time - LastShot));
		}
	}

	void TurnOffShoot () {
		for (int i = 0; i < transform.parent.childCount; i++)
			transform.parent.GetChild (i).GetComponent<Animator> ().SetBool ("Walks", false);
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
			if (DualWielded)
				Invoke ("ThrowEmptyWeapon", 0.1875f);
			Anim.SetBool ("Reload", true);
			Reloading = true;
			Invoke ("FillClips", ReloadTime);
			Invoke ("TurnOffReload", (float) FireRate / (1.4f));
		}
	}

	void FillClips () {
		Clips = MaxClipSize;
		Reloading = false; 
		if (WeaponType == 2)
			GuiManager.UpdateClipSize (Clips);
		else 
			GuiManager.UpdateLeftClipSize (Clips);
	}

	void ThrowCartus () {
		GameObject Thrown = Instantiate (Cartus, new Vector3(Barrel.transform.position.x, 0.05f, Barrel.transform.position.z) - Barrel.transform.up * 0.8f, Quaternion.Euler(90, 0 , Random.Range(0, 360)), null);
		Thrown.GetComponent<Rigidbody> ().velocity = transform.right * 5 + transform.up * Random.Range (-3, 3);
	}

	void ThrowEmptyWeapon () {
		if ((WeaponType == 2 && !Input.GetMouseButton (0)) || (WeaponType == 1 && !Input.GetMouseButton (1)) || Clips == 0) {
			GameObject Thrown = Instantiate (WPNDropped, new Vector3 (Barrel.transform.position.x, 0.05f, Barrel.transform.position.z) - Barrel.transform.up * 0.8f, Quaternion.Euler (90, 0, Random.Range (0, 360)), null);
			if (WeaponType == 1)
				Thrown.GetComponent<Rigidbody> ().velocity = -transform.right * 3 + transform.up * Random.Range (-3, 3);
			if (WeaponType == 2)
				Thrown.GetComponent<Rigidbody> ().velocity = transform.right * 3 + transform.up * Random.Range (-3, 3);
		
			Thrown.transform.FindChild ("ShinyDropped").gameObject.SetActive (false);
			Thrown.transform.FindChild ("CollectZone").gameObject.SetActive (false);
			Thrown.GetComponent<Light> ().enabled = false;
		}
	}

	public void ThrowCollectableWeapon () {
		GameObject Thrown = Instantiate (WPNDropped, new Vector3(Barrel.transform.position.x, 0.05f, Barrel.transform.position.z) - Barrel.transform.up * 0.8f, Quaternion.Euler(90, 0 , Random.Range(0, 360)), null);
		Thrown.GetComponent<Rigidbody> ().velocity = transform.up * Random.Range (4, 7);
		Thrown.GetComponentInChildren<WeaponInfo> ().Clips = Clips;
		Thrown.GetComponentInChildren<WeaponInfo> ().Name = Name;
		Thrown.GetComponentInChildren<WeaponInfo> ().WeaponType = WeaponType;
		Destroy (gameObject);
	}

	void ThrowMagazie () {
		GameObject Thrown = Instantiate (Magazie, new Vector3(Barrel.transform.position.x, 0.05f, Barrel.transform.position.z) - Barrel.transform.up * 0.8f, Quaternion.Euler(90, 0 , Random.Range(0, 360)), null);
		if (WeaponType == 1)
			Thrown.GetComponent<Rigidbody> ().velocity = - transform.right * 3 + transform.up * Random.Range (-3, 3);
		if (WeaponType == 2)
			Thrown.GetComponent<Rigidbody> ().velocity = transform.right * 3 + transform.up * Random.Range (-3, 3);
	}

	public void UnfreezePlayerIdle() {
		for (int i = 0; i < transform.parent.childCount; i++)
			transform.parent.GetChild (i).GetComponent<Animator> ().SetBool ("Walks", false);
		PlayerAnim.SetBool ("Shoot", false);
	}
}
