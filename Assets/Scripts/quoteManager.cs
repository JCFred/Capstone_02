using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quoteManager : MonoBehaviour {

	bool quoteOn = false;
	public float totalQuoteTime = 6.5f;
	float quoteTime;
	int currentQuote = 1;

	Vector2 startPos;
	public GameObject bulbOne;
	public GameObject quoteOne;

	public GameObject bulbTwo;
	public GameObject quoteTwo;

	public GameObject bulbThree;
	public GameObject quoteThree;

	// Use this for initialization
	void Start () {
		startPos = transform.position;


	}
	
	// Update is called once per frame
	void Update () {
		if (quoteOn) {
			quoteTime += Time.deltaTime;
			if (quoteTime >= totalQuoteTime) {
				transform.position = startPos;
				quoteOn = false;

				switch (currentQuote) {
					case 1:
						quoteOne.SendMessage("goHome");
						break;
					case 2:
						quoteTwo.SendMessage("goHome");
						break;
					case 3:
						quoteThree.SendMessage("goHome");
						break;
				}
			}
		}
	}

	void firstBulb () {
		quoteOn = true;
		quoteTime = 0;
		Vector2 newPos = new Vector2 (bulbOne.transform.position.x, bulbOne.transform.position.y + 2);
		transform.position = newPos;
		quoteOne.transform.position = transform.position;

	}

	void secondBulb () {
		currentQuote = 2;
		quoteOn = true;
		quoteTime = 0;
		Vector2 newPos = new Vector2 (bulbTwo.transform.position.x, bulbTwo.transform.position.y + 2);
		transform.position = newPos;
		quoteTwo.transform.position = transform.position;

	}

	void thirdBulb () {
		currentQuote = 3;
		quoteOn = true;
		quoteTime = 0;
		Vector2 newPos = new Vector2 (bulbThree.transform.position.x, bulbThree.transform.position.y + 2);
		transform.position = newPos;
		quoteThree.transform.position = transform.position;

	}
}
