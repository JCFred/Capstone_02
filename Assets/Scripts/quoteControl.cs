using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quoteControl : MonoBehaviour {

	Renderer renderer;
	Vector2 startPos;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<MeshRenderer>();
		renderer.sortingLayerName = "Quotes";
		renderer.sortingOrder = 2;
		startPos = transform.position;
	}
	
	void goHome () {
		transform.position = startPos;
	}
}
