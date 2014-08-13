using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour {

	//Control Scheme
	public KeyCode Left = KeyCode.A;
	public KeyCode Right = KeyCode.D;
	public KeyCode Jump = KeyCode.W;
	public KeyCode Glide = KeyCode.Q;
	public KeyCode FreeFall = KeyCode.S;

	//Maximum stat values
	public float healthBar = 200f;
	public float staminaBar = 100f;
	public float fireBar = 100f;

	private float currentHealth;
	private float currentStamina;
	private float currentFire;

	//Rate at which stats regenerate
	public float healthRegen = 5f;
	public float staminaRegen = 30f;
	public float fireRegen = 10f;

	//Amount of stamina used by each activity
	public float staminaUsageJump = 10f;
	public float staminaUsageFlap = 5f;
	public float staminaUsageGlide = 2f;

	//Amount of fire used by each activity
	public float fireUsageFireball = 10f;
	public float fireUsageFlamethrower = 20f;

	//Global variables for movement
	public float gravity = -15f;
	public float runSpeed = 6f;
	public float decaySpeed = 0.5f;
	public float jumpHeight = 2.5f;
	public float flapHeight = 1.5f;
	public float glidingSpeed = 100f;

	private DragonCharacterController2D _controller;
	private Animator _animator;

	//Boolean values storing current state of dragon
	private bool isGliding;

	void Awake()
	{
		_controller = GetComponent<DragonCharacterController2D> ();
		_animator = GetComponent<Animator> ();

		currentHealth = healthBar;
		currentStamina = staminaBar;
		currentFire = fireBar;

	}

	void Update()
	{
		//Grabs current velocity of Dragon
		var velocity = _controller.velocity;


		//var stamina = staminaBar;

		//Sets velocity.y to 0 and regenerates stamina when on ground
		if (_controller.isGrounded)
		{
			velocity.y = 0;
			isGliding = false;
			if(currentStamina > staminaBar - 1)
				currentStamina = staminaBar;
			else
				currentStamina += staminaRegen * Time.deltaTime;
		}

		//Horizontal Input
		if (Input.GetKey (Right)) 
		{
			velocity.x = runSpeed;
			//goRight();
		}
		else if( Input.GetKey(Left))
		{
			velocity.x = -runSpeed;
			//goLeft();
		}

		//Adds decay speed to the x velocity to slow dragon down
		if(velocity.x < 0.001 && velocity.x > -0.001)
			velocity.x = 0;
		else
			velocity.x *= decaySpeed;
	
		//Jumping from the ground
		if (Input.GetKeyDown (Jump) && _controller.isGrounded && currentStamina > staminaUsageJump) 
		{
			var targetJumpHeight = jumpHeight;
			velocity.y = Mathf.Sqrt(2f * targetJumpHeight * -gravity);
			currentStamina -= staminaUsageJump;
		}

		//Flapping wings while in the air
		if(Input.GetKeyDown(Jump) && !_controller.isGrounded && currentStamina > staminaUsageFlap)
		{
			var targetFlapHeight = flapHeight;
			velocity.y = Mathf.Sqrt(2f * targetFlapHeight * -gravity);
			currentStamina -= staminaUsageFlap;
			isGliding = false;
		}

		//Initiate glide when in the air
		if(Input.GetKeyDown(Glide) && !_controller.isGrounded && currentStamina > staminaUsageGlide)
		{
			isGliding = true;
			currentStamina -= staminaUsageGlide;
			//Debug.Log("Glide!");
		}

		if (isGliding) 
		{
			if(velocity.y < -1)
				velocity.y += glidingSpeed * Time.deltaTime;
		}



		//Initiate freefall from glide
		if(Input.GetKeyDown(FreeFall) && !_controller.isGrounded && isGliding)
		{
			velocity.y += glidingSpeed * Time.deltaTime;
			isGliding = false;
		}

		//Apply gravity to y velocity
		velocity.y += gravity * Time.deltaTime;

		OnGUI();


		//Move dragon using the DragonCharacterController2D Script
		_controller.move(velocity * Time.deltaTime);
	}

	void OnGUI() {
		GUI.Label (new Rect (10, 10, 100, 20), "Dragon");
		GUI.Label(new Rect(10, 30, 150, 20), "Health: " + currentHealth + " / " + healthBar.ToString());
		GUI.Label(new Rect(10, 50, 150, 20), "Stamina: " + currentStamina + " / " + staminaBar.ToString());
		GUI.Label(new Rect(10, 70, 150, 20), "Fire: " + currentFire + " / " + fireBar.ToString());
		GUI.Label(new Rect(10, 90, 150, 20), "Is Grounded: " + _controller.isGrounded.ToString());
		GUI.Label(new Rect(10, 110, 150, 20), "Is Gliding: " + isGliding);
		//GUI.Label(new Rect(10, 130, 150, 20), "X Velocity: " + _controller.velocity.x);
		//GUI.Label(new Rect(10, 150, 150, 20), "Y Velocity: " + _controller.velocity.y);
	}

	private void goLeft()
	{
		if (transform.localScale.x > 0f)
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	private void goRight()
	{
		if (transform.localScale.x < 0f)
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}
}