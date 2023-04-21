using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Patterns;
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
            this.AddListener(pooledObject.eventType, param => SpawnPooledObject());
        }
    }

    public void ActivateObjectFromPool(RaycastHit hit, EventType type)
    {
        ParticleInitData data = new ParticleInitData() {
            normal = hit.normal,
            position = hit.point
        };
            
        ParticlePool particlePool = new ParticlePool();
        foreach (var pool in _poolList.Where(pool => pool.ParticleEventType == type)) {
            particlePool = pool;
        }

        if (!particlePool) return;
            
        ParticleType particleType = new ParticleType() {
            InitData = data,
            pool = particlePool
        };
            
        EventDispatcher.Instance.FireEvent(type, particleType);
    }
    
    private void SpawnPooledObject()
    {
        throw new NotImplementedException();
    }
}
