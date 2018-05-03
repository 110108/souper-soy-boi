using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControler : PhysicsObject {
	
	public float maxSpeed = 7;
	public float jumpForce = 7;

	private SpriteRenderer sr;
	private Animator an;
	private Rigidbody2D rb;

	public bool grounded(){
		bool gc1 = Physics2D.Raycast (new vector2 (Transform.position.x, Transform.position.y - height), -Vector2.up, rayCastLengthCheck);
		bool gc2 = Physics2D.Raycast (new Vector2 (Transform.position.x + (width - 0.2f), Transform.position.y - height), -Vector2.up, RayCastLengthCheck);
		bool gc3 = Physics2D.Raycast (new Vector2 (Transform.position.x - (width - 0.2f), Transform.position.y - height), -Vector2.up, RayCastLengthCheck);
		if(gc1||gc2||gc3){
			return true;
		}else{
			return false;
		}
	}

	void Awake () {
		sr = GetComponent ();
		an = GetComponent ();
		rb = GetComponent ();
	}

	void ComputeVelocity () {
		Vector2 move = Vector2.zero;

		move.x = Input.GetAxis ("Horizontal");

		if (Input.GetButtonDown ("Jump") && grounded) {
			rb.velocity.y = jumpForce;
		}else if(Input.GetButtonUp("jump")){
			if(rb.velocity.y == 0){
				rb.velocity.y=rb.velocity.y*0.5f;
			}
		}

		bool flipsprite=(sr.flipX  (move.x &gt; 0.01f) : (move.x &lt; 0.01f));
		if(flipsprite){
			sr.flipX= !sr.flipX;
		}
		an.SetBool("grounded", grounded);
		an.SetFloat("velocityX", Mathf.Abs(rb.velocity.x)/maxSpeed);

		targetVelocity=move*maxSpeed;
	}
}
