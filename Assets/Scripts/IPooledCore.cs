using System;
using System.Collections;
using System.Collections.Generic;

public interface IPooledCore<out T>
{
    void Init(Action<T> killAction);
    IEnumerator RunRoutine();
}
