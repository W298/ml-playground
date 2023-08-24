using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Rigidbody _center;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _center = transform.GetChild(0).GetComponent<Rigidbody>();

        _center.maxAngularVelocity = float.MaxValue;
    }

    private void Update()
    {
        var moveDir = new Vector3(Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0, 0, Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0);
        
        _center.AddTorque(transform.up * (Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0) * 100);
        _rigidbody.AddForce(moveDir * 100000);
        
        _rigidbody.AddTorque(Vector3.up * (Input.GetKey(KeyCode.Z) ? -1 : Input.GetKey(KeyCode.C) ? 1 : 0) * 10000);
    }
}