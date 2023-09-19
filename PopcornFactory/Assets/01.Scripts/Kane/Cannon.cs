using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Cannon : MonoBehaviour
{

    public GameObject _bulletPref;

    public float _bulletSpeed = 10f;
    public float _bulletDamage = 10f;

    public float _interval = 0.5f;
    public float _range = 20f;
    //================= private =====================================
    [SerializeField] Transform _target;
    //float _currentInterval = 0f;
    WaveManager _waveManager;
    // =======================================================


    public Enemy[] _rangeEnemys;

    public enum CannonState
    {
        Wait,
        Find,
        Attack,
        Dead
    }
    public CannonState _cannonState;

    // ======================================================

    private void Start()
    {
        if (_waveManager == null) _waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();


        StartCoroutine(Cor_Update());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Shoot();
        }
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(_interval);

            if (_target != null)
            {
                //transform.LookAt(new Vector3(_target.position.x, 0.5f, _target.position.z));
                transform.DOLookAt(new Vector3(_target.position.x, 0.5f, _target.position.z), _interval * 0.5f).SetEase(Ease.Linear);
            }



            switch (_cannonState)
            {
                case CannonState.Wait:
                    Shoot();
                    //_cannonState = CannonState.Find;
                    break;

                case CannonState.Find:

                    if (_target == null)
                    {
                        FindEnemy();
                    }
                    else
                    {
                        _cannonState = CannonState.Attack;


                    }



                    break;

                case CannonState.Attack:
                    Shoot();
                    FindEnemy();
                    if (_target == null)
                        _cannonState = CannonState.Find;
                    break;

                case CannonState.Dead:

                    break;

                default:

                    break;

            }
        }
    }


    public void Shoot()
    {
        if (_target != null)
        {
            DefenseBullet _bullet = Instantiate(_bulletPref).GetComponent<DefenseBullet>();
            _bullet.transform.position = transform.position;
            _bullet.transform.LookAt(new Vector3(_target.position.x, 0.5f, _target.position.z)); //transform.position + transform.forward
            _bullet.SetInit(_bulletSpeed, _bulletDamage);
        }
    }

    public void FindEnemy()
    {
        Collider[] _cols = Physics.OverlapSphere(transform.position, _range);
        List<Collider> _colList = new List<Collider>();

        foreach (Collider col in _cols)
        {
            if (col.CompareTag("Enemy"))
            {
                _colList.Add(col);
            }
        }


        //if (_colList.Count > 1)
        //{
        //    Transform[] _array = new Transform[_colList.Count];
        //    for (int i = 0; i < _colList.Count; i++)
        //    {
        //        _array[i] = _colList[i].transform;
        //    }
        //    Array.Sort(_array);
        //    _target = _array[0];
        //}
        //else if (_colList.Count == 1)
        //{
        try
        {
            _target = _colList[0].transform;
        }
        catch { }
        //}

    }


    public void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(1f, 1f, 1f, 0.7f);

        Gizmos.DrawSphere(transform.position, _range);
    }

}
