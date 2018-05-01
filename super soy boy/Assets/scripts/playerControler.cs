using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerControler : MonoBehaviour {
	public GameObject player;
	public int y;
	private Vector3 reset;
	private Vector3 pos;

	void start(){
		reset = player.transform.position;
	}

	void update(){
		pos = player.transform.position;
		if(pos.y<y){
			player.transform.position=reset;
		}
	}
}
