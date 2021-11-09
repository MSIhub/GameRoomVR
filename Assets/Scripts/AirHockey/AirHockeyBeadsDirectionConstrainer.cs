using System;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyBeadsDirectionConstrainer : MonoBehaviour
    {

        [SerializeField] private GameObject _holder;
        private void Start()
        {
            var rb = gameObject.GetComponent<Rigidbody>();
            FreezeMotionBasedOnHolderForwardAxis(rb);
        }

        private void FreezeMotionBasedOnHolderForwardAxis(Rigidbody rb)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 pos = _holder.transform.localToWorldMatrix * _holder.transform.forward;
            if (Math.Round(pos.x) != 0)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
            }

            if (Math.Round(pos.y) != 0)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            }

            if (Math.Round(pos.z) != 0)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            }
        }
    }
}
