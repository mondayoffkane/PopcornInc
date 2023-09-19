using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnlockObj : MonoBehaviour
{

    public UnityEvent _testEvent;


    public void CallFunc()
    {

        _testEvent.Invoke();
    }



}
