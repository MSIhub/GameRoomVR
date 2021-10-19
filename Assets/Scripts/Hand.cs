using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Fusion;

[RequireComponent( typeof( HighlightCollector ) )]
[RequireComponent( typeof( VelocityBuffer ) )]
public class Hand : NetworkBehaviour
{
    HighlightCollector m_HighlightCollector;
    Highlightable m_ActiveHighlight;
    bool m_Grabbing;

    public Transform Visuals;
    public VelocityBuffer VelocityBuffer { get; private set; }

    TeleportHandler m_TeleportHandler;

    [Networked]
    private InputAction PreviousInputAction { get; set; }

    private void Awake()
    {
        VelocityBuffer = GetComponent<VelocityBuffer>();
        m_HighlightCollector = GetComponent<HighlightCollector>();
        m_TeleportHandler = GetComponentInChildren<TeleportHandler>();
    }

    public void UpdateInput( InputDataController input )
    {
        PreviousInputAction = input.PreprocessActions( PreviousInputAction );
        UpdatePose( input.LocalPosition, input.LocalRotation );

        if( input.GetAction( InputAction.GRAB ) )
        {
            Grab();
        }
        if( input.GetAction( InputAction.DROP ) )
        {
            Drop();
        }

        m_TeleportHandler?.UpdateInput( input );

        Visuals.localScale = m_Grabbing ? Vector3.one * 0.8f : Vector3.one;
    }

    void UpdatePose( Vector3 localPosition, Quaternion localRotation )
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;

        Runner.AddPlayerAreaOfInterest( Runner.LocalPlayer, transform.position, 1f );

    }
    public void UpdateLocalPose( Vector3 localPosition, Quaternion localRotation )
    {
        Visuals.position = transform.parent.TransformPoint( localPosition );
        Visuals.rotation = transform.parent.rotation * localRotation;
    }

    void Grab()
    {
        Drop();

        if( m_HighlightCollector.CurrentHighlight != null )
        {
            m_ActiveHighlight = m_HighlightCollector.CurrentHighlight;
            m_ActiveHighlight.Grab( this );
        }
        else
        {
            m_ActiveHighlight = null;
        }

        m_Grabbing = true;
    }

    public void Drop()
    {
        if( m_ActiveHighlight != null )
        {
            m_ActiveHighlight.Drop();
            m_ActiveHighlight = null;
        }

        m_Grabbing = false;
    }


}
