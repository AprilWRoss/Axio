using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	private GameObject HealthbarScalator;
	private GameObject Parent;
	public int MaxHealth;
	//[HideInInspector]
	public int Health;
	public int TransitionAmount;
	public float TransitionTime;
	public float TransitionTrigger;

	void Start () {
		HealthbarScalator = transform.FindChild ("HealthbarSupplicant").gameObject;
		Parent = transform.parent.gameObject;
		Health = MaxHealth;
		InvokeRepeating ("DimHealth", 0, TransitionTime);
	}

	void Update () {
		transform.rotation = Quaternion.Euler (90, 0, 0);
		transform.position = Parent.transform.position + new Vector3 (0, 0.1f, 0.9f);
	}

	public void SubtractHealth (int amount) {
		ChangeHealth (Health - amount);
	}

	void ChangeHealth (int NewHealth) {
		Health = NewHealth;
		if (Health > MaxHealth)
			Health = MaxHealth;

		if (Health <= 0)
			Destroy (Parent);
		HealthbarScalator.transform.localScale = new Vector3 ((float)Health / MaxHealth, 1, 1);
		CancelInvoke ("DimHealth");
		WakeHealth ();
		if (Health == MaxHealth)
			InvokeRepeating ("DimHealth", TransitionTrigger, TransitionTime);
	}

	void WakeHealth () {
		SpriteRenderer[] HealthSprites = gameObject.GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < HealthSprites.Length; i++)
			HealthSprites [i].color = new Color (1, 1, 1, 1);
	}

	void DimHealth () {
		SpriteRenderer[] HealthSprites = gameObject.GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < HealthSprites.Length; i++)
			HealthSprites [i].color -= new Color (0, 0, 0, (float) TransitionAmount/255);

		if (HealthSprites [0].color.a <= 0)
			CancelInvoke ("DimHealth");
	}
}
