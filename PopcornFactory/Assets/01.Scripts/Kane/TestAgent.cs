using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour
{

    NavMeshAgent _agent;


    public Transform[] _pos = new Transform[2];

    public Transform _target;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = _pos[1];
        _agent.SetDestination(_target.position);
    }

    // Update is called once per frame
    void Update()
    {


        if (_agent.remainingDistance < 1.0f)
        {
            if (_target == _pos[0])
            {
                _target = _pos[1];
                _agent.SetDestination(_target.position);
                _agent.areaMask = 1;
            }
            else
            {
                _target = _pos[0];
                _agent.SetDestination(_target.position);
                _agent.areaMask = 9;
            }
        }
    }
}
