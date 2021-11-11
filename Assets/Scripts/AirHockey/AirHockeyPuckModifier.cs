using System;
using Fusion;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyPuckModifier : NetworkBehaviour
    {
        [SerializeField] private float _impactIntensity = 1.5f;
        [SerializeField] private AudioSource _audioSource;
        private Rigidbody _rb;
        public void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void OnCollisionEnter(Collision other)
        {
            if (_rb==null) return;
            if (other.gameObject.CompareTag("SideWalls") || other.gameObject.CompareTag("Striker"))
            {
                _rb.velocity = _rb.velocity.normalized * _impactIntensity;
                _rb.angularVelocity = _rb.angularVelocity.normalized * _impactIntensity*4f;
            }

            if (!other.gameObject.CompareTag("SideWalls")) return;
            _audioSource.Play();
        }
    }
}
