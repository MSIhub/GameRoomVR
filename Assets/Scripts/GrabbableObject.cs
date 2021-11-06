using System;
using System.Collections;
using cardGame;
using UnityEngine;
using Fusion;

[RequireComponent( typeof( Highlightable ) )]
public class GrabbableObject : NetworkBehaviour
{
    [Networked] public Hand m_HoldingHand { get; set; }
    [Networked] Vector3 m_PositionOffset { get; set; }
    [Networked] Quaternion m_RotationOffset { get; set; }
    
    [Networked] Vector3 m_PositionOffset_right { get; set; }
    [Networked] Quaternion m_RotationOffset_right { get; set; }

    Rigidbody m_Body;
    public float ThrowForce = 2f;

    public Highlightable Highlight { get; private set; }

    public bool KeepRotationOffsetOnGrab = true;
    public bool KeepPositionOffsetOnGrab = true;

    private bool parentToHand = false;
    private bool unparentFromHand = false;

    private Vector3 _originalAttachPointPosition;
    private Quaternion _originalAttachPointRotation;
    
    public Transform originalParent;
    private bool _grabPointExist = false;

    private gameManagerLocal localGameManager;
    
    //TODO: card variables. These should reside in a specific card grab class which inherits from this class
    
    private bool objectIsCard = false;
    [SerializeField] private GameObject stackPrefab;
    public bool toBeStacked = false;
    public bool lastCardInStack;

    private void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Body.maxAngularVelocity = Mathf.Infinity;

        Highlight = GetComponent<Highlightable>();
        Highlight.GrabCallback += OnGrab;
        Highlight.DropCallback += OnDrop;

