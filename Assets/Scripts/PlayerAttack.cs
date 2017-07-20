using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	float timerMax = .35f;
	float timer = 0;
	GameObject player;
	float facing;
	void Start () {
		player = GameObject.FindWithTag("Player");
	}

	void Update () {
		Vector2 playerPos = player.transform.position;
		facing = player.transform.localScale.x;

		playerPos.x += .93f * -facing;
		transform.position = playerPos;

		Vector2 scale = Vector2.up;
		scale.x = facing;
		transform.localScale = scale;
	
		timer += Time.deltaTime;
		if (timer >= timerMax) {
			Destroy(gameObject);
		}
	}
}
