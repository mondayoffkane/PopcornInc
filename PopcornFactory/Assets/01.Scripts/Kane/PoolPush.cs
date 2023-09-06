using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolPush : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lab_Product"))
        {
            Managers.Pool.Push(other.GetComponent<Poolable>());
        }
    }




}
