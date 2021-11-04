using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[OrderAfter(typeof(NetworkRigidbody))]
public class Player : NetworkBehaviour
{
    public bool OverrideLocalPosition = true;
    public Transform Head;
    public GameObject HeadVisuals;
    public Hand LeftHand;
    public Hand RightHand;

    public PlayerInputHandler LocalInput;
    public Renderer[] Renderers;
    public Material LocalHandMaterial;

    public override void Spawned()
    {
        if( Object.HasInputAuthority )
        {
            HeadVisuals.SetActive( false );

            foreach( var rend in Renderers )
            {
                rend.sharedMaterial = LocalHandMaterial;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if( GetInput<InputData>( out var input ) )
        {
            LeftHand.UpdateInput( input.Left );
            RightHand.UpdateInput( input.Right );
            Head.localPosition = input.HeadLocalPosition;
            Head.localRotation = input.HeadLocalRotation;
            
            Runner.AddPlayerAreaOfInterest( Runner.LocalPlayer, Head.position, 1f );
        }
    }

    public override void Render()
    {
        if( LocalInput != null && OverrideLocalPosition )
        {
            if( LocalInput.LeftController != null )
            {
                LeftHand.UpdateLocalPose( LocalInput.LeftController.GetLocalPosition(), LocalInput.LeftController.GetLocalRotation() );
            }
            if( LocalInput.RightController != null )
            {
                RightHand.UpdateLocalPose( LocalInput.RightController.GetLocalPosition(), LocalInput.RightController.GetLocalRotation() );
            }
        }
    }
}
