using UnityEngine;
using System.Collections;

public class fakeGoal : MonoBehaviour {

	void OnTriggerEnter(Collider player)
	{
		if (gameObject.tag == "Player") {
			gameObject.SetActive (false);
		}
		this.gameObject.SetActive (false);
	}
}