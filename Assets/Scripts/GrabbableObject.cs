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

    private bool parentToHand = false;
    private bool unparentFromHand = false;
    
    private Transform originalParent;

    private void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Body.maxAngularVelocity = Mathf.Infinity;

        Highlight = GetComponent<Highlightable>();
        Highlight.GrabCallback += OnGrab;
        Highlight.DropCallback += OnDrop;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (parentToHand)
        {
            parentToHand = false;
            //reparent the object to the hand
            originalParent = transform.parent;
            transform.SetParent(m_HoldingHand.AttachPoint);
        }
        
        if (unparentFromHand)
        {
            unparentFromHand = false;
            //reparent the object to the hand
             transform.SetParent(originalParent);
        }
        
        if( m_HoldingHand != null )
        {
            Vector3 targetPosition = m_HoldingHand.AttachPoint.position;
            m_Body.velocity = ( targetPosition - transform.position ) / Runner.DeltaTime;


            Quaternion targetRotation = m_HoldingHand.AttachPoint.transform.rotation;
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

    private void OnDestroy()
    {
        if( Highlight != null )
        {
            Highlight.GrabCallback -= OnGrab;
            Highlight.DropCallback -= OnDrop;
        }
    }


    void OnGrab( Hand other )
    {
        Debug.Log("grabba " + transform.name);
        SetLayerMaskIncludingChildren("grabbed");
        parentToHand = true;    
        if( m_HoldingHand != null )
        {
            m_HoldingHand.Drop();
        }
        m_HoldingHand = other;
        
        

    }

    private void SetLayerMaskIncludingChildren(string MaskName)
    {
        foreach (var tf in gameObject.GetComponentsInChildren<Transform>())
        {
            tf.gameObject.layer = LayerMask.NameToLayer(MaskName);
        }
    }

    void OnDrop()
    {
        Debug.Log("droppa " + transform.name);
        //reparent the object to the original parent
        unparentFromHand = true;
        SetLayerMaskIncludingChildren("Default");
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
