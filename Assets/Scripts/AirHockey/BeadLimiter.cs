using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BeadLimiter : NetworkBehaviour
{
    [SerializeField] private Transform _lim1;
    [SerializeField] private Transform _lim2;
    [SerializeField] private float _threshold = 0.001f;
    private float _distLim1;
    private float _distLim2;
    
    private Rigidbody _beadRb;
    // Start is called before the first frame update
    private void Start()
    {
        _beadRb = GetComponent<Rigidbody>();
    }

    
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        var position = transform.position;
        _distLim1 = Vector3.Distance(_lim1.position, position);
        _distLim2 = Vector3.Distance(_lim2.position, position);
        Debug.Log(_distLim1);
        if (Math.Round(_distLim1,2) < _threshold)
        {
            _beadRb.velocity = Vector3.zero;
        }
        if (Math.Round(_distLim2,2) < _threshold)
        {
            _beadRb.velocity = Vector3.zero;
        }
    }
}
