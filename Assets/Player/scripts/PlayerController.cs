using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{

    public float groundDistance = 0.1f;
    public float maxFallVelocity = 30;
    public float maxCoyeteTime = 0.05f;
    public float horizontalSpeed = 2;
    public float jumpForce = 3f;
    public float jumpHoldForce = 5f;
    public float maxJumpTime= 0.6f;
    public float linkDistance=6;

    public LayerMask layerGround;
    public Transform transfromGroundLeftCheck;
    public Transform transfromGroundRightCheck;

    public static bool IsSeparate { get; protected set; }
    
    public float LinkDistanceSqrMagnitude { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsTouchingGround { get; private set; }

    protected Rigidbody2D mRigidbody;
    private float coyoteTime;
    private float jumpTime;

    public abstract PlayerController GetOtherPart();

    protected virtual void Awake()
    {
        groundDistance = Mathf.Abs(groundDistance);
        maxFallVelocity = (-1) * Mathf.Abs(maxFallVelocity);
        mRigidbody = GetComponent<Rigidbody2D>();
        LinkDistanceSqrMagnitude = linkDistance * linkDistance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RaycastHit2D groundLeftCheckHit = Physics2D.Raycast(transfromGroundLeftCheck.position, Vector2.down, groundDistance, layerGround);
        RaycastHit2D groundRightCheckHit = Physics2D.Raycast(transfromGroundRightCheck.position, Vector2.down, groundDistance, layerGround);
        IsTouchingGround = (groundLeftCheckHit || groundRightCheckHit);

        HoldJump();
        Vector2 velocity = mRigidbody.velocity;
        if (velocity.y < maxFallVelocity)
        {
            velocity.y = maxFallVelocity;
            mRigidbody.velocity = velocity;
        }
    }

    public virtual void Walk(float horizontalSpeedInput)
    {
        Vector2 velocity = mRigidbody.velocity;
        velocity.x = this.horizontalSpeed * Mathf.Clamp(horizontalSpeedInput, -1, 1);
        velocity.x = Mathf.Clamp(velocity.x, -this.horizontalSpeed, this.horizontalSpeed);
        mRigidbody.velocity = velocity;
        if (IsTouchingGround)
            coyoteTime = Time.time + maxCoyeteTime;
    }

    public virtual void Jump()
    {
        if (!IsJumping && (IsTouchingGround || coyoteTime > Time.time) )
        {
            float gravityScale = mRigidbody.gravityScale;
            Vector2 velocity = mRigidbody.velocity;
            velocity.y = 0;
            mRigidbody.velocity = velocity;
            IsJumping = true;
            jumpTime = 0;
            IsTouchingGround = false;
            mRigidbody.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            IsJumping = true;
            jumpTime = 0;
            IsTouchingGround = false;
        }
    }

    public void JumpRelease() 
    {
        IsJumping = false;
    }

    public virtual void Link()
    {
        PlayerController.IsSeparate = false;
    }

    public bool IsInsideLinkDistance() 
    {
        PlayerController other = GetOtherPart();
        if (other == null)
            return false;
        Vector3 distanceVector = other.transform.position - transform.position;
        return distanceVector.sqrMagnitude <= LinkDistanceSqrMagnitude;
    }

    private void HoldJump()
    {
        if (jumpTime < maxJumpTime)
        {
            if (IsJumping) 
            {
               
                mRigidbody.AddForce( jumpHoldForce * Vector2.up);
                jumpTime += Time.deltaTime;
            }
        }
        else 
        {
            IsJumping = false;
        }
    }


}
