using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auto_walk : MonoBehaviour {
	public Rigidbody2D ssb;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ssb.AddForce(new Vector2(1.00f,0.10f));
	}
}
