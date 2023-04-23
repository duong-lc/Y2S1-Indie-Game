using Core;
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
}
