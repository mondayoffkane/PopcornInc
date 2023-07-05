using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using static Product;

public class Staff : MonoBehaviour
{
    public Stack<Product> _productStack = new Stack<Product>();
    public Transform _target;
    public float _minDis = 1f;

    public float _pickInterval = 0.5f;
    public float _current_pickInterval = 0f;
    public float _moveInterval = 0.5f;
    public float _speed = 5f;


    public Table _table;
    public Cup _cup;

    StageManager _stageManager;
    NavMeshAgent _agent;

    public enum StaffState
    {
        Idle,
        Wait,
        PickUp,
        Move,
        PickDown,

    }
    public StaffState _staffState;

    public Product.ProductType _productType;
    Animator _animator;
    // ================================

    private void Start()
    {
        _stageManager = Managers.Game._stageManager;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed * Random.Range(1f, 1.2f);
        _agent.Warp(transform.position);
        //_cup = _stageManager._cupObj.GetComponent<Cup>();
        _animator = transform.GetComponent<Animator>();
    }

    void SetDest(Transform _trans)
    {
        _agent.destination = _trans.position;

        _agent.speed = _speed * Random.Range(1f, 1.2f);
        _agent.angularSpeed = Random.Range(120f, 360f);
        // add new destination
        _animator.SetBool("isWalk", true);
    }

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    _agent.areaMask = 0;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    _agent.areaMask = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    _agent.areaMask++;
        //}


