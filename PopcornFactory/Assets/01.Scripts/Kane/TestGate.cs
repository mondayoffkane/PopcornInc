using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGate : MonoBehaviour
{
    public GameObject _ball;
    public int _scope;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            for (int i = 0; i < _scope; i++)
            {
                Transform _newball = Managers.Pool.Pop(_ball, transform).transform;
                _newball.tag = "None";
                _newball.position = other.transform.position;
            }
        }
    }

}
