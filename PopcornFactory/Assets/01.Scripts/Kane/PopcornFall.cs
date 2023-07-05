using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornFall : MonoBehaviour
{

    public GameObject _corn;

    public float _power = 100f;
    public int _one_count = 1;

    public Material _mat;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < _one_count; i++)
            {
                Rigidbody _rig = Managers.Pool.Pop(_corn, transform).GetComponent<Rigidbody>();
                _rig.GetComponent<Renderer>().material = _mat;
                _rig.GetComponent<MeshFilter>().sharedMesh = _corn.GetComponent<MeshFilter>().sharedMesh;
                _rig.transform.position = transform.position;
                _rig.AddForce(transform.forward * _power);

            }
        }
    }
}