        switch (_staffState)
        {

            case StaffState.Idle:
                FindWork();


                break;

            case StaffState.Wait:

                break;

            case StaffState.Move:
                if (_agent.remainingDistance <= _minDis)
                {
                    if (_productStack.Count <= 0)
                    {
                        _staffState = StaffState.PickUp;

                    }
                    else
                    {
                        _staffState = StaffState.PickDown;

                    }
                    _animator.SetBool("isWalk", false);
                }
                break;

            case StaffState.PickUp:

                if (_productStack.Count < 3)
                {
                    _current_pickInterval += Time.deltaTime;
                    if (_current_pickInterval >= _pickInterval)
                    {
                        _current_pickInterval = 0f;
                        if (_table._productList.Count > 0)
                        {
                            Product _product = _table._productList[0];
                            _table._productList.RemoveAt(0);
                            //_table._productList.Remove(_product);
                            _product.transform.SetParent(transform);
                            switch (_productStack.Count)
                            {
                                case 0:
                                    _product.transform.DOLocalJump(new Vector3(-0.3f, 0f, 1f), 1f, 1, _moveInterval);
                                    break;

                                case 1:
                                    _product.transform.DOLocalJump(new Vector3(0.3f, 0f, 1f), 1f, 1, _moveInterval);
                                    break;

                                case 2:
                                    _product.transform.DOLocalJump(new Vector3(0f, 0.5f, 1f), 1f, 1, _moveInterval);
                                    break;

                            }
                            _productStack.Push(_product);
                            _agent.avoidancePriority--;

                        }
                        else
                        {
                            if (_productStack.Count > 0)
                            {
                                _productType = _productStack.Peek()._productType;
                                //_agent.areaMask = 1;


                                switch (_productType)
                                {
                                    case ProductType.Corn:
                                        if (_stageManager._cupObjs[0].gameObject.activeSelf == false)
                                        {
                                            _target = _stageManager._tableList[1].transform;
                                            _table = _target.GetComponent<Table>();
                                        }
                                        else
                                        {
                                            _target = _stageManager._cupObjs[0].transform;
                                            _cup = _target.GetComponent<Cup>();

                                        }
                                        break;

                                    case ProductType.PopCorn_base:
                                        if (_stageManager._cupObjs[1].gameObject.activeSelf == false)
                                        {
                                            _target = _stageManager._tableList[2].transform;
                                            _table = _target.GetComponent<Table>();
                                        }
                                        else
                                        {
                                            _target = _stageManager._cupObjs[1].transform;
                                            _cup = _target.GetComponent<Cup>();

                                        }
                                        break;

                                    case ProductType.Popcorn_1:
                                    case ProductType.Popcorn_2:
                                    case ProductType.Popcorn_3:
                                        _target = _stageManager._cupObjs[2].transform;
                                        _cup = _target.GetComponent<Cup>();

                                        break;
                                }

                                SetDest(_target);
                                _staffState = StaffState.Move;
                                //Debug.Log("Stage is Move");
                            }
                            else
                            {
                                FindWork();
                            }
                        }
                    }
                }
                else
                {
                    _productType = _productStack.Peek()._productType;

                    switch (_productType)
                    {
                        case ProductType.Corn:
                            if (_stageManager._cupObjs[0].gameObject.activeSelf == false)
                            {
                                _target = _stageManager._tableList[1].transform;
                                _table = _target.GetComponent<Table>();
                            }
                            else
                            {
                                _target = _stageManager._cupObjs[0].transform;
                                _cup = _target.GetComponent<Cup>();

                            }
                            break;

                        case ProductType.PopCorn_base:
                            if (_stageManager._cupObjs[1].gameObject.activeSelf == false)
                            {
                                _target = _stageManager._activeTableList[Random.Range(0, _stageManager._activeTableList.Count)].transform;
                                _table = _target.GetComponent<Table>();
                            }
                            else
                            {
                                _target = _stageManager._cupObjs[1].transform;
                                _cup = _target.GetComponent<Cup>();

                            }
                            break;

                        case ProductType.Popcorn_1:
                        case ProductType.Popcorn_2:
                        case ProductType.Popcorn_3:
                            _target = _stageManager._cupObjs[2].transform;
                            _cup = _target.GetComponent<Cup>();

                            break;
                    }

                    SetDest(_target);
                    _staffState = StaffState.Move;
                    //Debug.Log("Stage is Move");
                }
                break;

            case StaffState.PickDown:

                if (_productStack.Count > 0)
                {
                    _current_pickInterval += Time.deltaTime;
                    if (_current_pickInterval >= _pickInterval)
                    {
                        _current_pickInterval = 0f;
                        Transform _trans = _productStack.Pop().transform;
                        _agent.avoidancePriority--;
                        if (_target.GetComponent<Table>() != null)
                        {
                            _table.PushProduct(_trans);
                        }
                        else if (_target.GetComponent<Cup>() != null)
                        {
                            _cup.PushProduct(_trans);
                        }
                    }

                }
                else
                {
                    _staffState = StaffState.Idle;
                    _agent.avoidancePriority = 50;


                }




                break;

            default:

                break;
        }


    }

    public void FindWork()
    {
        //_target = _stageManager._machineList[Random.Range(0, _stageManager._machineList.Count)].transform;
        //_table = _target.GetComponent<Machine>()._table;
        //_table = _stageManager._targetList[Random.Range(0, _stageManager._targetList.Count)];


        //int _num = Random.Range(0, _stageManager._targetList.Count);
        _target = ReFind();

        //_target = _stageManager._targetList[Random.Range(0, _stageManager._targetList.Count)].transform;
        _table = _target.GetComponent<Table>();

        //_agent.areaMask = 9;


        SetDest(_target);
        _staffState = StaffState.Move;


        Transform ReFind(int _val = -1)
        {
            int _num;
            if (_val == -1)
            {
                _num = Random.Range(0, _stageManager._targetList.Count);
                if (_stageManager._targetList[_num].GetComponent<Table>()._productList.Count > 0)
                {
                    return _stageManager._targetList[_num].transform;
                }
                else
                {
                    return ReFind(_num);
                }
            }
            else
            {
                if (_val == 0)
                {
                    return _stageManager._targetList[_val].transform;
                }
                else { }
                if (_stageManager._targetList[_val - 1].GetComponent<Table>()._productList.Count > 0)
                {
                    return _stageManager._targetList[_val - 1].transform;
                }
                else
                {
                    return ReFind(_val - 1);
                }
            }


        }


    }






}
