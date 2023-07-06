using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory_Interval : MonoBehaviour
{
    public float _interval = 2f;


    public Transform _target;
    private void Start()
    {

        transform.SetParent(null);

        if (_interval != -1f)
        {
            StartCoroutine(Cor_Destroy());
        }
        else
        {
            transform.position = _target.transform.position + Vector3.up * 8f;
            GetComponent<RectTransform>().localScale = new Vector3(0.02f, 0.02f, 1f);
        }
    }

    private void Update()
    {
        if (_interval == -1)
        {
            if ((_target == null))
            {
                Destroy(this.gameObject);
            }
            else if ((_target.gameObject.activeSelf == false))
            {
                Destroy(this.gameObject);
            }
            else
            {

            }
        }
    }

    IEnumerator Cor_Destroy()
    {
        yield return new WaitForSeconds(_interval);

        Destroy(this.gameObject);
    }
}
