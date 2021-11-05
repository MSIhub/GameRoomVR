using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        private float colliderXoffset = 0.125f;

        private int previousCardHighlightNumber = 99;
        private Material initialCardMaterial;
        
        [SerializeField] private InputActionReference _selectCardInputActionRightHand;
        [SerializeField] private InputActionReference _selectCardInputActionLeftHand;

        private void Awake()
        {
            _selectCardInputActionRightHand.action.performed += getCardFromStack;
            _selectCardInputActionLeftHand.action.performed += getCardFromStack;;
        }

        private void getCardFromStack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if(highlightCard){
                Debug.Log("grab card card number = " + previousCardHighlightNumber);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            colliderXoffset = GetComponent<Collider>().bounds.extents.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (highlightCard)
            {
                //calculate x position of cardSelector
                float distanceToStackOrigin = Vector3.Distance(new Vector3(cardSelectorTransform.position.x, 0, 0),
                    new Vector3(transform.position.x - colliderXoffset, 0, 0))/(colliderXoffset*2);
                int cardNumberToHighlight = Mathf.FloorToInt(distanceToStackOrigin * cardsInStack.Count);
                if (cardNumberToHighlight != previousCardHighlightNumber)
                {
                    //check if this is the first card to be highlighted or not, if it is not the first card, then reset the previous cards material
                    if (previousCardHighlightNumber < cardsInStack.Count)
                    {
                        cardsInStack[previousCardHighlightNumber].GetComponentInChildren<Renderer>().material =
                            initialCardMaterial;
                    }
                    previousCardHighlightNumber = cardNumberToHighlight;
                    initialCardMaterial = cardsInStack[cardNumberToHighlight].GetComponentInChildren<Renderer>().material;
                    cardsInStack[cardNumberToHighlight].GetComponentInChildren<Renderer>().material = highlightMaterial;

                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("card") && !ghostShow)
            {
                //create ghost and indicate position
                Debug.Log("card entered Stack = " + other.transform.parent.name);
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
                Debug.Log("cardSelector entered stack - hand = " + other.GetComponent<stackedCardSelector>().selectorHandSide);
                if (other.GetComponent<stackedCardSelector>().selectorHandSide == Hand.HandSide.left)
                {
                    
                    if (!GetComponentInParent<Player>().LeftHand.m_Grabbing && cardsInStack != null && cardsInStack.Count > 0) highlightCard = true;
                    
                }
                else
                {
                    if (!GetComponentInParent<Player>().RightHand.m_Grabbing && cardsInStack != null && cardsInStack.Count > 0) highlightCard = true;
                    Debug.Log("highlight card = true (right hand)");
                }
                cardSelectorTransform = other.transform;
            }
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
            
            if (other.gameObject.layer == LayerMask.NameToLayer("cardSelector"))
            {
                if (other.GetComponent<stackedCardSelector>().selectorHandSide == Hand.HandSide.left)
                {
                    if (!GetComponentInParent<Player>().LeftHand.m_Grabbing) highlightCard = false;
                }
                else
                {
                    if (!GetComponentInParent<Player>().RightHand.m_Grabbing) highlightCard = false;
                }
                cardsInStack[previousCardHighlightNumber].GetComponentInChildren<Renderer>().material =
                    initialCardMaterial;
                previousCardHighlightNumber = 99;
            }
            
        }

        public void destroyStack()
        {
            //unparent all cards in Stack
            Collider[] children = GetComponentsInChildren<Collider>();
            foreach (var child in children)
            {
                if (child.CompareTag("card"))
                {
                    child.transform.parent = child.transform.parent.GetComponent<GrabbableObject>().originalParent;
                }
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
