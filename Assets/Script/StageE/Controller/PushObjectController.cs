using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PushObjectController : MonoBehaviour
{
    Rigidbody _rb;
    Vector3 _origin;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _origin = transform.position;
    }

    void FixedUpdate()
    {
        if (transform.position.z - _origin.z > 0.6f) _rb.MovePosition(transform.position - 0.3f * Time.deltaTime * Vector3.forward);
        if (transform.position.z - _origin.z < 0.6f) _rb.MovePosition(transform.position + 0.3f * Time.deltaTime * Vector3.forward);
    }
}