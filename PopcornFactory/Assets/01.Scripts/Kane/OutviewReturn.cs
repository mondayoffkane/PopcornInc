using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutviewReturn : MonoBehaviour
{

    private void OnBecameInvisible()
    {
        Managers.Pool.Push(transform.GetComponent<Poolable>());
    }

}
