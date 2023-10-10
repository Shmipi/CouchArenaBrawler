using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
	public float speed = 25.0F;
	public float airSpeed;
	public float jumpSpeed = 20.0F;
	public float gravity = 80.0F;
	public int maxJumps = 2;
	private int nrOfJumps;
	private Vector2 moveDirection = Vector2.zero;

	// Use this for initialization
	void Start()
	{
		airSpeed = speed / 2;
		nrOfJumps = 0;
	}

	// Update is called once per frame
	void Update()
	{
		CharacterController controller = GetComponent<CharacterController>();

		// is the controller on the ground?
		if (controller.isGrounded)
		{
			nrOfJumps = 0;
			//Feed moveDirection with input.
			moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
			moveDirection = transform.TransformDirection(moveDirection);
			//Multiply it by speed.
			moveDirection *= speed;
			//Jumping
			if (Input.GetButtonDown("Jump"))
            {
				Jump();
			}
		} else
        {
			moveDirection.x = Input.GetAxis("Horizontal") * speed;

			if (Input.GetButtonDown("Jump") && nrOfJumps < maxJumps)
			{
				Jump();
			}
		}
		//Applying gravity to the controller
		moveDirection.y -= gravity * Time.deltaTime;
		//Making the character move
		controller.Move(moveDirection * Time.deltaTime);
	}

	void Jump()
    {
		moveDirection.y = jumpSpeed;
		nrOfJumps++;
	}
}
