using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Patterns;
using Data_Classes;
using UnityEngine;
using EventType = Core.Events.EventType;


public class PoolingManager : Singleton<PoolingManager>
{
    [SerializeField] private PoolManagerData alwaysLoadedParticleData;
    [SerializeField] private PoolManagerData sceneBasedParticleData;

    private List<PooledData> _pooledObjectList = new();
    [SerializeField] private List<ObjectPool> _notePoolList = new();
    private void Awake()
    {
        _pooledObjectList = _pooledObjectList.Concat(alwaysLoadedParticleData.pooledDataList)
            .Concat(sceneBasedParticleData.pooledDataList)
            .ToList();

        foreach(var pooledObject in _pooledObjectList){
            this.AddListener(pooledObject.eventType, param => SpawnPooledObject(pooledObject.eventType, (PooledObjectCallbackData) param));
        }
    }

    public void ActivateObjectFromPool(EventType type, NoteData.LaneOrientation orientation)
    {
        EventDispatcher.Instance.FireEvent(type, orientation);
    }
    
    private void SpawnPooledObject(EventType eventType, PooledObjectCallbackData data)
    {
        switch (eventType)
        {
            case EventType.SpawnNoteNormal:
                break;
            case EventType.SpawnNoteSlider:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
