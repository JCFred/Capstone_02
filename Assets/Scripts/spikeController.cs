using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeController : MonoBehaviour {

	public bool firstCheckPoint;
	public bool secondCheckPoint;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			if (firstCheckPoint) {
				GameObject.Find("Player").SendMessage("checkpointOne");
			}

			if (secondCheckPoint) {
				GameObject.Find("Player").SendMessage("checkpointTwo");
			}
		}
	}
}
