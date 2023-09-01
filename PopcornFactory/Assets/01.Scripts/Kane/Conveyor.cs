using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{

    public float _power;



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lab_Product"))
        {
            other.GetComponent<Rigidbody>().AddForce(transform.forward * _power);
        }
    }

}
