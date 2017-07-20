using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController {

	public LayerMask passengerMask;
	Vector3 move;
	public float speed = 4;
	public bool horizontalMovement = false;
	public float moveDistance = 4;
	Vector2 startPos;
	int dir;

	List<PassengerMovement> passengerMovement;

	public override void Start () {
		base.Start ();
		dir = 1;
		startPos.x = transform.position.x;
		startPos.y = transform.position.y;
	}
	

	void Update () {
		UpdateRayCastOrigins ();

		//Platform moving right and left
		if (horizontalMovement) {
			move.y = 0;
			//start right
			if (transform.position.x == startPos.x) {
				move.x = speed;
			}
			//move right
			if (dir == 1) {
				if (transform.position.x > startPos.x && Mathf.Abs(startPos.x - transform.position.x) >= moveDistance) {
					dir = -1;
					move.x = -speed;
				}
			//move left
			} else {
				if (transform.position.x < startPos.x && Mathf.Abs(startPos.x - transform.position.x) >= moveDistance) {
					dir = 1;
					move.x = speed;
				}
			}

		//Platform moving up and down
		} else {
			move.x = 0;
			//start up
			if (transform.position.y == startPos.y) {
				move.y = speed;
			}
			//move up
			if (dir == 1) {
				if (transform.position.y > startPos.y && Mathf.Abs(startPos.y - transform.position.y) >= moveDistance) {
					dir = -1;
					move.y = -speed;
				}
			//move down
			} else {
				if (transform.position.y < startPos.y && Mathf.Abs(startPos.y - transform.position.y) >= moveDistance) {
					dir = 1;
					move.y = speed;
				}
			}
		}

		Vector3 velocity = move * Time.deltaTime;

		CalculatePassengerMovement (velocity);

		MovePassengers (true);
		transform.Translate (velocity);
		MovePassengers (false);
	}

	void MovePassengers (bool beforeMovePlatform) {
		if (passengerMovement.Count == 1) {
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("onPlatform");
		} else {
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("offPlatform");
		}

		foreach (PassengerMovement passenger in passengerMovement) {
			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passenger.transform.GetComponent<Controller2D>().Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

	void CalculatePassengerMovement (Vector3 velocity) {
		HashSet<Transform> movePassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		//vertically moving platform
		if (velocity.y != 0) {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit) {
					if (!movePassengers.Contains (hit.transform)) {
						movePassengers.Add (hit.transform);
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3 (pushX, pushY), directionY == 1, true)); 
					}
				}
			}
		}

		//horizontally moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit) {
					hit.collider.gameObject.SendMessage("onPlatform");
					if (!movePassengers.Contains (hit.transform)) {
						movePassengers.Add (hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = 0;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3 (pushX, pushY), false, true)); 
					}
				}
			}
		}

		//Passenger on top horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = 2 * skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, rayLength, passengerMask);

				if (hit) {
					if (!movePassengers.Contains (hit.transform)) {
						movePassengers.Add (hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3 (pushX, pushY), true, false)); 
					}
				}
			}
		}
	}

	struct PassengerMovement {
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}
}
