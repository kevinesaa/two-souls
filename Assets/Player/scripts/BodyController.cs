using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : PlayerController
{

    private SoulController soul;

    protected override void Awake()
    {
        base.Awake();
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Link()
    {
        base.Link();
        soul.transform.position = transform.position;
    }

    public override PlayerController GetOtherPart()
    {
        return soul;
    }

    public void SetSoul(SoulController soul)
    {
        this.soul = soul;
    }
}
