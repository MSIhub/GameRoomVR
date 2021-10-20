using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hands
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] private InputActionReference _gripInputAction;
        [SerializeField] private InputActionReference _triggerInputAction;
        [SerializeField] private InputActionReference _thumbInputAction;
        
        private Animator _handAnimator;

        private void Awake()
        {
            _handAnimator = GetComponent<Animator>();
        }
/*
        private void OnEnable()
        {
            _gripInputAction.action.performed += GripPressed;
            _triggerInputAction.action.performed += TriggerPressed;
            _thumbInputAction.action.started += ThumbDown;
            _thumbInputAction.action.canceled += ThumbUp;
        }

        private void ThumbDown(InputAction.CallbackContext obj)
        {
            _handAnimator.SetBool("ThumbDown", true);
        }
        
        private void ThumbUp(InputAction.CallbackContext obj)
        {
            _handAnimator.SetBool("ThumbDown", false);
        }

        private void TriggerPressed(InputAction.CallbackContext obj)
        {
            _handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
        }

        private void GripPressed(InputAction.CallbackContext obj)
        {
            _handAnimator.SetFloat("Grip", obj.ReadValue<float>());
        }

        private void OnDisable()
        {
            _gripInputAction.action.performed -= GripPressed;
            _triggerInputAction.action.performed -= TriggerPressed;
            _thumbInputAction.action.started -= ThumbDown;
            _thumbInputAction.action.canceled -= ThumbUp;
        }
        */
    }
}
