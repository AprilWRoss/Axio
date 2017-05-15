using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
	
	//MOVEMENT VARS//
	public float Speed;
	private float Xmov;
	private float Ymov;

	//EXTRAS//
	private Vector3 refMov;
	private Animator Anim;
	public float StartWalkAngle;
	private GameObject CurrentWeapon;
	private GUIMGR GuiManager;
	private Rigidbody Body;
	private Vector3 Mov;

	void Start () {
		Body = gameObject.GetComponent<Rigidbody> ();
		Anim = transform.FindChild ("Model").GetComponent<Animator> ();
		CurrentWeapon = transform.FindChild ("CurrentWeapon").gameObject;
		GuiManager = GameObject.Find ("MGR").GetComponent<GUIMGR> ();
		GuiManager.UpdateClipSize (16);
	}

	void Update () {
		//MOVEMENT
		Xmov = Input.GetAxisRaw ("Horizontal");
		Ymov = Input.GetAxisRaw ("Vertical");

		if (Xmov > 0) {
			if (Ymov != 0)
				Xmov = 0.7f;
			
			if (Ymov > 0)
				Ymov = 0.7f;

			if (Ymov < 0)
				Ymov = -0.7f;
		}

		if (Xmov < 0) {
			if (Ymov != 0)
				Xmov = -0.7f;
			
			if (Ymov > 0)
				Ymov = 0.7f;

			if (Ymov < 0)
				Ymov = -0.7f;
		}
		//transform.position = Vector3.SmoothDamp (transform.position, transform.position + new Vector3 (Xmov * Speed, 0, Ymov * Speed), ref refMov, 0.25f);
		Mov = Vector3.SmoothDamp (Mov, new Vector3 (Xmov * Speed, 0, Ymov * Speed), ref refMov, 0.1f);
		Body.velocity = Mov;
		//Debug.Log (new Vector3 (Xmov * Speed, 0, Ymov * Speed));
		/*if (Body.velocity.x > MaxSpeed)
			Body.velocity = new Vector3 (MaxSpeed, 0, Body.velocity.z);
		if (Body.velocity.x < -MaxSpeed)
			Body.velocity = new Vector3 (-MaxSpeed, 0, Body.velocity.z);

		if (Body.velocity.z > MaxSpeed)
			Body.velocity = new Vector3 (Body.velocity.x, 0, MaxSpeed);
		if (Body.velocity.z < -MaxSpeed)
			Body.velocity = new Vector3 (Body.velocity.x, 0, -MaxSpeed);*/

		//ANIMATION
		Animator[] WeaponAnims = CurrentWeapon.GetComponentsInChildren<Animator> ();
		if (Xmov != 0 || Ymov != 0) {
			float Rot = transform.rotation.eulerAngles.y;
			float MovementAngle = 0;

			if (Ymov > 0) {
				if (Xmov == 0)
					MovementAngle = 0;
				if (Xmov > 0)
					MovementAngle = 45;
				if (Xmov < 0)
					MovementAngle = 315;
			}

			if (Ymov == 0) {
				if (Xmov > 0)
					MovementAngle = 90;
				if (Xmov < 0)
					MovementAngle = 270;
			}

			if (Ymov < 0) {
				if (Xmov == 0)
					MovementAngle = 180;
				if (Xmov > 0)
					MovementAngle = 135;
				if (Xmov < 0)
					MovementAngle = 225;
			}

			float MinLimit = MovementAngle - StartWalkAngle;
			float MaxLimit = MovementAngle + StartWalkAngle;
			int Iteration = 0;

			Reiterate:
			if (Iteration <= 1) {
				if (MaxLimit > 360)
					MaxLimit -= 360;

				if (MinLimit < 0)
					MinLimit += 360;

				if (MinLimit > MaxLimit) {
					if (Rot >= MinLimit || Rot <= MaxLimit) {
						Anim.SetBool ("isWalking", true);
						for (int i = 0; i < WeaponAnims.Length; i++)
							WeaponAnims [i].SetBool ("Walks", true);
					} else {
						Anim.SetBool ("isWalking", false);
						for (int i = 0; i < WeaponAnims.Length; i++)
							WeaponAnims [i].SetBool ("Walks", false);
					}
				} else {
					if (Rot >= MinLimit && Rot <= MaxLimit) {
						Anim.SetBool ("isWalking", true);
						for (int i = 0; i < WeaponAnims.Length; i++)
							WeaponAnims [i].SetBool ("Walks", true);
					} else {
						Anim.SetBool ("isWalking", false);
						for (int i = 0; i < WeaponAnims.Length; i++)
							WeaponAnims [i].SetBool ("Walks", false);
					}
				}
				Iteration++;
				MinLimit = MinLimit - 180;
				MaxLimit = MaxLimit - 180;
				if (Anim.GetBool ("isWalking") == false)
					goto Reiterate;
			}
		} else {
			Anim.SetBool ("isWalking", false);
			for (int i = 0; i < WeaponAnims.Length; i++)
				WeaponAnims [i].SetBool ("Walks", false);
		}

		//SHOOT
		if (Input.GetMouseButtonDown (0)) {
			int cCount = CurrentWeapon.transform.childCount;
			for (int i = 0; i < cCount; i++) {
				Weapon WPN = CurrentWeapon.transform.GetChild (i).GetComponent<Weapon>();
				if (WPN.WeaponType == 2) {
					WPN.Shoot ();
					GuiManager.UpdateClipSize (WPN.Clips);
				}
			}
		}

		//RELOAD
		if (Input.GetKeyDown (KeyCode.R)) {
			int cCount = CurrentWeapon.transform.childCount;
			for (int i = 0; i < cCount; i++) {
				Weapon WPN = CurrentWeapon.transform.GetChild (i).GetComponent<Weapon>();
				if (WPN.WeaponType == 2) {
					if (!WPN.Reloading)
						WPN.Reload ();
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Q))
			DropWeapon (1);
	}

	void FixedUpdate() {
		//ROTATION
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hitD = Physics.RaycastAll (ray);
		transform.LookAt (hitD[0].point);
		transform.rotation = Quaternion.Euler (new Vector3 (0, transform.rotation.eulerAngles.y, 0));
	}

	public void DropWeapon(int index) {
		index--;
		if (CurrentWeapon.transform.childCount > index) {
			GameObject WPN = CurrentWeapon.transform.GetChild (index).gameObject;
			Instantiate (WPN.GetComponent <Weapon> ().WPNDropped, transform.position + transform.forward, Quaternion.Euler(90, 0, 0));
			Destroy (WPN);
		}
	}
}