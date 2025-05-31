using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject<T>
{
    public void Subscribe(IObserver<T> observer);
    public void Unsubscribe(IObserver<T> observer);
    public void Publish(T value);
}
