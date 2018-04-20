using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerControler : MonoBehaviour {
//	[RequireComponent(typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(Animator))]

	public float speed = 14f;
	public float accel = 6f;

	private Vector2 input;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private Animator animator;

	void start(){
		sr = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void update(){
		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Jump");
		if (input.x > 0f) {
			sr.flipX = false;
		} else if (input.x < 0f) {
			sr.flipX = true;
		}

		var acceleration = accel;
		var xVelocity = 0f;
		if (input.x == 0) {
			xVelocity = 1f;
		} else {
			xVelocity = rb.velocity.x;
		}
		rb.AddForce (new Vector2 (((input.x * speed) + rb.velocity.x) * acceleration, 0));
		rb.velocity = new Vector2 (xVelocity, rb.velocity.y);
	}
}