using Core;
using UnityEngine;


public class ObjectPool : PoolBase<PooledObjectBase>
{
    private EventType _pooledObjectEventType;
    public EventType PooledObjectEventType {
        get => _pooledObjectEventType;
    }
        
    private PooledObjectBase _prefab;
    
    public void Init(PooledObjectBase prefab, EventType eventType)
    {
        _prefab = prefab;
        _pooledObjectEventType = eventType;
    }

    public override PooledObjectBase CreateSetup()
    {
        return Instantiate(_prefab, transform);
    }
}
