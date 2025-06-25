using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beebyte.Obfuscator;
[Skip]
public class SuperWheelColider : MonoBehaviour {
    private readonly string audioName = "circleHit";
    private JointAngleLimits2D _jointAngleLimits2D = new JointAngleLimits2D();
	

    private void Awake()
    {
        if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() && SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            _jointAngleLimits2D.min=-10f;
            _jointAngleLimits2D.max=80f;
            HingeJoint2D jointAngleLimits2D = transform.GetComponent<HingeJoint2D>();
            if (jointAngleLimits2D!=null)
            {
                jointAngleLimits2D.useLimits = true;
                jointAngleLimits2D.limits = _jointAngleLimits2D;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll) 
    {
        Libs.AudioEntity.Instance.PlayEffect (audioName);
    }
}