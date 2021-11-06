using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Fusion;
using UnityEngine.InputSystem;

[RequireComponent( typeof( HighlightCollector ) )]
[RequireComponent( typeof( VelocityBuffer ) )]
public class Hand : NetworkBehaviour
{
    public enum HandSide{left, right}
    
    HighlightCollector m_HighlightCollector;
    Highlightable m_ActiveHighlight;
    public bool m_Grabbing;
    public bool preventGrabbing = false;
    
    
    [SerializeField] private InputActionReference _gripInputAction;
    [SerializeField] private InputActionReference _triggerInputAction;
    [SerializeField] private InputActionReference _thumbInputAction;
    public HandSide handSide = HandSide.left;
    public Animator handAnimator;
    public Transform Visuals;
    public Transform AttachPoint;
    public VelocityBuffer VelocityBuffer { get; private set; }

    TeleportHandler m_TeleportHandler;
    
    [Networked]
    private InputAction PreviousInputAction { get; set; }
    private void Awake()
    {
        
        
        VelocityBuffer = GetComponent<VelocityBuffer>();
        m_HighlightCollector = GetComponent<HighlightCollector>();
        m_TeleportHandler = GetComponentInChildren<TeleportHandler>();
        _gripInputAction.action.performed += GripPressed;
        _triggerInputAction.action.performed += TriggerPressed;
        _thumbInputAction.action.started += ThumbDown;
        _thumbInputAction.action.canceled += ThumbUp;
    }

    private void ThumbUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
            handAnimator.SetBool("ThumbDown", false);
       
        
    }

    private void ThumbDown(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
            handAnimator.SetBool("ThumbDown", true);
       
        
    }

    private void TriggerPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
            handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
        
}

    private void GripPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
            handAnimator.SetFloat("Grip", obj.ReadValue<float>());
        
       
    }

    public void UpdateInput( InputDataController input )
    {
        PreviousInputAction = input.PreprocessActions( PreviousInputAction );
        UpdatePose( input.LocalPosition, input.LocalRotation );
        
        if( input.GetAction( InputAction.GRAB ) && !preventGrabbing )
        {
            Grab();
        }
        if( input.GetAction( InputAction.DROP ) )
        {
            Drop();
        }
        
        

        m_TeleportHandler?.UpdateInput( input );

        //Visuals.localScale = m_Grabbing ? Vector3.one * 0.8f : Vector3.one;
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

    public void GrabCardFromStack(GameObject card)
    {
        Debug.Log("GrabCardFromStack called");
        m_ActiveHighlight = card.GetComponent<Highlightable>();
        m_ActiveHighlight.Grab(this);
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
    private void OnDisable()
    {
        _gripInputAction.action.performed -= GripPressed;
        _triggerInputAction.action.performed -= TriggerPressed;
        _thumbInputAction.action.started -= ThumbDown;
        _thumbInputAction.action.canceled -= ThumbUp;
    }


}
