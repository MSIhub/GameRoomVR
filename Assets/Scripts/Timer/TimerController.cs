using System;
using CardDispenser;
using TMPro;
using UnityEngine;

namespace Timer
{
    public class TimerController : MonoBehaviour
    {

        [SerializeField] private float _timerValueInMinutes = 10f;

        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        private float _timerValueInSeconds;

        private ButtonController _button;
        private void Start()
        {
            _timerValueInSeconds = _timerValueInMinutes * 60;
            _button = GetComponentInChildren<ButtonController>();
        }

        private void Update()
        {
           _timerValueInSeconds -= Time.deltaTime;
           var minutehand = (_timerValueInSeconds % 3600 / 60) - 1;
           var secondhand = _timerValueInSeconds% 60;
           if (minutehand <= 0) { minutehand = 0; }
           if (secondhand <= 0) { secondhand = 0; }

            var text = $"{minutehand:00}:{secondhand% 60:00}";
            _textMeshProUGUI.text = text;
            
            if (_button.IsButtonPressed)
            {
                _timerValueInSeconds = _timerValueInMinutes * 60;
            }

            if (_timerValueInSeconds <= 0)
            {
                _textMeshProUGUI.text = "00 : 00";
            }

           
        }
    }
}
