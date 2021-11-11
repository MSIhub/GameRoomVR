using System;
using UnityEngine;

namespace CardDispenser
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Transform _buttonBase;
        [SerializeField] private Transform _buttonPress;
        [SerializeField] private float _offset = 0.01f;
        private float _off;
        private bool _isDirectionFlipped = false;
        private Vector3 _translationDirection;
        public bool IsButtonPressed { get; set; }

        public void Spawned()
        {
            IsButtonPressed = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (IsButtonPressed) return;
            TranslateButtonIn();
            IsButtonPressed = true;
        }

      
        private void OnTriggerExit(Collider other)
        {
            if (!IsButtonPressed) return;
            TranslateButtonOut();
            IsButtonPressed = false;

        }

        private void TranslateButtonIn()
        {
            _translationDirection = _buttonPress.worldToLocalMatrix * _buttonPress.up;
            _isDirectionFlipped =
                Math.Round(_translationDirection.x) < 0 | Math.Round(_translationDirection.y) < 0 | Math.Round(_translationDirection.z) < 0;
            _off = _isDirectionFlipped ? _offset : -_offset;
            _buttonPress.localPosition += _translationDirection * (_off);
              
            
        }
        
        private void TranslateButtonOut()
        {
            _buttonPress.localPosition -= _translationDirection * (_off);
        }

    }
}
