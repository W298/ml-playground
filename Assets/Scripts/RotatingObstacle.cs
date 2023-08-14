using System;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public bool ccw = false;
    public float magnitude;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.angularVelocity = new Vector3(0, Mathf.PI * (ccw ? -1 : 1), 0);
        magnitude = _rigidbody.angularVelocity.magnitude;
    }
}
