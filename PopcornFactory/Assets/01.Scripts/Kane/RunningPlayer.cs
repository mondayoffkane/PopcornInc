using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningPlayer : MonoBehaviour
{
    bool isClick;
    Vector3 Init_Pos;
    Vector3 Start_Pos;
    Vector3 Start_Point, End_Point;
    public float Move_Speed = 10f;
    public float Sense = 0.05f;
    public float Move_x;
    public float BaseLimit_x = 4.2f;
    public float CurrentLimit_x_left = -4.2f;
    public float CurrentLimit_x_right = 4.2f;

    public GameObject _bulletPref;

    // ================================
    public Transform[] _shootPos;
    public int _gunCount = 1;


    public float _range = 10;
    public float _damage = 10;
    public float _speed = 20f;
    public float _fireRate = 0.8f;

    [SerializeField] float _currentInterval = 0f;

    // ====================================================
    void Start()
    {
        _bulletPref = Resources.Load<GameObject>("Bullet");
    }


    void Update()
    {
        Move();

    }


    public void Move()
    {



        if (Input.GetMouseButtonDown(0))
        {
            isClick = true;
            Start_Point = Input.mousePosition;
            End_Point = Input.mousePosition;

            Start_Pos = transform.position;



        }
        else if (Input.GetMouseButton(0))
        {
            End_Point = Input.mousePosition;
            transform.Translate(transform.forward * Move_Speed * Time.deltaTime);

            Move_x = End_Point.x - Start_Point.x;

            transform.position = new Vector3(Start_Pos.x + Move_x * Sense, transform.position.y, transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isClick = false;

        }


        if (transform.position.x > CurrentLimit_x_right)
        {
            transform.position = new Vector3(CurrentLimit_x_right, transform.position.y, transform.position.z);
            Start_Point = Input.mousePosition;
            Start_Pos = transform.position;
            Move_x = 0f;
        }
        else if (transform.position.x < CurrentLimit_x_left)
        {
            transform.position = new Vector3(CurrentLimit_x_left, transform.position.y, transform.position.z);
            Start_Point = Input.mousePosition;
            Start_Pos = transform.position;
            Move_x = 0f;
        }

        if (isClick)
        {
            _currentInterval += Time.deltaTime;
            if (_currentInterval >= _fireRate)
            {
                _currentInterval = 0f;
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _gunCount++;
            if (_gunCount > 4) _gunCount = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _gunCount--;
            if (_gunCount < 1) _gunCount = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _fireRate -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _fireRate += 0.1f;
        }
    }


    public void Shoot()
    {

        for (int i = 0; i < _gunCount; i++)
        {
            //Bullet _bullet = Managers.Pool.Pop(_bulletPref).GetComponent<Bullet>();
            Bullet _bullet = Instantiate(_bulletPref).GetComponent<Bullet>();
            _bullet.transform.position = _shootPos[i].position;
            _bullet.InitStat(_range, _damage, _speed);
        }



        //Bullet _bullet = Managers.Pool.Pop(_bulletPref).GetComponent<Bullet>();
        //_bullet.InitStat(_range, _damage, _speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            Gate _gate = other.GetComponent<Gate>();
            switch (_gate._gateType)
            {
                case Gate.GateType.Damage:
                    _damage += _gate.value * 0.01f;
                    break;

                case Gate.GateType.Range:
                    _range += _gate.value * 0.01f;
                    break;

                case Gate.GateType.FireRate:
                    _fireRate -= _gate.value * 0.001f;
                    break;
            }

        }
    }


}
