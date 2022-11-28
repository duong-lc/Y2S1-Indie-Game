using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

public class ScoreManager : MonoBehaviour
{
    private void Awake()
    {
        EventDispatcher.Instance.AddListener(EventType.OnNoteHitEvent, param => OnHit());
        EventDispatcher.Instance.AddListener(EventType.OnNoteMissEvent, param => OnMiss());
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnHit()
    {
        
    }

    private void OnMiss()
    {
        
    }
}
