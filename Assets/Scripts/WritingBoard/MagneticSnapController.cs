using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WritingBoard
{
    public class MagneticSnapController : MonoBehaviour
    {
        private Transform _originalParent;
        private bool _isMagnetic = false;
        private Rigidbody rbParent ;
        private BoardVisualComponent _boardVisualComponent;

        private void Start()
        {
            var boardRB = gameObject.GetComponentInParent<Rigidbody>();
            _boardVisualComponent = boardRB.GetComponentInChildren<BoardVisualComponent>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<MagneticAttachComponent>(out var magneticAttachComponent)) return;
            rbParent = magneticAttachComponent.GetComponentInParent<Rigidbody>();
            rbParent.position =other.ClosestPoint(magneticAttachComponent.transform.position);
            _originalParent = rbParent.transform.parent;
           // rbParent.isKinematic = true;
           rbParent.transform.parent = _boardVisualComponent.transform;
        }

        
        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent<MagneticAttachComponent>(out var magneticAttachComponent)) return;
            rbParent = magneticAttachComponent.GetComponentInParent<Rigidbody>();
            rbParent.velocity = Vector3.zero;
            rbParent.angularVelocity = Vector3.zero;

        }
        

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<MagneticAttachComponent>(out var magneticAttachComponent)) return;
            rbParent = magneticAttachComponent.GetComponentInParent<Rigidbody>();
            rbParent.isKinematic = false;
            rbParent.transform.parent = _originalParent;
            _isMagnetic = false;
        }
    }
}
