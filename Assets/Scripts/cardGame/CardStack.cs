using System;
using UnityEngine;

namespace cardGame
{
    public class CardStack : MonoBehaviour
    {
        [SerializeField] private Material ghostMaterial;

        private GameObject ghost;
        
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
            Debug.Log("collided with = " + other.gameObject.name);
            if (other.CompareTag("card"))
            {
                //create ghost and indicate position

                ghost = Instantiate(other.gameObject, transform);
                ghost.GetComponent<Renderer>().material = ghostMaterial;
                ghost.transform.Translate(new Vector3(0,0.01f,0));
                ghost.transform.Rotate(new Vector3(0,0,15));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Destroy(ghost);
        }
    }
}
