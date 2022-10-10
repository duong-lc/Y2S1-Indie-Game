using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Events
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T item);
    }
}
