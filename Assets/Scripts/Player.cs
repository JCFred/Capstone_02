using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public bool triggered = false;

	//Jump variables
	public bool isJumping;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 0.4f;
	float maxJumpVelocity;
	float minJumpVelocity;
	float gravity;

	bool wallSliding;
	bool onMovingPlatform;

	//attack variables
	public bool attacking;
	public float attackPauseTime = .35f;
	float attackPaused;
	float attackRad = .35f;

	//health and taking damage variables
	int health = 6;
	bool damaged = false;
	public float dmgInvTime = .5f; 
	float invuln;
	float blinkTime = .05f;
	float blink;
	int color = 0;

	//smoothing and turn speed
	float accelerationTimeAir = .2f;
	float accelerationTimeGround = .1f;
	float velocityXSmoothing;

	public float wallSlidingSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeTillWallUnstick;
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	float moveSpeed = 6;
	Vector3 velocity;

	Controller2D controller;
	Animator animator;
	public LayerMask enemyLayerMask;
	public GameObject PlayerHitBox;

	public bool onGround;

	void Start () {
		controller = GetComponent<Controller2D> ();
		animator = GetComponent<Animator>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		attacking = false;
		isJumping = false;
	}


	void Update () {

		onGround = onMovingPlatform;

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGround : accelerationTimeAir);

		//WALL SLIDE
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && (!controller.collisions.below && velocity.y < 0)) {
			wallSliding = true;

			if (velocity.y < -wallSlidingSpeedMax) {
				velocity.y = -wallSlidingSpeedMax;
			}
			if (timeTillWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0) {
					timeTillWallUnstick -= Time.deltaTime;
				} else {
					timeTillWallUnstick = wallStickTime;
				}
			} else {
				timeTillWallUnstick = wallStickTime;
			}
		}

		//ATTACK 
		if (Input.GetButtonDown ("Fire1") && attackPaused < -attackPauseTime && !wallSliding) {
			attackPaused = attackPauseTime;
			attacking = true;
			Instantiate (PlayerHitBox);

			if (!isJumping) {
				animator.SetBool ("attackForward", true);

			} else if (isJumping) {
				animator.SetBool ("jumpAttackForward", true);
			}
		}
		attackPaused -= Time.deltaTime;

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
		if (controller.collisions.below) {
			isJumping = false;
		}

		if (attackPaused < -0.2f) {
			attacking = false;
		}

		//JUMP & WALL SLIDE JUMP
		if (Input.GetButtonDown ("Jump")) {
			if (wallSliding) {
				isJumping = true;
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				} else if (input.x != wallDirX && input.x != 0) {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;

				} else {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
			}

			if (controller.collisions.below || onMovingPlatform) {
				isJumping = true;
				velocity.y = maxJumpVelocity;
			}
		}
		if (Input.GetButtonUp ("Jump")) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		velocity.y += gravity * Time.deltaTime;
		if (!attacking) {
			controller.Move (velocity * Time.deltaTime);
		} else {
			velocity *= .3f;
			controller.Move (velocity * Time.deltaTime);
		}

		//Player took damage
		if (damaged) {
			invuln += Time.deltaTime;
			if (invuln >= dmgInvTime) {
				if (color == 1) {
					GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
					color = 0;
				}
				damaged = false;
				GameObject.Find("PlayerHealth").SendMessage("updateHealth");
			} else {
				//Blink white and grey
				blink += Time.deltaTime;
				if (blink >= blinkTime) {
					blink = 0.0f;
					if (color == 0) {
						GetComponent<SpriteRenderer> ().color = new Color (0.5f, 0.5f, 0.5f, 1f);
						color = 1;
					} else {
						GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
						color = 0;
					}
				}
			}
		}

	}

	//Controll player animations
	void LateUpdate () {
		//Flip the player with facingDirection
		Vector2 scale = gameObject.transform.localScale;
		if (controller.collisions.faceDirection == -1) {
			scale.x = 1;
		} else {
			scale.x = -1;
		}
		transform.localScale = scale;

		if (!attacking) {
			//Start and stop the running animation
			if (velocity.x > 0.19f || velocity.x < -0.19f) {
				animator.SetInteger ("xSpeed", 1);
			} else {
				animator.SetInteger ("xSpeed", 0);
			}

			//Jump animations
			if (controller.collisions.below) {
				animator.SetInteger ("ySpeed", 0);

			} else if (velocity.y > 0.1f) {
				animator.SetInteger ("ySpeed", 1);

			} else if (velocity.y < -0.1f) {
				animator.SetInteger ("ySpeed", -1);
			}

			if (wallSliding && velocity.x == 0) {
				animator.SetBool ("slide", true);
			} else {
				animator.SetBool ("slide", false);
			}
		}
	}


	void attackSwing () {
		Vector2 HitBoxPos = transform.position;
		HitBoxPos.x += controller.collisions.faceDirection * .93f;
	
//		Vector2 highAttackPos = transform.position;
//		highAttackPos.x += controller.collisions.faceDirection * .7f + (velocity.x * Time.deltaTime);
//		highAttackPos.y += .3f;
//		Vector2 lowAttackPos = transform.position;
//		lowAttackPos.x += controller.collisions.faceDirection * .9f + (velocity.x * Time.deltaTime);
//		lowAttackPos.y -= .2f;
//		if (Physics2D.OverlapCircle (highAttackPos, attackRad, enemyLayerMask) || Physics2D.OverlapCircle (lowAttackPos, attackRad, enemyLayerMask)) {
//			Debug.Log ("enemy Hit");
//		}

		//TESTING ATTACK HIT COLLIDER CIRCLES
		//TOP HIT CIRCLE
		Vector2 attackPos = transform.position;
		Vector2 attackPos2 = transform.position;
		attackPos.x += controller.collisions.faceDirection * .7f + (velocity.x * Time.deltaTime);
		attackPos.y += .3f;
		Debug.DrawRay (attackPos, new Vector2(controller.collisions.faceDirection * attackRad, 0), Color.red);
		Debug.DrawRay (attackPos, new Vector2(-controller.collisions.faceDirection * attackRad, 0), Color.red);
		Debug.DrawRay (attackPos, new Vector2(0, attackRad), Color.red);
		Debug.DrawRay (attackPos, new Vector2(0, -attackRad), Color.red);
		//BOTTOM HIT CIRCLE
		attackPos2.x += controller.collisions.faceDirection * .9f + (velocity.x * Time.deltaTime);
		attackPos2.y -= .2f;
		Debug.DrawRay (attackPos2, new Vector2(controller.collisions.faceDirection * attackRad, 0), Color.blue);
		Debug.DrawRay (attackPos2, new Vector2(-controller.collisions.faceDirection * attackRad, 0), Color.blue);
		Debug.DrawRay (attackPos2, new Vector2(0, attackRad), Color.blue);
		Debug.DrawRay (attackPos2, new Vector2(0, -attackRad), Color.blue);
	}

	void PlayerDamaged () {
		GameObject.Find("PlayerHealth").SendMessage("healthDownOne");
				damaged = true;
				blink =0;
				invuln = 0;
	}

	public void attackDone () {
		animator.SetBool ("attackForward", false);
		animator.SetBool ("jumpAttackForward", false);
		attacking = false;
	}

	void onPlatform () {
		onMovingPlatform = true;
		controller.collisions.below = true;
	}
	void offPlatform () {
		onMovingPlatform = false;
	}

	void checkpointOne () {
		transform.position = new Vector2 (-4.966f, -13.748f);
		GameObject.Find("PlayerHealth").SendMessage("healthDownOne");
		damaged = true;
		blink =0;
		invuln = 0;
	}

	void checkpointTwo () {
		transform.position = new Vector2 (-26.797f, -10.602f);
		GameObject.Find("PlayerHealth").SendMessage("healthDownOne");
		damaged = true;
		blink =0;
		invuln = 0;
	}
}
