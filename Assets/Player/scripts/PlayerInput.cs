using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public Slider sliderMaxThrowForce;
    public Text textMaxThrowForce;
    
    public Slider sliderFinalThrowForce;
    public Text texFinalThrowForce;

    public Slider sliderGravityAditive;
    public Text textGravityAditive;

    public Slider sliderAirTimeAditive;
    public Text textAirTimeAditive;

    public BodyController body;
    public SoulController soul;
    public GameObject bodyPivot;

    private AllPlayer player;
    private float horizontalInput;

    private void initEditorControllers() {

        sliderFinalThrowForce.minValue = 0;
        sliderFinalThrowForce.value = 0;
        sliderFinalThrowForce.maxValue = soul.maxForceSoulThrow;
        //charging
        sliderMaxThrowForce.minValue = 0;
        sliderMaxThrowForce.maxValue = soul.maxForceSoulThrow;
        sliderMaxThrowForce.value = soul.aditiveForcePerSecond;
        
        //down
        sliderGravityAditive.minValue = 0;
        sliderGravityAditive.maxValue = soul.initialSoulGravityScale;
        sliderGravityAditive.value = soul.aditiveSoulGravityScalePerSecond;

        //up
        sliderAirTimeAditive.minValue = 0;
        sliderAirTimeAditive.maxValue = soul.airTimeBeforeGravity;
        sliderAirTimeAditive.value = soul.airTimeBeforeGravity;
    }

    private void allocateFromEditor() {
        //charging
        soul.aditiveForcePerSecond = sliderMaxThrowForce.value;
        textMaxThrowForce.text = "" + sliderMaxThrowForce.value;

        //down
        soul.aditiveSoulGravityScalePerSecond = sliderGravityAditive.value;
        textGravityAditive.text = "" + sliderGravityAditive.value;

        //up
        soul.airTimeBeforeGravity = sliderAirTimeAditive.value;
        textAirTimeAditive.text =""+ sliderAirTimeAditive.value;
    }

    LineRenderer line;
    private void DrawDistance() 
    {
        if (line == null) 
        {
            line = GetComponent<LineRenderer>();
            line.SetPositions(new Vector3[] {soul.transform.position,body.transform.position });
        }
        line.enabled = false;
        if (player.IsSeparate && soul.IsInsideLinkDistance()) 
        {

            line.enabled = true;
            line.SetPosition(0, soul.transform.position + (-2 * Vector3.forward));
            line.SetPosition(1, body.transform.position+ (-2 * Vector3.forward) );
        }
    }

    private void Awake()
    {
        CameraController.INSTANCE.Target = body.transform;
        player = new AllPlayer(body, soul, bodyPivot);    
        soul.gameObject.SetActive(false);
        bodyPivot.SetActive(false);
        
        //remove
        initEditorControllers();
    }

    // Update is called once per frame
    void Update()
    {
        //remove
        allocateFromEditor();
        DrawDistance();

        horizontalInput = Input.GetAxis("Horizontal");
        player.Move(horizontalInput);
        
        if (Input.GetButtonDown("Jump")) 
        {
            player.Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.JumpRelease();
        }

        if (player.IsSeparate)
        {
            player.HideThrowPoint();

            if (Input.GetButtonDown("ChangePlayer")) 
            {
                CameraController.INSTANCE.Target = player.ChangeCharacter().transform;
            }

            //union
            if (Input.GetButtonDown("Fire2"))
            {
                player.Link();
                CameraController.INSTANCE.Target = body.transform;
            }
        }
        else
        {
            //intenta separarse
            if (Input.GetButton("Fire2"))
            {
                //remove
                sliderFinalThrowForce.value = soul.throwForce;
                texFinalThrowForce.text =""+ soul.throwForce;
                player.IncrementThrowForce();
                texFinalThrowForce.text = "" + soul.throwForce;
                //remove
                sliderFinalThrowForce.value = soul.throwForce;


                player.CalculateScreenPositionForThrowSoul(Camera.main);
                player.ShowThrowDirection(InputPositionForThrowSoul());
                
                if (Input.GetButtonDown("Fire1"))
                {
                    CameraController.INSTANCE.Target = soul.transform;
                    player.ThrowSoul(InputPositionForThrowSoul());
                    player.HideThrowPoint();
                }
            }

            if (Input.GetButtonUp("Fire2"))
            {
                player.ResetThrowForece();
                player.HideThrowPoint();
            }
        }

        
    }

    private Vector3 InputPositionForThrowSoul() 
    {
        return Input.mousePosition;
    }
}
