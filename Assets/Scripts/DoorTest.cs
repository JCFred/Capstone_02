using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTest : MonoBehaviour {

	public float newX;
	public float newY;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "Player") {
			Debug.Log ("Correct collision");
			Vector2 otherPos = other.gameObject.transform.position;
			otherPos.x = newX;
			otherPos.y = newY;
			other.gameObject.transform.position = otherPos;

		}
    }


}
