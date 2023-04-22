using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Patterns;
using Data_Classes;
using StaticClass;
using UnityEngine;
using EventType = Core.Events.EventType;


public class PoolingManager : Singleton<PoolingManager>
{
    [SerializeField] private PoolManagerData alwaysLoadedParticleData;
    [SerializeField] private PoolManagerData sceneBasedParticleData;

    //private Dictionary<EventType, ObjectPool> _eventToPool = new();
    private Dictionary<EventType, PooledData> _eventToPooledData = new();
    [SerializeField] private List<Lane> laneList = new();
    private void Awake()
    {
        _eventToPooledData = _eventToPooledData.AddRange(alwaysLoadedParticleData.eventToPooledData)
            .AddRange(sceneBasedParticleData.eventToPooledData);
        
        
        foreach(var kvp in _eventToPooledData){
            this.AddListener(kvp.Key, param => SpawnPooledObject(kvp.Key, (PooledObjectCallbackData) param));
        }
    }

    private void Start()
    {
        //Init For all note types in lane
        foreach (var kvp in _eventToPooledData)
        {
            foreach (var lane in laneList)
            {
                GameObject poolGO = new GameObject();
                poolGO.transform.parent = lane.transform;
                poolGO.name = $"Pool {kvp.Value.prefab.name}";
                
                var pool = poolGO.AddComponent<ObjectPool>();
                
                pool.Init(kvp.Value.prefab, kvp.Key);
                //_eventToPool.Add(kvp.Key, pool);
                
                pool.InitPool(kvp.Value.prefab, poolGO.transform, kvp.Value.initialPoolSize, kvp.Value.maxPoolSize);
                for (var i = 0; i < kvp.Value.initialPoolSize; i++) {
                    var obj = pool.CreateSetup();
                    obj.transform.parent = poolGO.transform;
                    pool.Release(obj);
                }
            }
        }
    }

    private void SpawnPooledObject(EventType eventType, PooledObjectCallbackData data)
    {
        switch (eventType)
        {
            case EventType.SpawnNoteNormal:
                var normalData = (NoteInitData)data;
                SpawnNote(eventType, normalData.orientation, data);
                break;
            case EventType.SpawnNoteSlider:
                var sliderData = (NoteInitData)data;
                SpawnNote(eventType, sliderData.orientation, data);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SpawnNote(EventType eventType, NoteData.LaneOrientation orientation, PooledObjectCallbackData data)
    {
        foreach (var lane in laneList) {
            if (lane.LaneOrientation != orientation) continue;
            foreach (var pool in lane.NotePools) {
                if (pool.PooledObjectEventType != eventType) continue;
                var obj = pool.Get();
                obj.Init(data, pool.Release);
            }
        }
    }
}
