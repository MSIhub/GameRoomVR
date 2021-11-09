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
        private Vector3 _translation;
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
            _translation = _buttonPress.localToWorldMatrix * _buttonPress.up;
            _isDirectionFlipped =
                Math.Round(_translation.x) < 0 | Math.Round(_translation.y) < 0 | Math.Round(_translation.z) < 0;
            _off = _isDirectionFlipped ? _offset : -_offset;
            _buttonPress.Translate(_translation * (_off), _buttonBase);
        }
        
        private void TranslateButtonOut()
        {
            _buttonPress.Translate(_translation * (-_off), _buttonBase);
        }

    }
}
