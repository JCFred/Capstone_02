using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthControl : MonoBehaviour {

	
	public Vector2 hpPos = new Vector2 (-3, 3);


	Vector2 CameraPos;
	public int playerHealth;
	bool flashing;

	float flashingSpeed = .15f;
	float currentFlash;
	int blink;

	Animator animator;
	Camera MainCamera;


	// Use this for initialization
	void Start () {
		MainCamera = Camera.main;
		animator = GetComponent<Animator>();

		playerHealth = 6;
		flashing = false;
		blink = 0;
	}
	
	//Update is called once per frame
	void UpdatePos () {
		CameraPos.x = MainCamera.transform.position.x;
		CameraPos.y = MainCamera.transform.position.y;


		transform.position = new Vector2 (CameraPos.x + hpPos.x, CameraPos.y + hpPos.y);

		if (flashing) {
			animator.SetBool ("flash", true);
			currentFlash += Time.deltaTime;
			if (currentFlash >= flashingSpeed) {
				currentFlash = 0;
				if (blink == 0) {
					blink = 1;
					GetComponent<SpriteRenderer> ().color = new Color (0.5f, 0.5f, 0.5f, 1f);
				} else {
					blink = 0;
					GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				}
			}

		}
	}

	void healthDownOne () {
		playerHealth -= 1;
		flashing = true;
		currentFlash = 0;
		if (playerHealth <= 0) {
			//GAME OVER!!
		}
	}

	void updateHealth () {
		flashing = false;
		blink = 0;
		GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
		switch (playerHealth) {
			case 6:
				animator.SetInteger("health", 6);
				break;
			case 5:
				animator.SetInteger("health", 5);
				break;
			case 4:
				animator.SetInteger("health", 4);
				break;
			case 3:
				animator.SetInteger("health", 3);
				break;
			case 2:
				animator.SetInteger("health", 2);
				break;
			case 1:
				animator.SetInteger("health", 1);
				break;
		}
		animator.SetBool ("flash", false);

	}

}
