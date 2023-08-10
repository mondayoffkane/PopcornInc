using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPIWorker : MonoBehaviour
{

    public Animator _animator;


    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public KeyCode[] _keys;

    void Update()
    {

        if (Input.GetKeyDown(_keys[0]))
        {
            _animator.SetBool("Axe_1", !_animator.GetBool("Axe_1"));
            _animator.SetBool("Axe_2", false);
            _animator.SetBool("Axe_3", false);
        }
        else if (Input.GetKeyDown(_keys[1]))
        {
            _animator.SetBool("Axe_2", !_animator.GetBool("Axe_2"));
            _animator.SetBool("Axe_1", false);
            _animator.SetBool("Axe_3", false);
        }
        else if (Input.GetKeyDown(_keys[2]))
        {
            _animator.SetBool("Axe_3", !_animator.GetBool("Axe_3"));
            _animator.SetBool("Axe_2", false);
            _animator.SetBool("Axe_1", false);
        }
        else if (Input.GetKeyDown(_keys[3]))
        {
            _animator.SetBool("Axe_3", false);
            _animator.SetBool("Axe_2", false);
            _animator.SetBool("Axe_1", false);
        }

    }
}
