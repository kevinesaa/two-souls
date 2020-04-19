using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlayer 
{    
    public const int BODY_INDEX = 0;
    public const int SOUL_INDEX = 1;

    public bool IsSeparate { get { return PlayerController.IsSeparate; } }
    public Vector3 ScreenPositionForThrowSoul { get { return screenPositionForThrowSoul; } }

    private int currentIndex;
    private Vector3 screenPositionForThrowSoul;
    private readonly PlayerController[] playerParts;
    private readonly BodyController body;
    private readonly SoulController soul;
    private readonly GameObject bodyPivot;
    
    public AllPlayer(BodyController body, SoulController soul, GameObject bodyPivot) 
    {
        body.SetSoul(soul);
        soul.SetBody(body);
        this.body = body;
        this.soul = soul;
        this.bodyPivot = bodyPivot;
        playerParts = new PlayerController[] { body, soul };
    }

    public void Move(float horizontalInput) 
    {
        if (IsSeparate) 
        {
            Flip(playerParts[currentIndex], horizontalInput);
            playerParts[currentIndex].Walk(horizontalInput);
        }
        else 
        {
            Flip(body, horizontalInput);
            body.Walk(horizontalInput);
        }
    }

    public PlayerController ChangeCharacter() 
    {
        if (IsSeparate) 
        {
            currentIndex++;
            if (currentIndex >= playerParts.Length)
            {
                currentIndex = 0;
            }
        }
        return playerParts[currentIndex];
    }

    public void Jump()
    {
        playerParts[currentIndex].Jump();
    }

    public void JumpRelease()
    {
        playerParts[currentIndex].JumpRelease();
    }

    public void Link() 
    {
        if (IsSeparate && playerParts[currentIndex].IsInsideLinkDistance())
        {
            playerParts[currentIndex].Link();
            soul.gameObject.SetActive(false);
            body.gameObject.SetActive(true);
            currentIndex = BODY_INDEX;
        }
    }

    public void CalculateScreenPositionForThrowSoul(Camera camera)
    {
        screenPositionForThrowSoul = camera.WorldToScreenPoint(this.bodyPivot.transform.position);
    }

    public void IncrementThrowForce()
    {
        if (!IsSeparate)
        {
            soul.IncrementThrowForce();
        }
    }

    public void ShowThrowDirection(Vector3 throwPositionInput)
    {
        if (!IsSeparate)
        {
            bodyPivot.SetActive(true);
            float currentAngle = this.bodyPivot.transform.eulerAngles.z;
            throwPositionInput = throwPositionInput - screenPositionForThrowSoul;
            float rotate = Mathf.Atan2(throwPositionInput.y, throwPositionInput.x) * Mathf.Rad2Deg;
            rotate = (rotate + soul.defaultLookGrade);
            rotate = Mathf.MoveTowardsAngle(currentAngle, rotate, soul.maxDegreRotatePerFrame);
            this.bodyPivot.transform.eulerAngles = rotate * Vector3.forward;
        }
        
    }

    public void HideThrowPoint()
    {
        if (bodyPivot.activeSelf)
        {
            bodyPivot.SetActive(false);
        }
        
    }

    public void ResetThrowForece()
    {
        soul.ResetThrowForece();
    }

    public void ThrowSoul(Vector3 inputPosition)
    {
        if (!IsSeparate)
        {
            currentIndex = SOUL_INDEX;
            soul.transform.position = body.transform.position;
            soul.gameObject.SetActive(true);
            soul.ThrowSoul(inputPosition - screenPositionForThrowSoul);
        }
    }

    private void Flip(PlayerController playerController, float horizontalInput)
    {
        if ((playerController.transform.right.x * horizontalInput) < 0) 
        {
            playerController.transform.Rotate(0, 180, 0);
        }
    }
}
