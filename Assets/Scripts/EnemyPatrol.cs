using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class EnemyPatrol : MonoBehaviour {

	public float moveSpeed = 2;
	public float gravity = -2;

	public float health = 4f;
	public float playerDamage = 1.5f;

	bool hitByPlayer;
	int hitDir;
	float takeDmgTimer = .2f;
	float takeDmg;

	int faceingDirection = 1;
	bool dead;


	Vector3 velocity;

	public LayerMask collisionMask;
	Controller2D controller;
	Animator animator;

	void Start () {
		controller = GetComponent<Controller2D> ();
		animator = GetComponent<Animator>();
		hitByPlayer = false;
		dead = false;

	}

	//Hit my player attack
	void OnTriggerEnter2D (Collider2D other) {
		//hit play player's attack hitbox
		if (other.gameObject.tag == "attackHitBox") {
			if (!hitByPlayer && !dead) {
				float colDir = GameObject.FindGameObjectWithTag ("Player").transform.position.x - transform.position.x;
				if (colDir > 0.0f) {
					hitDir = -1;
				} else if (colDir < 0.0f) {
					hitDir = 1;
				}
				hitByPlayer = true;
				velocity.x = 0;
				health -= playerDamage;
				takeDmg = 0;
			}
		}
		//hit the player's hit box
		if (other.gameObject.tag == "Player") {
			if (!dead) {
				GameObject.Find("Player").SendMessage("PlayerDamaged");
			}
		}
	}


	void Update () {
		if (health <= 0) {
			dead = true;
		}

		if (controller.collisions.right && controller.collisions.below) {
			faceingDirection = -1;
		} else if (controller.collisions.left && controller.collisions.below) {
			faceingDirection = 1;
		} else if (controller.collisions.below && OnLedge ()) {
			faceingDirection *= -1;
		}

		if (!hitByPlayer && !dead) {
			velocity.x = moveSpeed * faceingDirection;
			velocity.y += gravity * Time.deltaTime;
		} else if (!dead) {
			velocity.x = (moveSpeed * 3) * hitDir;
			velocity.y += .075f;
			dmgTimer ();
		}
			

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		if (!dead) {
			controller.Move (velocity * Time.deltaTime);
		}
	}

	//Controll player animations
	void LateUpdate () {
		if (dead) {
			animator.SetBool ("dead", true);
		}

		//Flip the player with facingDirection
		if (!hitByPlayer) {
			Vector2 scale = gameObject.transform.localScale;
			if (controller.collisions.faceDirection == 1) {
				scale.x = 1;
			} else {
				scale.x = -1;
			}
			transform.localScale = scale;
		}

		if (hitByPlayer) {
			animator.SetBool ("hit", true);
		} else {
			animator.SetBool ("hit", false);
		}
	}

	bool OnLedge () {
		Bounds bounds = GetComponent<BoxCollider2D> ().bounds;

		if (faceingDirection == -1) {
			Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
			RaycastHit2D hit = Physics2D.Raycast (bottomLeft, Vector2.down, 0.05f, collisionMask);

			Debug.DrawRay (bottomLeft, Vector2.down * 0.05f, Color.blue);
			if (hit) {
				return false;
			} else {
				return true;
			}
		} else {
			Vector2 bottomRight = new Vector2(bounds.max.x, bounds.min.y);
			RaycastHit2D hit = Physics2D.Raycast (bottomRight, Vector2.down, 0.05f, collisionMask);

			Debug.DrawRay (bottomRight, Vector2.down * 0.05f, Color.blue);
			if (hit) {
				return false;
			} else {
				return true;
			}
		}
	}

	void dmgTimer () {
		takeDmg += Time.deltaTime;

		if (takeDmg >= takeDmgTimer) {
			hitByPlayer = false;
		}
	}


}
