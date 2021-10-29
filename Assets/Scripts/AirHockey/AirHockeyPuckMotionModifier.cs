using System;
using Fusion;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyPuckMotionModifier : NetworkBehaviour
    {
        [SerializeField] private float _impactIntensity = 1.5f;
        private Rigidbody _rb;
        public void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.name);
            _rb.velocity = _rb.velocity.normalized * _impactIntensity;
        }
    }
}
