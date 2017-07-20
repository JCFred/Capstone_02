using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inspiration : MonoBehaviour {

	bool hitByPlayer;
	public GameObject textBox;
	Animator animator;
	int oneHit = 1;

	public bool isFirst;
	public bool isSecond;
	public bool isThird;


	void Start () {
		animator = GetComponent<Animator>();
	}


	//Hit by player attack
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "attackHitBox") {
			if (!hitByPlayer) {
				hitByPlayer = true;
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (hitByPlayer && oneHit > 0) {
			animator.SetBool ("break", true);
			if (isFirst) {
				GameObject.FindGameObjectWithTag ("textBox").SendMessage ("firstBulb");
			} else if (isSecond) {
				GameObject.FindGameObjectWithTag ("textBox").SendMessage ("secondBulb");
			} else if (isThird) {
				GameObject.FindGameObjectWithTag ("textBox").SendMessage ("thirdBulb");
			}

			oneHit -= 1;	
		}
	}
}
