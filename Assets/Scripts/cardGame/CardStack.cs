using System;
using System.Collections.Generic;
using UnityEngine;

namespace cardGame
{
    public class CardStack : MonoBehaviour
    {
        [SerializeField] private Material ghostMaterial;

        private GameObject ghost;
        private bool ghostShow = false;

        public List<GameObject> cardsInStack;
        public Vector3 ghostPosition;
        
        private Player PlayerRef;

        private void Awake()
        {
            PlayerRef = FindObjectOfType<Player>();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
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
                ghost.transform.Translate((new Vector3(0.01f,0.005f,-0.005f)+cardsInStack[cardsInStack.Count-1].transform.localPosition));
                ghost.transform.Rotate(new Vector3(0,7.5f,0));
                other.transform.parent.GetComponent<GrabbableObject>().toBeStacked = true;
                ghostPosition = ghost.transform.localPosition;
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
        }
    }
}
