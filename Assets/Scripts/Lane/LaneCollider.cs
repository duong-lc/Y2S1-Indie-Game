using System;
using System.Collections;
using System.Collections.Generic;
using StaticClass;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class LaneCollider : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;
    private List<Collider> noteList = new ();
    private LayerMask _noteLayerMask;
    
    public Rigidbody Rigidbody {
        get {
            if (!_rb) {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
    }
    
    public Collider Collider {
        get {
            if (!_col) {
                _col = GetComponent<Collider>();
            }
            return _col;
        }
    }

    private void Awake()
    {
        Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        // if(other.gameObject.layer)
        CheckLayerMask.IsInLayerMask(other.gameObject, _noteLayerMask);
    }
}
