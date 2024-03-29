﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulController : PlayerController
{
    [Header("Throw angle")]
    public float defaultLookGrade = -90;
    public float maxDegreRotatePerFrame = 5;

    [Header("Before throw moment")]
    //before throw moment
    public float maxForceSoulThrow = 10;
    public float aditiveForcePerSecond = 0.1f;
    public float throwForce;

    [Header("up throw moment")]
    //up moment
    public float initialThrowingTime = 0.09f;
    public float airTimeBeforeGravity = 1;
    float throwingTime;

    [Header("down throw moment")]
    //down moment
    public float aditiveSoulGravityScalePerSecond = 0.2f;
    public float initialSoulGravityScale;
    bool diffGravity;
    
    bool isMaxThrowingAirTime;
    bool isThrowing = false;
   
    private BodyController body;

    protected override void Awake()
    {
        base.Awake();
        initialSoulGravityScale = mRigidbody.gravityScale;        
    }

    protected override void Update()
    {
        if (!IsSeparate) 
        {
            return;
        }
        base.Update();
        if (isThrowing) 
        {
            throwingTime += Time.deltaTime;
            isMaxThrowingAirTime = throwingTime >= airTimeBeforeGravity;
            if (throwingTime < initialThrowingTime) 
            {
                return;
            }
        }
        
        
        diffGravity = (mRigidbody.gravityScale != initialSoulGravityScale);
        if (IsTouchingGround)
        {
            isThrowing = false;
            if (diffGravity) 
            {
                mRigidbody.gravityScale = initialSoulGravityScale;
            }
        }
        else
        {
            //down moment
            if (isThrowing && isMaxThrowingAirTime && diffGravity) 
            {
                mRigidbody.gravityScale += aditiveSoulGravityScalePerSecond * Time.deltaTime;
                mRigidbody.gravityScale = Mathf.Clamp(mRigidbody.gravityScale, 0, initialSoulGravityScale);
            }
        }
    }

    public override void Walk(float horizontalSpeedInput)
    {
        if (!isThrowing)
        {
            base.Walk(horizontalSpeedInput);
        }
    }

    public override void Jump()
    {
        if (!isThrowing)
        {
            base.Jump();
        }
    }

    public override PlayerController GetOtherPart()
    {
        return body;
    }

    public void SetBody(BodyController body)
    {
        this.body = body;
    }

    public void ThrowSoul(Vector2 direction)
    {        
        direction.Normalize();
        if ((transform.right.x * direction.x) < 0)
        {
            transform.Rotate(0, 180, 0);
        }
        mRigidbody.gravityScale = 0;
        throwingTime = 0;
        PlayerController.IsSeparate = true;
        isThrowing = true;
        mRigidbody.AddForce(throwForce * direction , ForceMode2D.Impulse);
        ResetThrowForece();
    }

    public void IncrementThrowForce()
    {
        throwForce += aditiveForcePerSecond * Time.deltaTime;
        throwForce = Mathf.Clamp(throwForce, 0, maxForceSoulThrow);
    }

    public void ResetThrowForece()
    {
        throwForce = 0;
    }

    public override void Link()
    {
        base.Link();
        body.transform.position = transform.position;
    }

    protected override void HoldJump()
    {
        if (!isThrowing)
        {
            base.HoldJump();
        }
    }


}
