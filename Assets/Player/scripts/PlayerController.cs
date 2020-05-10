using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public abstract class PlayerController : MonoBehaviour
{
    [Header("jump params")]
    public float initialJumpSpeed = 15f;
    public float holdJumpSpeed = 5f;
    public float maxJumpDistance = 3f;
    public float maxJumpTime = 0.6f;
    public float maxCoyeteTime = 0.05f;
    public float groundDistance = 0.1f;
    public float fallingMultiply = 2f;
    public float maxFallVelocity = 30;
    
    public LayerMask layerGround;
    public Transform transfromGroundLeftCheck;
    public Transform transfromGroundRightCheck;

    [Header("walk params")]
    public float horizontalSpeed = 2;
    public float linkDistance=6;

   
    public static bool IsSeparate { get; protected set; }
    
    public float LinkDistanceSqrMagnitude { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsTouchingGround { get; private set; }

    protected Rigidbody2D mRigidbody;
    private float coyoteTime;
    private float jumpPosition;
    private float jumpTime;
    private float initialGravityScale;

    public abstract PlayerController GetOtherPart();

    protected virtual void Awake()
    {
        groundDistance = Mathf.Abs(groundDistance);
        maxFallVelocity = (-1) * Mathf.Abs(maxFallVelocity);
        mRigidbody = GetComponent<Rigidbody2D>();
        initialGravityScale = mRigidbody.gravityScale;
        LinkDistanceSqrMagnitude = linkDistance * linkDistance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RaycastHit2D groundLeftCheckHit = Physics2D.Raycast(transfromGroundLeftCheck.position, Vector2.down, groundDistance, layerGround);
        RaycastHit2D groundRightCheckHit = Physics2D.Raycast(transfromGroundRightCheck.position, Vector2.down, groundDistance, layerGround);
        IsTouchingGround = (groundLeftCheckHit || groundRightCheckHit);

        if (IsTouchingGround)
            coyoteTime = Time.time + maxCoyeteTime;

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
    }

    public virtual void Jump()
    {
        if (!IsJumping && (IsTouchingGround || coyoteTime > Time.time) )
        {
            mRigidbody.gravityScale = 0;
            IsJumping = true;
            IsTouchingGround = false;
            jumpPosition = transform.localPosition.y + maxJumpDistance;
            mRigidbody.velocity = mRigidbody.velocity.x * Vector2.right;
            mRigidbody.velocity =  (initialJumpSpeed * Vector2.up);
            jumpTime = Time.time + Time.deltaTime + maxJumpTime;
            mRigidbody.gravityScale = 0;
            IsJumping = true;
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

    protected virtual void HoldJump()
    {
        mRigidbody.gravityScale = initialGravityScale;
        if ( mRigidbody.velocity.y > 0 &&  ((transform.position.y <= jumpPosition ) || (Time.time  <= jumpTime)))
        {
            if (IsJumping)
            {
                mRigidbody.gravityScale = initialGravityScale * (fallingMultiply / 2);
                mRigidbody.velocity =  ( (holdJumpSpeed )* Vector2.up);

            }
        }
        else 
        {
            IsJumping = false;
            if(!IsTouchingGround)
                mRigidbody.gravityScale = initialGravityScale * (fallingMultiply);
        }
    }


}
