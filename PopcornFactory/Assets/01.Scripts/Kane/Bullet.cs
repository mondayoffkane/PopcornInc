using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    public float _range;
    public float _damage;
    public float _speed;


    public float _startPos;

    public void InitStat(float _RangeValue, float _DamageValue, float _SpeedValue = 20f)
    {
        _range = _RangeValue;
        _damage = _DamageValue;
        _speed = _SpeedValue;


        transform.localScale = Vector3.one * (1 + _damage * 0.01f);
        _startPos = transform.position.z;




    }



    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.forward * _speed * Time.deltaTime);

        if (transform.position.z >= _startPos + _range)
        {
            Destroy(this.gameObject);
            // Manager.Pool.Push
        }

    }
}
