using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory_Interval : MonoBehaviour
{
    public float _interval = 2f;



    private void Start()
    {

        transform.SetParent(null);

        StartCoroutine(Cor_Destroy());
    }

    IEnumerator Cor_Destroy()
    {
        yield return new WaitForSeconds(_interval);

        Destroy(this.gameObject);
    }
}
