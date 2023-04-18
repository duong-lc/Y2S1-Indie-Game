using System;
using System.Collections;
using System.Collections.Generic;
using Data_Classes;
using NoteClasses;
using StaticClass;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class LaneCollider : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;
    private List<NoteBase> noteList = new ();
    private LayerMask _noteLayerMask;

    private NoteData.LaneOrientation _laneOrientation
    {
        get
        {
            

            return NoteData.LaneOrientation.Undefined;
        }
    }
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

    private void Awake() {
        Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider col) {
        var note = GetNote(col);
        
        if (note) noteList.Add(note);
    }

    private void OnTriggerExit(Collider col) {
        var note = GetNote(col);
        if (note) noteList.Remove(note);
    }

    private NoteBase GetNote(Collider col) {
        var note = col.GetComponent<NoteBase>();
        return !note || noteList.Contains(note) ? null : note;
    }
}
