using System;
using Core;
using NoteClasses;
using UnityEngine;
using EventType = Core.Events.EventType;


public class ObjectPool : PoolBase<PooledObjectBase>
{
    private EventType _pooledObjectEventType;
    public EventType PooledObjectEventType {
        get => _pooledObjectEventType;
    }
        
    private PooledObjectBase _prefabRef;
    
    public void Init(PooledObjectBase prefab, EventType eventType)
    {
        _prefabRef = prefab;
        _pooledObjectEventType = eventType;
    }

    public override PooledObjectBase CreateSetup()
    {
        return Instantiate(_prefabRef, transform);
    }

    public override void Release(PooledObjectBase obj) {
        var note = obj as NoteBase;
        if(note != null) note.Collider.enabled = false;
        
        obj.transform.position = new Vector3(999, 999, 999);
        base.Release(obj);
    }

    public override PooledObjectBase Get()
    {
        var obj = base.Get();
        var note = obj as NoteBase;
        if(note != null) note.Collider.enabled = true;
        
        return obj;
    } 
}
