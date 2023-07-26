using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBullet : MonoBehaviour
{


    public float _speed = 10;
    public float _damage = 10;


    public void SetInit(float Speed, float Damage)
    {
        _speed = Speed;
        _damage = Damage;

    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}
