using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventObject : MonoBehaviour
{

    public UnityEvent _unlockEvent;
    //[HideInInspector] public UnityEvent _cleanEvent;



    public void CallUnlock()
    {
        _unlockEvent.Invoke();
    }

    //public void CallClean()
    //{
    //    _cleanEvent.Invoke();
    //}



}
