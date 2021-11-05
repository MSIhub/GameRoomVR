using UnityEngine;

namespace CardDispenser
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Transform _buttonBase;
        [SerializeField] private Transform _buttonPress;

        public bool IsButtonPressed { get; set; }

        public void Spawned()
        {
            IsButtonPressed = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (IsButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,-0.01f,0), _buttonBase);
            IsButtonPressed = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,0.01f,0), _buttonBase);
            IsButtonPressed = false;

        }
    }
}
