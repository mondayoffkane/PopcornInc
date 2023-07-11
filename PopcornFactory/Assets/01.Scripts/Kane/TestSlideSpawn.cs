using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSlideSpawn : MonoBehaviour
{

    public GameObject _ball;


    public float _power = 100f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rigidbody _rb = Managers.Pool.Pop(_ball, transform).GetComponent<Rigidbody>();
            _rb.transform.localPosition = new Vector3(0, Random.Range(6.8f, 9f), Random.Range(-4f, 4f));
            _rb.AddForce(transform.up * _power * Random.Range(0.8f, 1.2f));
            _rb.AddTorque(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
        }
        if (Input.GetKey(KeyCode.W))
        {
            Rigidbody _rb = Managers.Pool.Pop(_ball, transform).GetComponent<Rigidbody>();
            _rb.transform.localPosition = new Vector3(Random.Range(0f, 2.5f), Random.Range(6.8f, 9f), Random.Range(-1f, 1f));
            _rb.AddForce(transform.up * _power * Random.Range(0.8f, 1.2f));
            _rb.AddTorque(new Vector3(Random.Range(0f, 30f), Random.Range(0f, 30f), Random.Range(0f, 30f)));
        }
    }
}
