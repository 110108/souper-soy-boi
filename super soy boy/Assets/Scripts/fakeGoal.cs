using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeGoal : MonoBehaviour {
	public GameObject This;

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Player") {
			gameObject.SetActive (false);
			Debug.Log ("trigger");
		}
		if(gameObject.tag=="fakeout"){
			Debug.Log ("trigger0");
		}
		Debug.Log ("trigger1");

	}
}
