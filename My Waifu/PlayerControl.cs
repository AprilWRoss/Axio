using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private GameObject WeaponSlots;
	private bool DualWield;
	private int CurrentWeaponIndex;
	private Animator WakeyTarget;
	private GameObject Footsteps;

	void Start () {
		Body = gameObject.GetComponent<Rigidbody> ();
		Anim = transform.FindChild ("Model").GetComponent<Animator> ();
		CurrentWeapon = transform.FindChild ("CurrentWeapon").gameObject;
		Footsteps = transform.FindChild ("Footsteps").gameObject;
		GuiManager = GameObject.Find ("MGR").GetComponent<GUIMGR> ();
		WeaponSlots = GameObject.Find ("WeaponSlots");
		GuiManager.UpdateClipSize (12);
		InvokeRepeating ("FootstepProcessor", 0, 0.35f);
	}

	void FootstepProcessor () {
		if (Body.velocity.x >= 1.5f || Body.velocity.x <= -1.5f || Body.velocity.z >= 1.5f || Body.velocity.z <= -1.5f) {
			int Trial =	Random.Range (0, 4);
			Footsteps.transform.FindChild("Metal").GetChild (Trial).GetComponent<AudioSource> ().Play ();
		}
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
		Mov = Vector3.SmoothDamp (Mov, new Vector3 (Xmov * Speed, 0, Ymov * Speed), ref refMov, 0.1f);
		Body.velocity = Mov;

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
		int cCount = CurrentWeapon.transform.childCount;
		if (cCount == 2) {
			Weapon WPNleft;
			Weapon WPNright;
			if (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().WeaponType == 1) {
				WPNleft = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
				WPNright = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ();
			} else {
				WPNleft = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ();
				WPNright = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
			}

			//Same-Weapon Dual-Wielding
			if (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().Name == CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ().Name) {
				if (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().Automatic <= 1) {
					if (Input.GetMouseButtonDown (0)) {
						if (WPNleft.LastShot >= WPNright.LastShot) {
							WPNright.Shoot ();
							GuiManager.UpdateClipSize (WPNright.Clips);
						} else {
							WPNleft.Shoot ();
							GuiManager.UpdateLeftClipSize (WPNleft.Clips);
						}
					}
				}

				if (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().Automatic == 2) {
					if (Input.GetMouseButton (0)) {
						if (WPNleft.LastShot >= WPNright.LastShot) {
							WPNright.Shoot ();
							GuiManager.UpdateClipSize (WPNright.Clips);
						} else {
							WPNleft.Shoot ();
							GuiManager.UpdateLeftClipSize (WPNleft.Clips);
						}
					}
				}
			} else {
				if (WPNright.Automatic <= 1) {
					if (Input.GetMouseButtonDown (0)) {
						WPNright.Shoot ();
						GuiManager.UpdateClipSize (WPNright.Clips);
					}
				} else {
					if (Input.GetMouseButton (0)) {
						WPNright.Shoot ();
						GuiManager.UpdateClipSize (WPNright.Clips);
					}
				}

				if (WPNleft.Automatic <= 1) {
					if (Input.GetMouseButtonDown (1)) {
						WPNleft.Shoot ();
						GuiManager.UpdateLeftClipSize (WPNleft.Clips);
					}
				} else {
					if (Input.GetMouseButton (1)) {
						WPNleft.Shoot ();
						GuiManager.UpdateLeftClipSize (WPNleft.Clips);
					}
				}
			}
			goto EndShoot;
		}

		for (int i = 0; i < cCount; i++) {
			Weapon WPN = CurrentWeapon.transform.GetChild (i).GetComponent<Weapon>();
			if (WPN.Automatic <= 1) {
				if (Input.GetMouseButtonDown (0)) {
					WPN.Shoot ();
					if (WPN.WeaponType == 1)
						GuiManager.UpdateLeftClipSize (WPN.Clips);
					else
						GuiManager.UpdateClipSize (WPN.Clips);
				}
			}

			if (WPN.Automatic == 2) {
				if (Input.GetMouseButton (0)) {
					WPN.Shoot ();
					if (WPN.WeaponType == 1)
						GuiManager.UpdateLeftClipSize (WPN.Clips);
					else
						GuiManager.UpdateClipSize (WPN.Clips);
				}
			}
		}
		EndShoot:

		//RELOAD
		if (Input.GetKeyDown (KeyCode.R)) {
			for (int i = 0; i < cCount; i++) {
				Weapon WPN = CurrentWeapon.transform.GetChild (i).GetComponent<Weapon>();
				if (!WPN.Reloading)
					WPN.Reload ();
			}
		}

		//WEAPON SWITCHING
		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Keypad1)) {
			Weapon WPN = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
			if (WeaponSlots.transform.childCount >= 2) {
				if (WeaponSlots.transform.GetChild (0).GetComponent<WeaponInfo> ().Name != WPN.Name) {
					if (DualWield)
						SwitchWeapons ();
					else {
						if (WeaponSlots.transform.GetChild (0).GetComponent<WeaponInfo> ().Name != WPN.Name) {
							WeaponSlots.transform.GetChild (1).GetComponent<WeaponInfo> ().Clips = WPN.Clips;
							WeaponSlots.transform.GetChild (1).GetComponent<WeaponInfo> ().LastShot = WPN.LastShot;

							Destroy (CurrentWeapon.transform.GetChild (0).gameObject);
							WeaponInfo WI = WeaponSlots.transform.GetChild (0).GetComponent<WeaponInfo> ();
							string Path = "Prefabs/Weapons/" + WI.Name + "/" + WI.Name + "Right";
							GameObject NewWeapon = (GameObject)Resources.Load (Path);
							GameObject NWPN = Instantiate (NewWeapon, Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
							NWPN.transform.localPosition = Vector3.zero;
							NWPN.GetComponent<Weapon> ().Clips = WI.Clips;
							NWPN.GetComponent<Weapon> ().LastShot = WI.LastShot;
							CurrentWeaponIndex = 0;
							GuiManager.UpdateClipSize (WI.Clips);
						}
					}
					WeaponSlots.transform.GetChild (0).GetComponentInChildren<Animator> ().SetBool ("Wakey", true);
					WakeyTarget = WeaponSlots.transform.GetChild (0).GetComponentInChildren<Animator> ();
					Invoke ("TurnOffWakeyInGUIWeaponSelection", 0.05f);
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Keypad2)) {
			Weapon WPN = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
			if (WeaponSlots.transform.childCount >= 2) {
				if (WeaponSlots.transform.GetChild (1).GetComponent<WeaponInfo> ().Name != WPN.Name) {
					if (DualWield)
						SwitchWeapons ();
					else {
						if (WeaponSlots.transform.GetChild (1).GetComponent<WeaponInfo> ().Name != WPN.Name) {
							WeaponSlots.transform.GetChild (0).GetComponent<WeaponInfo> ().Clips = WPN.Clips;
							WeaponSlots.transform.GetChild (0).GetComponent<WeaponInfo> ().LastShot = WPN.LastShot;

							Destroy (CurrentWeapon.transform.GetChild (0).gameObject);
							WeaponInfo WI = WeaponSlots.transform.GetChild (1).GetComponent<WeaponInfo> ();
							string Path = "Prefabs/Weapons/" + WI.Name + "/" + WI.Name + "Right";
							GameObject NewWeapon = (GameObject)Resources.Load (Path);
							GameObject NWPN = Instantiate (NewWeapon, Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
							NWPN.transform.localPosition = Vector3.zero;
							NWPN.GetComponent<Weapon> ().Clips = WI.Clips;
							NWPN.GetComponent<Weapon> ().LastShot = WI.LastShot;
							CurrentWeaponIndex = 1;
							GuiManager.UpdateClipSize (WI.Clips);
						}
					}
					WeaponSlots.transform.GetChild (1).GetComponentInChildren<Animator> ().SetBool ("Wakey", true);
					WakeyTarget = WeaponSlots.transform.GetChild (1).GetComponentInChildren<Animator> ();
					Invoke ("TurnOffWakeyInGUIWeaponSelection", 0.05f);
				}
			}
		}

		//DUAL-WIELD MODE
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (WeaponSlots.transform.childCount >= 2) {
				DualWield = !DualWield;
				if (DualWield) {
					Weapon WPN = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
					if (WPN.WeaponType != 0) {
						WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().Clips = WPN.Clips;
						WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().LastShot = WPN.LastShot;
						Destroy (CurrentWeapon.transform.GetChild (0).gameObject);

						WeaponInfo WI = WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ();
						WeaponInfo LeftWI = WeaponSlots.transform.GetChild (1 - CurrentWeaponIndex).GetComponent<WeaponInfo> ();
						string RightPath = "Prefabs/Weapons/" + WI.Name + "/" + WI.Name + "RightDual";
						string LeftPath = "Prefabs/Weapons/" + LeftWI.Name + "/" + LeftWI.Name + "Left";
						GameObject NewWPNRight = Instantiate ((GameObject)Resources.Load (RightPath), Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
						GameObject NewWPNLeft = Instantiate ((GameObject)Resources.Load (LeftPath), Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
						NewWPNRight.transform.localPosition = Vector3.zero;
						NewWPNLeft.transform.localPosition = Vector3.zero;

						NewWPNRight.GetComponent<Weapon> ().Clips = WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().Clips;
						NewWPNRight.GetComponent<Weapon> ().LastShot = WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().LastShot;

						NewWPNLeft.GetComponent<Weapon> ().Clips = WeaponSlots.transform.GetChild (1 - CurrentWeaponIndex).GetComponent<WeaponInfo> ().Clips;
						NewWPNLeft.GetComponent<Weapon> ().LastShot = WeaponSlots.transform.GetChild (1 - CurrentWeaponIndex).GetComponent<WeaponInfo> ().LastShot;

						GuiManager.UpdateLeftClipSize (LeftWI.Clips);
					}
				} else {
					if (WeaponSlots.transform.childCount >= 2) {
						WeaponSlots.transform.GetChild (1 - CurrentWeaponIndex).GetComponent<WeaponInfo> ().Clips = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ().Clips;
						WeaponSlots.transform.GetChild (1 - CurrentWeaponIndex).GetComponent<WeaponInfo> ().LastShot = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ().LastShot;
						WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().Clips = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().Clips;
						WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ().LastShot = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().LastShot;

						Destroy (CurrentWeapon.transform.GetChild (1).gameObject);
						Destroy (CurrentWeapon.transform.GetChild (0).gameObject);
						GuiManager.UpdateClipSize (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().Clips);
						GuiManager.UpdateLeftClipSize (" ");

						WeaponInfo RightWPN = WeaponSlots.transform.GetChild (CurrentWeaponIndex).GetComponent<WeaponInfo> ();
						string Path = "Prefabs/Weapons/" + RightWPN.Name + "/" + RightWPN.Name + "Right";
						GameObject NWPN = Instantiate ((GameObject) Resources.Load (Path), Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
						NWPN.transform.localPosition = Vector3.zero;
						NWPN.GetComponent<Weapon> ().Clips = RightWPN.Clips;
						NWPN.GetComponent<Weapon> ().LastShot = RightWPN.LastShot;
					}
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Q))
			DropWeapon (0);
	}

	void FixedUpdate() {
		//ROTATION
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hitD = Physics.RaycastAll (ray);
		transform.LookAt (hitD[0].point);
		transform.rotation = Quaternion.Euler (new Vector3 (0, transform.rotation.eulerAngles.y, 0));
	}

	void SwitchWeapons () {
		Weapon WPNleft;
		Weapon WPNright;
		if (CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ().WeaponType == 1) {
			WPNleft = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
			WPNright = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ();
		} else {
			WPNleft = CurrentWeapon.transform.GetChild (1).GetComponent<Weapon> ();
			WPNright = CurrentWeapon.transform.GetChild (0).GetComponent<Weapon> ();
		}

		Destroy (WPNright.gameObject);
		Destroy (WPNleft.gameObject);

		string LTRPath = "Prefabs/Weapons/" + WPNleft.Name + "/" + WPNleft.Name + "RightDual";
		string RTLPath = "Prefabs/Weapons/" + WPNright.Name + "/" + WPNright.Name + "Left";

		GameObject NRightWPN = Instantiate ((GameObject) Resources.Load(LTRPath), Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
		NRightWPN.transform.localPosition = Vector3.zero;
		NRightWPN.GetComponent<Weapon> ().Clips = WPNleft.Clips;
		NRightWPN.GetComponent<Weapon> ().LastShot = WPNleft.LastShot;

		GameObject NLeftWPN = Instantiate ((GameObject) Resources.Load(RTLPath), Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
		NLeftWPN.transform.localPosition = Vector3.zero;
		NLeftWPN.GetComponent<Weapon> ().Clips = WPNright.Clips;
		NLeftWPN.GetComponent<Weapon> ().LastShot = WPNright.LastShot;

		GuiManager.UpdateClipSize (WPNleft.Clips);
		GuiManager.UpdateLeftClipSize (WPNright.Clips);
		CurrentWeaponIndex = 1 - CurrentWeaponIndex;
	}

	public void DropWeapon(int index) {
		if (CurrentWeapon.transform.childCount > index) {
			for (int i = 0; i < WeaponSlots.transform.childCount; i++)
				if (WeaponSlots.transform.GetChild (i).GetComponent<WeaponInfo> ().Name == CurrentWeapon.transform.GetChild (index).GetComponent<Weapon> ().Name) {
					Destroy (WeaponSlots.transform.GetChild (i).gameObject);
					break;
				}
			CurrentWeapon.transform.GetChild (index).GetComponent<Weapon> ().ThrowCollectableWeapon ();
		}
	}

	void OnTriggerStay (Collider obj) {
		if (Input.GetKeyDown (KeyCode.E)) {
			if (obj.gameObject.tag == "Collectable") {
				WeaponInfo pickupWI = obj.gameObject.GetComponent<WeaponInfo> ();
				string Path = "Prefabs/Weapons/" + pickupWI.Name + "/" + "GUI" + pickupWI.Name;
				GameObject NewGUI = Instantiate ((GameObject)Resources.Load (Path), Vector3.zero, Quaternion.identity, WeaponSlots.transform);

				NewGUI.GetComponent<WeaponInfo>().Clips = pickupWI.Clips;
				NewGUI.GetComponent<WeaponInfo> ().LastShot = pickupWI.LastShot;
				NewGUI.transform.localPosition = new Vector3 (299 - 33 * WeaponSlots.transform.childCount, 142, 0);

				NewGUI.GetComponentInChildren<Animator> ().SetBool ("Wakey", true);
				WakeyTarget = NewGUI.GetComponentInChildren<Animator> ();
				Invoke ("TurnOffWakeyInGUIWeaponSelection", 0.05f);

				Path = "Prefabs/Weapons/" + pickupWI.Name + "/" + pickupWI.Name + "Right";
				TurnOffWakeyInGUIWeaponSelection ();
				if (CurrentWeapon.transform.childCount == 0) {
					GameObject NewbornWeapon = (GameObject)Resources.Load (Path);
					GameObject NWPN = Instantiate (NewbornWeapon, Vector3.zero, Quaternion.Euler (90, transform.rotation.eulerAngles.y, 0), CurrentWeapon.transform);
					NWPN.transform.localPosition = Vector3.zero;
					NWPN.GetComponent<Weapon> ().Clips = pickupWI.Clips;
				}
				Destroy (obj.transform.parent.gameObject);
			}
		}
	}

	void TurnOffWakeyInGUIWeaponSelection () {
		WakeyTarget.SetBool ("Wakey", false);
	}
}