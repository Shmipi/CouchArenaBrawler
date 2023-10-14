using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    private float horizontal;
    private float speed = 20f;
    private float jumpingPower = 30f;
    private bool isFacingRight = true;

    private bool isWallSliding;
    private float wallSlidingSpeed = 3f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float WallJumpingDuration = 0.4f;
    private Vector3 wallJumpingPower = new Vector3(12f, 20f);

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private float gravityScale = 4f;
    public static float globalGravity = -9.81f;

    private int maxJumps = 2;
    private int jumpCount = 0;

    public Transform handTransform;

    private ThrowableScript throwable;
    private bool canPickUp;
    private bool holdingWeapon;

    private void Start()
    {
        rb.useGravity = false;
        canPickUp = false;
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                jumpCount = 0;
                rb.velocity = new Vector3(rb.velocity.x, jumpingPower, 0);
                jumpCount++;
            } else
            {
                if(jumpCount < maxJumps)
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpingPower, 0);
                    jumpCount++;
                }
            }
        }

        if(Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, 0);
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.E) && canPickUp)
        {
            PickUp();
        }

        if (holdingWeapon && Input.GetKeyDown(KeyCode.K))
        {
            throwable.ThrowWeapon(transform);
            throwable = null;
            holdingWeapon = false;
        }

    }

    private void FixedUpdate()
    {
        if(!isWallJumping)
        {
            rb.velocity = new Vector3(horizontal * speed, rb.velocity.y, 0f);
        }

        if (!isWallSliding)
        {
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics.CheckSphere(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if(IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, -1), 0f);
        } else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector3(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if(transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), WallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Throwable")
        {
            Debug.Log("Time to pick up!");
            canPickUp = true;
            throwable = other.GetComponent<ThrowableScript>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Throwable")
        {
            Debug.Log("Leaving throwable!");
            canPickUp = false;
        }
    }

    private void PickUp()
    {
        Debug.Log("Picked up!");
        holdingWeapon = true;
        throwable.PickUp(handTransform);
    }
}
