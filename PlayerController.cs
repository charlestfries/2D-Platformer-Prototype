using UnityEngine;
using System.Collections;

// Allows class to be attached to GameObjects
public class PlayerController : MonoBehaviour
{
    // Editable public variables for player movement
    public float speed = 5f; // Movement speed
    public float jumpForce = 7f; // Jump force
    public float dashDistance = 15f; // Dash distance
    public float fallMultiplier = 2.5f; // Gravity multiplier
    public float lowJumpMultiplier = 2f; // Gravity multiplier for short jumps
    public float dashCooldown = 2f; // Dash cooldown
    public float wallSlideSpeed = 2f; // Wall slide
    public float wallJumpPushOffDistance = 2f; // Controls the horizontal push-off distance
    public float wallJumpPushOffTime = 0.2f; // For wall jump smoothing


    // Private state variables
    private Rigidbody rb; // Controls physics interactions
    private bool isGrounded; // Tracks if player is touching the ground
    private bool isDashing; // Indicates if a dash is in progress
    private bool canDash = true; // Checks if player can initiate a dash
    private bool isTouchingWall; // Checks wall contact
    private int jumpCount = 0; // Counts consecutive jumps
    private int maxJumpCount = 1; // Limits consecutive jumps
    private float dashTime = 0.2f; // Sets duration of a dash
    private float dashTimer; // Measures time remaining in dash
    private float timeSinceLastDash; // Tracks cooldown period between dashes
    private Vector3 dashDirection; // Determines direction of dash
    private float timeSinceLastWallJump = 0f; // Timer to track wall jump cooldown
    private bool isWallJumping = false; // For wall jump smoothing

    // Indicates direction player is facing
    private int facingDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Rigidbody component
        rb = GetComponent<Rigidbody>();
        // Set timer based on cooldown for immediate dashing
        timeSinceLastDash = dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // Call movement and ability logic functions
        Move();
        JumpLogic();
        DashLogic();
        WallSlideLogic();
        timeSinceLastDash += Time.deltaTime;
    }

    // Handles player movement
    void Move()
    {
        // Prevent movement while dashing
        if (isDashing) return;
        // Get horizontal input and calculate movement vector
        float x = Input.GetAxis("Horizontal") * speed;
        Vector3 movement = new Vector3(x, rb.velocity.y, 0);
        // Apply calculated movement to the Rigidbody
        rb.velocity = movement;
        // Update facing direction based on input
        if (x > 0) facingDirection = 1;
        else if (x < 0) facingDirection = -1;
    }

    // Handles jumping logic
    void JumpLogic()
    {
        // Check for jump input, ground status, and jump count
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isTouchingWall))
        {
            if (isTouchingWall && !isGrounded)
            {
                // Wall jump push player off the wall
                StartCoroutine(SmoothWallJump(-facingDirection * wallJumpPushOffDistance, jumpForce));
            }
            else
            {
                // Regular jump
                rb.velocity = Vector3.up * jumpForce;
            }
            // Reset ground status and increment jump count
            isGrounded = false;
            jumpCount++;
        }
        // Apply additional gravity for faster falling
        if (rb.velocity.y < 0)
        {
            // Increases the speed of falling
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Apply modified gravity for shorter jumps
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            //Makes jumps shorter if the jump button is released early
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Handles dash logic
    void DashLogic()
    {
        // Check for dash input and cooldown
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Z)) && canDash && timeSinceLastDash >= dashCooldown)
        {
            // Initiate dash
            StartDash();
            // Reset dash cooldown timer and stop further dashing until cooldown is up
            timeSinceLastDash = 0;
            canDash = false;
        }
        // Continue dash movement if dashing
        if (isDashing)
        {
            ContinueDash();
        }
        // Re-enable dashing once cooldown is up
        if (timeSinceLastDash >= dashCooldown)
        {
            canDash = true;
        }
    }

    // Handles wall slide logic
    void WallSlideLogic()
    {
        if (isTouchingWall && !isGrounded)
        {
            //Slows down the fall when sliding down a wall
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(-wallSlideSpeed, rb.velocity.y), 0);
        }
    }

    // Initiates the dash movement
    void StartDash()
    {
        // Set dashing state and dash timer
        isDashing = true;
        dashTimer = dashTime;
        // Reset current velocity and calculate dash direction and distance
        rb.velocity = Vector3.zero;
        dashDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0).normalized * dashDistance;
    }

    // Continues the dash movement for the duration of the dash
    void ContinueDash()
    {
        // Decrement the dash timer based on elapsed time
        dashTimer -= Time.deltaTime;
        // Check if the dash duration has ended
        if (dashTimer <= 0)
        {
            // End the dash and exit the function
            isDashing = false;
            return;
        }

        // Apply the dash velocity in the calculated direction during the dash
        rb.velocity = dashDirection / dashTime;
    }

    IEnumerator SmoothWallJump(float pushOffDistance, float jumpForce)
    {
        // Begins the wall jump
        isWallJumping = true;

        // Timer for how long the smooth wall jump lasts
        float timer = 0;
        while (timer < wallJumpPushOffTime)
        {
            // Adds time since the last frame to the timer
            timer += Time.deltaTime;
            float fraction = timer / wallJumpPushOffTime;

            // Smoothly adjust the wall jump movement
            rb.velocity = new Vector3(Mathf.Lerp(0, pushOffDistance, fraction), rb.velocity.y, 0);

            // Applies an upward force to jump off the wall
            if (timer <= Time.deltaTime)
            {
                // Starts the wall jump by pushing the player up.
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
            }

            // Waits for the next frame before continuing the loop
            yield return null;
        }

        // Ends the wall jump
        isWallJumping = false;
    }

    // Detects collisions with other objects
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player has collided with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Set the player as grounded and reset jump count
            isGrounded = true;
            jumpCount = 0;
        }
        // Check if the player has collided with a wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Allow the player to jump again after hitting a wall
            isTouchingWall = true;
            jumpCount = 0;
        }
    }

    // Detects when the player leaves a collision with the ground or a wall
    private void OnCollisionExit(Collision collision)
    {
        // Check if the player has stopped touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Update the player's ground status to not grounded
            isGrounded = false;
        }
        // Check if the player has stopped touching the wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Update the player's ground status to not touching wall
            isTouchingWall = false;
        }
    }
}