        localGameManager = FindObjectOfType<gameManagerLocal>();
    }

    private void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (parentToHand)
        {
            parentToHand = false;
            originalParent = transform.parent;
            if (objectIsCard)
            {
                handleCardGrab();
            }
            else
            {
                transform.SetParent(m_HoldingHand.AttachPoint);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            

        }
        
        if (unparentFromHand)
        {
            unparentFromHand = false;
            //reparent the object to the hand
            if (objectIsCard)
            {
                if (!lastCardInStack && !toBeStacked)
                {
                    transform.SetParent(originalParent);
                    SetLayerMaskIncludingChildren("Default");
                }
                if (lastCardInStack)
                {
                    //this is the last card, so we need to destroy the stack
                    localGameManager.Stack.GetComponent<CardStack>().destroyStack();
                    lastCardInStack = false;
                    localGameManager.stackSpawned = false;
                    transform.SetParent(originalParent);
                    foreach (var currentCollider in gameObject.GetComponentsInChildren<Collider>(true))
                    {
                        currentCollider.gameObject.tag = "card";
                    }
                    SetLayerMaskIncludingChildren("Default");
                    
                }
                
                //if the current card drop is a drop in the stack
                if (toBeStacked)
                {
                    Debug.Log("card to be added to stack > " + name);
                    //add card to stack at ghost position
                    foreach (var currentCollider in gameObject.GetComponentsInChildren<Collider>(true))
                    {
                        currentCollider.gameObject.tag = "stackedCard";
                    }
                    
                    transform.SetParent(localGameManager.Stack.transform);
                    transform.localPosition = localGameManager.Stack.GetComponent<CardStack>().ghostPosition;
                    transform.localRotation = localGameManager.Stack.GetComponent<CardStack>().ghostRotation;
                    localGameManager.Stack.GetComponent<CardStack>().addCardToStack(gameObject);
                    toBeStacked = false;
                }

                
                
            } else
            {
                transform.SetParent(originalParent);
                SetLayerMaskIncludingChildren("Default");
            }

            
        }
        
        /*
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

        */

    }

    private void handleCardGrab()
    {
        if (localGameManager.stackSpawned)
        {
            transform.SetParent(m_HoldingHand.AttachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            lastCardInStack = true;
            foreach (var currentCollider in gameObject.GetComponentsInChildren<Collider>(true))
            {
                currentCollider.gameObject.tag = "stackedCard";
            }
            localGameManager.Stack = Instantiate(stackPrefab,m_HoldingHand.AttachPoint);
            localGameManager.Stack.GetComponent<CardStack>().addCardToStack(gameObject);
            transform.SetParent(localGameManager.Stack.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            localGameManager.stackSpawned = true;
            //deactivate fingertip cardSelector
            foreach (var currentCardSelector in m_HoldingHand.transform.parent.GetComponentsInChildren<stackedCardSelector>(true))
            {
                if(currentCardSelector.selectorHandSide == m_HoldingHand.handSide) currentCardSelector.gameObject.SetActive(false);
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
       // Debug.Log("grabba " + transform.name);
        SetLayerMaskIncludingChildren("grabbed");
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        
        parentToHand = true;    
        if( m_HoldingHand != null )
        {
            m_HoldingHand.Drop();
        }
        m_HoldingHand = other;
        if (gameObject.CompareTag("card"))
        {
            m_HoldingHand.handAnimator.SetBool("cardInHand", true);
            objectIsCard = true;
            
        }
        if (gameObject.CompareTag("Striker"))
        {
            m_HoldingHand.handAnimator.SetBool("strikerInHand", true);
        }
        if (gameObject.CompareTag("Chalk"))
        {
            m_HoldingHand.handAnimator.SetBool("penInHand", true);
        }
        //save attachpoint
        _originalAttachPointPosition = m_HoldingHand.AttachPoint.localPosition;
        _originalAttachPointRotation = m_HoldingHand.AttachPoint.localRotation;
        
        //add offset from object
        if (m_HoldingHand.handSide == Hand.HandSide.left)
        {
            m_HoldingHand.AttachPoint.localPosition = m_PositionOffset;
            m_HoldingHand.AttachPoint.localRotation = m_RotationOffset;
        }
        else
        {
            m_HoldingHand.AttachPoint.localPosition = m_PositionOffset_right;
            m_HoldingHand.AttachPoint.localRotation = m_RotationOffset_right;
        }
        

    }

    public void SetLayerMaskIncludingChildren(string MaskName)
    {
        foreach (var tf in gameObject.GetComponentsInChildren<Transform>())
        {
            tf.gameObject.layer = LayerMask.NameToLayer(MaskName);
        }
    }

    public void OnDrop()
    {
      //  Debug.Log("droppa " + transform.name);
        //reparent the object to the original parent
        unparentFromHand = true;
        

        //remove object specific object
        //add offset from object
        m_HoldingHand.AttachPoint.localPosition = _originalAttachPointPosition;
        m_HoldingHand.AttachPoint.localRotation = _originalAttachPointRotation;
        //reactivate finger tip selector
        if (gameObject.CompareTag("card"))
        {
            m_HoldingHand.handAnimator.SetBool("cardInHand", false);
        }
        if (gameObject.CompareTag("Striker"))
        {
            m_HoldingHand.handAnimator.SetBool("strikerInHand", false);
        }
        if (gameObject.CompareTag("Chalk"))
        {
            m_HoldingHand.handAnimator.SetBool("penInHand", false);
        }

        if (!toBeStacked)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;

            if( m_HoldingHand != null && m_HoldingHand.VelocityBuffer != null )
            {
                m_Body.velocity = m_HoldingHand.VelocityBuffer.GetAverageVelocity() * ThrowForce;
            }
            else
            {
                m_Body.velocity = m_Body.velocity * ThrowForce;
            }
        }

        if (lastCardInStack)
        {
            m_HoldingHand.transform.parent.GetComponentInChildren<stackedCardSelector>(true).gameObject.SetActive(true);
        }
        
        
        m_HoldingHand = null;
    }

    public void ResetCard()
    {
        SetLayerMaskIncludingChildren("Default");
        m_HoldingHand = null;
        toBeStacked = false;
        GetComponentInChildren<Collider>().tag = "card";
    }


}
