using UnityEngine;

namespace CardDispenser
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Transform _buttonBase;
        [SerializeField] private Transform _buttonPress;
        [SerializeField] private float _zOffset = 0.01f;

        public bool IsButtonPressed { get; set; }

        public void Spawned()
        {
            IsButtonPressed = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (IsButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,-_zOffset,0), _buttonBase);
            IsButtonPressed = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsButtonPressed) return;
            _buttonPress.Translate(new Vector3(0,_zOffset,0), _buttonBase);
            IsButtonPressed = false;

        }
    }
}
