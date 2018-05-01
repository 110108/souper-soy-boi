using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reset : MonoBehaviour {
	private Vector3 resetPos;
	private Vector2 currentPos;
	public GameObject player;
	public int depth;

	// Use this for initialization
	void Start () {
		resetPos = new Vector3 (transform.position.x, transform.position.y+2, transform.position.z);
	}

	// Update is called once per frame
	void Update () {
		currentPos = new Vector2 (transform.position.x, transform.position.y);
		if (currentPos.y<depth){
			player.transform.position = resetPos;
		}
	}
}