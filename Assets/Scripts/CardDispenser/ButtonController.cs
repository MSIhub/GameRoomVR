using System;
using UnityEngine;

namespace CardDispenser
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Transform _buttonBase;
        [SerializeField] private Transform _buttonPress;

        private bool _isButtonPressed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_isButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,-0.01f,0), _buttonBase);
            _isButtonPressed = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,0.01f,0), _buttonBase);
            _isButtonPressed = false;

        }
    }
}
