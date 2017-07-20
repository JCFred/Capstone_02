using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Animator animator;
	public int playerSpeed = 6;
	bool facingLeft = false;
	public float moveX;
	private float moveY;

	enum State {WALK, FALL, JUMP, ATTACK, SLIDE, WALL_JUMP};
	State playerState;
	Vector2 playerSize;
	Vector2 pos;

	public LayerMask wallGrabMask;

	bool jumpKey;
	float jumpForce;
	public float jumpHeight = 3.0f;
	bool groundedLastFrame = false;	
	public float wallJumpControlDelay = 0.15f;
	float wallJumpControlDelayLeft = 0;
	int wallJumpDir;

	public Vector2 test;

	Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		
		jumpForce = CalculateJumpForce(Physics2D.gravity.magnitude, jumpHeight);
		animator = this.GetComponent<Animator>();

		playerState = State.WALK;
		playerSize = GetComponent<CapsuleCollider2D> ().size;
	}
	
	// get player inputs
	void Update () {
		if (playerState != State.WALL_JUMP) {
			moveX = Input.GetAxis ("Horizontal") * playerSpeed;
		}

		jumpKey = Input.GetButton ("Jump");

		test = rb2d.velocity;
	}

	//player controller state engine
	void FixedUpdate () {
		pos = transform.position;

		if (playerState == State.WALK) {
			walkState ();
		} else if (playerState == State.FALL) {
			fallState ();
		} else if (playerState == State.JUMP) {
			jumpState ();
		} else if (playerState == State.ATTACK) {
			attackState ();
		} else if (playerState == State.SLIDE) {
			slideState ();
		} else if (playerState == State.WALL_JUMP) {
			wallJumpState ();
		}
	}

	//move and animate the player
	void LateUpdate () {
		Debug.Log (isGrabbing ());
		if (playerState != State.WALL_JUMP) {
			rb2d.velocity = new Vector2 (moveX, rb2d.velocity.y);
		} else {
			rb2d.velocity = new Vector2 (pos.x + moveX, pos.y + moveY);
		}


		FlipPlayer ();

		playerAnimate ();
	}


	//<<<<<<<<STATES>>>>>>>>>
	void walkState() {
		if (jumpKey && IsGrounded()) {
			Debug.Log("State switch to JUMP");
			playerState = State.JUMP;
			//apply initial jump force
			rb2d.AddForce (new Vector2(0,1) * jumpForce * rb2d.mass, ForceMode2D.Impulse);

		} else if (!IsGrounded()) {
			Debug.Log("State switch to FALL");
			playerState = State.FALL;
		}



	}


	void fallState () {
		if (IsGrounded ()) {
			Debug.Log ("State switch to WALK");
			playerState = State.WALK;
		}

		if (isGrabbing ()) {
			Debug.Log ("State switch to SLIDE");
			playerState = State.SLIDE;
		}
	}


	void jumpState () {
		if (rb2d.velocity.y < 0.1f) {
			Debug.Log("State switch to FALL");
			playerState = State.FALL;

		} else if (rb2d.velocity.y > 0.1f && !jumpKey) {
			rb2d.velocity = Vector2.zero;
			Debug.Log("State switch to FALL");
			playerState = State.FALL;
		}

		if (isGrabbing ()) {
			Debug.Log ("State switch to SLIDE");
			playerState = State.SLIDE;

		} else if (IsGrounded ()) {
			Debug.Log ("State switch to WALK");
			playerState = State.WALK;
		}
	}


	void attackState() {

	}

	void slideState () {
		wallJumpControlDelayLeft = 0;
		if (rb2d.velocity.y < 0) {
			rb2d.gravityScale = 0.0f;
		}

		if (!isGrabbing () && !IsGrounded ()) {
			Debug.Log ("State switch to FALL");
			playerState = State.FALL;
			rb2d.gravityScale = 1;
		} else if (IsGrounded ()) {
			Debug.Log ("State switch to WALK");
			playerState = State.WALK;
			rb2d.gravityScale = 1;
		} else if (Input.GetButtonDown ("Jump")) {
			Debug.Log ("State switch to WALL_JUMP");
			playerState = State.WALL_JUMP;
			rb2d.gravityScale = 1;
			if (moveX > 0.1f) {
				//rb2d.AddForce (new Vector2 (4, 1) * (jumpForce) * rb2d.mass, ForceMode2D.Impulse);
				wallJumpDir = 0;
			} else if (moveX < -0.1f) {
				//rb2d.AddForce (new Vector2 (-4, 1) * (jumpForce) * rb2d.mass, ForceMode2D.Impulse);
				wallJumpDir = 1;
			}
		}
	}

	void wallJumpState () {
		wallJumpControlDelayLeft += Time.deltaTime;
		if (wallJumpControlDelayLeft > wallJumpControlDelay) {
			Debug.Log ("State switch to JUMP");
			playerState = State.JUMP;
		//facing left, jumping right
		} else if (wallJumpDir == 0) {
			moveX = -playerSpeed * Time.deltaTime;
			moveY = jumpForce * Time.deltaTime;
		//facing right, jumping left
		} else if (wallJumpDir ==1) {
			moveX = playerSpeed * Time.deltaTime;
			moveY = jumpForce * Time.deltaTime;
		}
	}

	//<<<<<<<ANIMATION CONTROLL>>>>>>>
	void playerAnimate () {
		//WALK
		if (moveX > 0.0 || moveX < 0.0) {
			animator.SetInteger ("xSpeed", 1);
			//IDLE
		} else if (moveX == 0.0) {
			animator.SetInteger ("xSpeed", 0);
		} 

		//Jump
		if (rb2d.velocity.y > 0.1f) {
			animator.SetInteger ("ySpeed", 1);
			//FALL
		} else if (rb2d.velocity.y < -0.1f) {
			animator.SetInteger ("ySpeed", -1);
		} else if (rb2d.velocity.y == 0.0f) {
			animator.SetInteger ("ySpeed", 0);
		}
		//ATTACK FORWARD
		if (Input.GetKeyDown (KeyCode.X)) {
			animator.SetTrigger ("attack_forward");
		}
		//WALL SLIDING
		if (playerState != State.SLIDE) {
			animator.SetBool ("slide", false);
		} else {
			animator.SetBool ("slide", true);
		}
	}


	//<<<<<COllIDERS>>>>>

	//check if player is on the ground
	bool IsGrounded() {
		//if up/down veolocity is 0 for two consecutive frames
		if(Mathf.Abs ( rb2d.velocity.y ) < 0.1f) {	

			if(groundedLastFrame)
				return true;

			groundedLastFrame = true;
		}
		else {
			groundedLastFrame = false;
		}

		return false;
	}

	//check if player is grabbing wall 
	bool isGrabbing () {
		Debug.DrawRay (pos + new Vector2 (playerSize.x * 0.35f, playerSize.y * 0.35f), new Vector2 (0.1f, 0), Color.blue);
		if (moveX > 0) {
			return Physics2D.OverlapCircle(pos + new Vector2 (playerSize.x * 0.65f, playerSize.y * 0.4f), 0.2f);
		} else if (moveX < 0) {
			return Physics2D.OverlapCircle(pos + new Vector2 (-playerSize.x * 0.65f, playerSize.y * 0.4f), 0.2f);
		} else {
			return false;
		}

	}




	//<<<<<GENERAL FUNCTIONS>>>>>

	//Flip the player sprite according to user controls
	void FlipPlayer () {
		//PLAYER DIRECTION
		if (moveX < 0.0f && facingLeft == false) {
			facingLeft = !facingLeft;
			Vector2 localScale = gameObject.transform.localScale;
			localScale.x *= -1;
			transform.localScale = localScale;
		} else if (moveX > 0.0f && facingLeft == true) {
			facingLeft = !facingLeft;
			Vector2 localScale = gameObject.transform.localScale;
			localScale.x *= -1;
			transform.localScale = localScale;
		}
	}

	//find the jump power based on wanted hieght of jump in unit against the world gravity
	public static float CalculateJumpForce(float gravityStrength, float jumpHeight) {
     //h = v^2/2g
     //2gh = v^2
     //sqrt(2gh) = v
     return Mathf.Sqrt(2 * gravityStrength * jumpHeight);
 	}
}
