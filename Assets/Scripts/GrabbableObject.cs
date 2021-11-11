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
    private bool stackedCard = false;
    public Transform SelectionPoint;
    private bool throwCooldownTimerRunning = false;
    private float throwCooldownTimerTime = 0.5f;
    private float throwCooldownTimer = 0f;
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
        if (throwCooldownTimerRunning)
        {
            throwCooldownTimer += Runner.DeltaTime;
            if (throwCooldownTimer >= throwCooldownTimerTime)
            {
                throwCooldownTimerRunning = false;
                Debug.Log("Delayed Layer Reset before = " + gameObject.layer.ToString());
                SetLayerMaskIncludingChildren("Default");
                Debug.Log("Delayed Layer Reset after = " + gameObject.layer.ToString());
            }
        }
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
                if (stackedCard)
                {
                    if (localGameManager.stackSpawned)
                    {
                        localGameManager.Stack.GetComponent<CardStack>().destroyStack();
                    }
                    
                }
                if (!toBeStacked)
                {
                    transform.SetParent(originalParent);
                    ResetCard(false);
                    throwCooldownTimerRunning = true;
                }
              
               //if the current card drop is a drop in the stack
                if (toBeStacked && !stackedCard)
                {
                    Debug.Log("card to be added to stack > " + name);
                    //add card to stack at ghost position
                    foreach (var currentCollider in gameObject.GetComponentsInChildren<Collider>(true))
                    {
                        currentCollider.gameObject.tag = "stackedCard";
                    }
                    //as the hand must be in the trigger on dropping the card, trigger it
                    //get collider
                    /*Collider FingerTip = null;
                    foreach (var tip in m_HoldingHand.transform.parent.GetComponentsInChildren<stackedCardSelector>())
                    {
                        if (m_HoldingHand.handSide == tip.selectorHandSide) FingerTip = tip.transform.GetComponent<Collider>();
                    }
                    localGameManager.Stack.GetComponent<CardStack>().handleFingerTipEnteredStack(FingerTip);
                    */
                    //set hands and grabbed state
                    m_HoldingHand = localGameManager.Stack.GetComponent<CardStack>().stackHoldingHand;
                    transform.SetParent(localGameManager.Stack.transform);
                    transform.localPosition = localGameManager.Stack.GetComponent<CardStack>().ghostPosition;
                    transform.localRotation = localGameManager.Stack.GetComponent<CardStack>().ghostRotation;
                    localGameManager.Stack.GetComponent<CardStack>().addCardToStack(gameObject);
                    toBeStacked = false;
                    stackedCard = true;
                }


                
                
            } else
            {
                transform.SetParent(originalParent);
                throwCooldownTimerRunning = true;
                //remove object specific object
                //add offset from object
                resetHand();
        
        

                

        
        
                
            }

            if (!toBeStacked)
            {
                Debug.Log("object <"+ gameObject.name +"> is not to be stacked, turn physics on");
                //copy attach point position
                //m_HoldingHand.AttachPoint.localPosition = _originalAttachPointPosition;
                //m_HoldingHand.AttachPoint.localRotation = _originalAttachPointRotation;
                transform.Translate(m_HoldingHand.AttachPoint.localPosition);
                transform.Rotate(m_HoldingHand.AttachPoint.localRotation.eulerAngles);
                    
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
            m_HoldingHand = null;
        }
        
        if (!objectIsCard)
        {
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
            foreach (var currentCollider in gameObject.GetComponentsInChildren<Collider>(true))
            {
                currentCollider.gameObject.tag = "stackedCard";
            }
            localGameManager.Stack = Instantiate(stackPrefab,m_HoldingHand.AttachPoint);
            localGameManager.Stack.GetComponent<CardStack>().stackHoldingHand = m_HoldingHand;
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

            stackedCard = true;

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
        if (objectIsCard)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        
        
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
        if (gameObject.CompareTag("board") || gameObject.CompareTag("eraser"))
        {
            m_HoldingHand.handAnimator.SetBool("BoardInHand", true);
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
        
    }

    public void resetHand()
    {
        if (m_HoldingHand != null)
        {
            Debug.Log("resetHand - m_HoldingHand = " + m_HoldingHand.name);
            m_HoldingHand.AttachPoint.localPosition = _originalAttachPointPosition;
            m_HoldingHand.AttachPoint.localRotation = _originalAttachPointRotation;
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
            if (gameObject.CompareTag("board") || gameObject.CompareTag("eraser"))
            {
                m_HoldingHand.handAnimator.SetBool("BoardInHand", false);
            }
        }
    }

    public void ResetCard(bool instantRelayering)
    {
        if (instantRelayering)
        {
            SetLayerMaskIncludingChildren("Default");
        }
        m_HoldingHand = null;
        toBeStacked = false;
        GetComponentInChildren<Collider>().tag = "card";
        stackedCard = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }


}
