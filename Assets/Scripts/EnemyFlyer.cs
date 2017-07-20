using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyer : MonoBehaviour {

	float health = 2;
	float playerDamage = 1.5f;


	bool hitByPlayer;
	Vector2 hitDir;
	float takeDmgTimer = .5f;
	float takeDmg;

	Vector3 velocity;

	Vector2 startPos;

	Animator animator;

	// Use this for initialization
	void Start () {
		startPos.x = transform.position.x;
		startPos.y = transform.position.y;
		animator = GetComponent<Animator>();
		hitByPlayer = false;
	}

	//Hit my player attack
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "attackHitBox") {
			if (!hitByPlayer) {
				float colDirX = GameObject.FindGameObjectWithTag ("Player").transform.position.x - transform.position.x;
				float colDirY = GameObject.FindGameObjectWithTag ("Player").transform.position.y - transform.position.y;

				if (colDirX > 0.0f) {
					hitDir.x = -1;
				} else if (colDirX < 0.0f) {
					hitDir.x = 1;
				}

				if (colDirY > 0.0f) {
					hitDir.y = -1;
				} else if (colDirY < 0.0f) {
					hitDir.y = 1;
				}

				hitByPlayer = true;
				Debug.Log (health);
				velocity.x = 0;
				health -= playerDamage;
				takeDmg = 0;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (hitByPlayer) {
			

			dmgTimer ();
		}
	}


	void LateUpdate () {
		if (hitByPlayer) {
			animator.SetBool ("damaged", true);
			GetComponent<SpriteRenderer> ().color = new Color (1, 0, 0, 1);
		} else {
			animator.SetBool ("damaged", false);
			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
		}
	}

	void dmgTimer () {
		takeDmg += Time.deltaTime;

		if (takeDmg >= takeDmgTimer) {
			hitByPlayer = false;
		}
	}
}
