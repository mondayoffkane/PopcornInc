using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;


public class Explosion : MonoBehaviour
{

    public float _power;
    public float _radius;


    Rigidbody[] _rigs;

    public float scale_time = 1f;

    public AnimationCurve _ease;



    public KeyCode _key;
    public float _time = 1f;
    Vector3 _pos;
    private void Start()
    {
        _pos = transform.localPosition;

    }

    private void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            transform.localPosition = _pos;
            transform.DOLocalMoveY(15f, _time).SetEase(Ease.Linear);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().isKinematic = false;
            other.GetComponent<Rigidbody>().AddExplosionForce(_power * Random.Range(0.8f, 1.2f), transform.position, _radius);
            other.GetComponent<Rigidbody>().AddForce(Vector3.down * 100f);
            other.transform.SetParent(transform.parent);
            //other.transform.DOScale(Vector3.zero, 1f).SetEase(_ease);
        }
    }



    public void ExplosionObj()
    {



        Collider[] _cols = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider col in _cols)
        {
            col.GetComponent<Rigidbody>().isKinematic = false;
            col.GetComponent<Rigidbody>().AddExplosionForce(_power, transform.position, _radius);
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(Color.gray.r, Color.gray.g, Color.gray.b, 0.5f);
        Gizmos.DrawSphere(transform.position, _radius);

    }

}
