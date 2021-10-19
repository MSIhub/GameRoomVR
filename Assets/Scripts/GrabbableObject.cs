using System.Collections;
using UnityEngine;
using Fusion;

[RequireComponent( typeof( Highlightable ) )]
public class GrabbableObject : NetworkBehaviour
{
    [Networked] Hand m_HoldingHand { get; set; }
    [Networked] Vector3 m_PositionOffset { get; set; }
    [Networked] Quaternion m_RotationOffset { get; set; }

    Rigidbody m_Body;
    public float ThrowForce = 2f;

    public Highlightable Highlight { get; private set; }

    public bool KeepRotationOffsetOnGrab = true;
    public bool KeepPositionOffsetOnGrab = true;


    private void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Body.maxAngularVelocity = Mathf.Infinity;

        Highlight = GetComponent<Highlightable>();
        Highlight.GrabCallback += OnGrab;
        Highlight.DropCallback += OnDrop;
    }

    private void OnDestroy()
    {
        if( Highlight != null )
        {
            Highlight.GrabCallback -= OnGrab;
            Highlight.DropCallback -= OnDrop;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if( m_HoldingHand != null )
        {
            Vector3 targetPosition = m_HoldingHand.transform.position + m_HoldingHand.transform.TransformDirection( m_PositionOffset );
            m_Body.velocity = ( targetPosition - transform.position ) / Runner.DeltaTime;


            Quaternion targetRotation = m_HoldingHand.transform.rotation * m_RotationOffset;
            Quaternion rotationDelta = targetRotation * Quaternion.Inverse( m_Body.rotation );
            rotationDelta.ToAngleAxis( out var angleInDegrees, out var rotationAxis );
            if( angleInDegrees > 180f )
                angleInDegrees -= 360f;

            var angularVelocity = ( rotationAxis * angleInDegrees * Mathf.Deg2Rad ) / Runner.DeltaTime;
            if( float.IsNaN( angularVelocity.x ) == false )
            {
                m_Body.angularVelocity = angularVelocity;
            }

        }

    }

    void OnGrab( Hand other )
    {
        if( m_HoldingHand != null )
        {
            m_HoldingHand.Drop();
        }
        m_HoldingHand = other;

        if( KeepRotationOffsetOnGrab )
        {
            m_RotationOffset = Quaternion.Inverse( m_HoldingHand.transform.rotation ) * transform.rotation;
        }
        else
        {
            m_RotationOffset = Quaternion.identity;
        }

        if( KeepPositionOffsetOnGrab )
        {
            m_PositionOffset = m_HoldingHand.transform.InverseTransformDirection( transform.position - m_HoldingHand.transform.position );
        }
        else
        {
            m_PositionOffset = Vector3.zero;
        }
    }

    void OnDrop()
    {
        if( m_HoldingHand != null && m_HoldingHand.VelocityBuffer != null )
        {
            m_Body.velocity = m_HoldingHand.VelocityBuffer.GetAverageVelocity() * ThrowForce;
        }
        else
        {
            m_Body.velocity = m_Body.velocity * ThrowForce;
        }

        m_HoldingHand = null;
    }


}
