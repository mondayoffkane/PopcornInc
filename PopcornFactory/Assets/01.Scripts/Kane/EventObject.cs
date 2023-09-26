using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventObject : MonoBehaviour
{

    public UnityEvent _unlockEvent;
   



    public void CallUnlock()
    {
        _unlockEvent.Invoke();
    }




}
