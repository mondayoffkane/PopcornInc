using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpring : MonoBehaviour
{

    public float _power;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * _power);
        }
    }

}
