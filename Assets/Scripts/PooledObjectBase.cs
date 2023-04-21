using System;
using System.Collections;
using UnityEngine;


public class PooledObjectBase : MonoBehaviour, IPooledCore<PooledObjectBase>
{
    protected bool canRelease;
    protected Action<PooledObjectBase> KillAction;

    protected virtual void Start()
    {
        canRelease = false;
    }

    public virtual void Init(Action<PooledObjectBase> killAction)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator RunRoutine()
    {
        while (!canRelease) {
            yield return null;
        }
        KillAction(this);
        yield return null;
    }
}
