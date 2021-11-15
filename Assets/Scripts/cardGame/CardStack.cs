using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cardGame
{
    public class CardStack : MonoBehaviour
    {
        [SerializeField] private Material ghostMaterial;
        [SerializeField] private Material highlightMaterial;

        private GameObject ghost;
        private bool ghostShow = false;

        public List<GameObject> cardsInStack;
        public Vector3 ghostPosition;
        public Quaternion ghostRotation;
        
        
        private bool highlightCard = false;
        private Transform cardSelectorTransform;
        private float colliderOffset = 0.125f;

        private GameObject previousHighlightCard = null;
        private Material initialCardMaterial;
        private Hand.HandSide selectorHandSide = Hand.HandSide.right;
        
        [SerializeField] private InputActionReference _selectCardInputActionRightHand;
        [SerializeField] private InputActionReference _selectCardInputActionLeftHand;

        private Hand _leftHandReference;
        private Hand _rightHandReference;

        public Hand stackHoldingHand = null;

        private void Awake()
        {
            _selectCardInputActionRightHand.action.performed += getCardFromStack;
            _selectCardInputActionLeftHand.action.performed += getCardFromStack;
            _leftHandReference = GetComponentInParent<Player>().LeftHand;
            Debug.Log("Card Stack spawned, local player id = " +
                        GetComponentInParent<Player>().Runner.LocalPlayer.PlayerId);
            Debug.Log("Card Stack spawned, trraversed player id = " +
                      GetComponentInParent<Player>().GetInstanceID());

            _rightHandReference = GetComponentInParent<Player>().RightHand;
        }

        private void getCardFromStack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if(highlightCard)
            {
                
                Debug.Log("grab card <" + previousHighlightCard.name + "> from Stack");
                GrabbableObject cardGrabba = previousHighlightCard.GetComponent<GrabbableObject>();
                //reset material
                previousHighlightCard.GetComponentInChildren<Renderer>().material = initialCardMaterial;
                //unparent card from stack
                previousHighlightCard.transform.parent = cardGrabba.originalParent;
                //remove card from stack
                cardsInStack.Remove(previousHighlightCard);
                //remove highlight logic
                highlightCard = false;
                //reset Card
                cardGrabba.ResetCard(false);
                //initiate grab with other hand
                if (selectorHandSide == Hand.HandSide.left)
                {
                    _leftHandReference.GrabCardFromStack(previousHighlightCard);
                }
                else
                {
                    _rightHandReference.GrabCardFromStack(previousHighlightCard);
                }
                previousHighlightCard = null;
                //if it is the last card, destroy the stack
                if (cardsInStack.Count == 0)
                {
                    Debug.Log("cards in stack count is 0");
                    destroyStack();
                }
                else
                {
                    //collapse stack
                    GameObject previousCard = null;
                    foreach (var currentCard in cardsInStack)
                    {
                        if (previousCard != null)
                        {
                            //check if distance is bigger than it should be
                            if ((currentCard.transform.localRotation.eulerAngles.y -
                                previousCard.transform.localRotation.eulerAngles.y) > 8f)
                            {
                                //shift left by 1
                                currentCard.transform.Rotate(new Vector3(0,-7.5f, 0));
                                currentCard.transform.Translate(new Vector3(-0.01f,-0.005f,0.005f));
                            }
                        }
                        else
                        {
                            //check if the first card in the stack is at the right position
                            if (currentCard.transform.localRotation.eulerAngles.y > 0)
                            {
                                currentCard.transform.Rotate(new Vector3(0,-7.5f, 0));
                                currentCard.transform.Translate(new Vector3(-0.01f,-0.005f,0.005f));
                            }
                        }
                        previousCard = currentCard;
                    }
                }
                
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Bounds colliderBounds = GetComponent<Collider>().bounds;
            colliderOffset = Vector3.Distance(colliderBounds.extents, (Vector3.zero));
            Debug.Log("Stack initiated, colliderOffset = " + colliderOffset);
        }

        // Update is called once per frame
        void Update()
        {
            if (highlightCard)
            {
                //calculate distance to each card
                float minDistance = 100000000f;
                GameObject cardToHighlight = cardsInStack[0];
                foreach (var currentCard in cardsInStack)
                {
                    if (Vector3.Distance(currentCard.GetComponent<GrabbableObject>().SelectionPoint.position, (cardSelectorTransform.position)) < minDistance)
                    {
                        cardToHighlight = currentCard;
                        minDistance = Vector3.Distance(currentCard.GetComponent<GrabbableObject>().SelectionPoint.position, cardSelectorTransform.position);
                    }
                }
                //change highlight if hover is over another card
                if (cardToHighlight != previousHighlightCard)
                {
                    //check if this is the first card to be highlighted or not, if it is not the first card, then reset the previous cards material
                    if (previousHighlightCard != null)
                    {
                        previousHighlightCard.GetComponentInChildren<Renderer>().material =
                            initialCardMaterial;
                    }
                    previousHighlightCard = cardToHighlight;
                    initialCardMaterial = cardToHighlight.GetComponentInChildren<Renderer>().material;
                    cardToHighlight.GetComponentInChildren<Renderer>().material = highlightMaterial;

                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("card") && !ghostShow)
            {
                //create ghost and indicate position
                Debug.Log("card to enter Stack = " + other.transform.parent.name);
                ghost = Instantiate(other.transform.parent.gameObject, transform);
                ghost.GetComponent<Rigidbody>().isKinematic = true;
                ghost.GetComponent<Rigidbody>().useGravity = false;
                ghost.GetComponentInChildren<Renderer>().material = ghostMaterial;
                
                foreach (var currentCollider in ghost.GetComponentsInChildren<Collider>(true))
                {
                    currentCollider.gameObject.tag = "Untagged";
                }
                ghostShow = true;
                ghost.transform.localPosition = cardsInStack[cardsInStack.Count - 1].transform.localPosition;
                ghost.transform.Translate(new Vector3(0.01f,0.005f,-0.005f));
                ghost.transform.localRotation = cardsInStack[cardsInStack.Count - 1].transform.localRotation;
                ghost.transform.Rotate(new Vector3(0,7.5f,0));
                other.transform.parent.GetComponent<GrabbableObject>().toBeStacked = true;
                ghostPosition = ghost.transform.localPosition;
                ghostRotation = ghost.transform.localRotation;
            }

            if (other.CompareTag("cardSelector"))
            {
                handleFingerTipEnteredStack(other);
            }
        }

        public void handleFingerTipEnteredStack(Collider other)
        {
            if (other.GetComponent<stackedCardSelector>().selectorHandSide == Hand.HandSide.left)
            {
                //for Left hand
                if (!_leftHandReference.m_Grabbing && cardsInStack != null &&
                    cardsInStack.Count > 0)
                {
                    _leftHandReference.preventGrabbing = true;
                    highlightCard = true;
                    selectorHandSide = Hand.HandSide.left;
                }
            }
            else
            {
                //for Right hand
                if (!_rightHandReference.m_Grabbing && cardsInStack != null &&
                    cardsInStack.Count > 0)
                {
                    _rightHandReference.preventGrabbing = true;
                    highlightCard = true;
                    selectorHandSide = Hand.HandSide.right;
                }
            }

            cardSelectorTransform = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("card"))
            {
                Debug.Log("card exited Stack = " + other.transform.parent.name);
                Destroy(ghost);
                ghostShow = false;
                other.transform.parent.GetComponent<GrabbableObject>().toBeStacked = false;
            }
            
            if (other.CompareTag("cardSelector"))
            {
                if (selectorHandSide == Hand.HandSide.left)
                {
                    _leftHandReference.preventGrabbing = false;
                }
                else
                {
                    _rightHandReference.preventGrabbing = false;
                }
                highlightCard = false;
                if(previousHighlightCard != null) previousHighlightCard.GetComponentInChildren<Renderer>().material =
                        initialCardMaterial;
                previousHighlightCard = null;
            }
            
        }

        public void destroyStack()
        {
            Debug.Log("stack destruction requested");
            //if there are still cards in the stack unparent them before destroying the whole stack
            if (cardsInStack != null && cardsInStack.Count > 0)
            {
                foreach (var currentCard in cardsInStack)
                {
                    currentCard.GetComponent<GrabbableObject>()?.ResetCard(true);
                    currentCard.transform.parent = currentCard.GetComponent<GrabbableObject>().originalParent;
                }
            }
            //reset hand
            stackHoldingHand.ResetAttachPoint();
            //TODO: remove hard coded reference to handAnimator. This should be flexible, maybe there will be a specific stack animator in the future
            stackHoldingHand.handAnimator.SetBool("cardInHand", false);
            FindObjectOfType<gameManagerLocal>().stackSpawned = false;
            FindObjectOfType<gameManagerLocal>().Stack = null;
            //reenable selector
            foreach (var selector in stackHoldingHand.transform.parent.GetComponentsInChildren<stackedCardSelector>(true))
            {
                selector.gameObject.SetActive(true);
            }
            Destroy(gameObject);
        }

        public void addCardToStack(GameObject card)
        {
            cardsInStack.Add(card);
            if (ghostShow)
            {
                ghostShow = false;
                Destroy(ghost);
            }
        }

        private void OnDisable()
        {
            _selectCardInputActionLeftHand.action.performed -= getCardFromStack;
            _selectCardInputActionRightHand.action.performed -= getCardFromStack;
        }
    }
}
