using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float _speed = 10f;

    Transform _target;
    public float _hp = 5f;



    public enum EnemyState
    {
        Wait,
        Move,
        Dead
    }
    public EnemyState _enemyState;
    private void Start()
    {
        _enemyState = EnemyState.Move;
    }



    // Update is called once per frame
    void Update()
    {
        switch (_enemyState)
        {
            case EnemyState.Wait:

                break;

            case EnemyState.Move:
                //transform.LookAt(_target);
                transform.LookAt(new Vector3(0f, 0.5f, 0f));
                transform.Translate(Vector3.forward * Time.deltaTime * _speed);

                //if(Vector3.Distance(transform.position, _target.position) <= 1f)
                //{
                //    _target.GetComponent<Cannon>()
                //}
                break;

            case EnemyState.Dead:

                break;

            default:

                break;
        }


    }


    public void Attack()
    {

    }






    public void OnDamage(float _value)
    {
        _hp -= _value;

        if (_hp <= 0)
        {
            _enemyState = EnemyState.Dead;

            //transform.gameObject.SetActive(false);
            Destroy(transform.gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnDamage(other.GetComponent<DefenseBullet>()._damage);
            Destroy(other.gameObject);
        }
    }



}
