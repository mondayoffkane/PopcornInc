using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using static Product;

public class Staff : MonoBehaviour
{
    public int _pickCount = 1;

    public Stack<Product> _productStack = new Stack<Product>();
    public Transform _target;
    public float _minDis = 1f;

    public float _pickInterval = 0.5f;
    public float _current_pickInterval = 0f;
    public float _moveInterval = 0.5f;
    public float _speed = 5f;


    //public Table _table;
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

    public int _landNum = 0;
    public int _productNum;
    //public Product.ProductType _productType;
    Animator _animator;
    public int _num;
    // ================================

    private void OnEnable()
    {
        Init();

        _stageManager = Managers.Game._stageManager;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed * Random.Range(1f, 1.2f);
        _agent.Warp(transform.position);
        //_cup = _stageManager._cupObj.GetComponent<Cup>();
        _animator = transform.GetComponent<Animator>();
    }

    void Init()
    {
        int _n = _productStack.Count;
        for (int i = 0; i < _n; i++)
        {
            Managers.Pool.Push(_productStack.Pop().GetComponent<Poolable>());
        }
        _productStack.Clear();
        _target = null;
        _staffState = StaffState.Idle;
    }

    public void SetTrans(Vector3 _pos)
    {
        transform.position = _pos;
        _agent.Warp(_pos);
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

                if (_productStack.Count < _pickCount)
                {
                    _current_pickInterval += Time.deltaTime;
                    if (_current_pickInterval >= _pickInterval)
                    {
                        _current_pickInterval = 0f;
                        if (_target.GetComponent<Table>()._productList.Count > 0)
                        {
                            Product _product = _target.GetComponent<Table>()._productList[0];
                            _target.GetComponent<Table>()._productList.RemoveAt(0);

                            _product.transform.SetParent(transform);
                            switch (_productStack.Count % 3)
                            {
                                case 0:
                                    _product.transform.DOLocalJump(new Vector3(-0.5f, 0f, 1f), 1f, 1, _moveInterval);
                                    break;

                                case 1:
                                    _product.transform.DOLocalJump(new Vector3(0.5f, 0f, 1f), 1f, 1, _moveInterval);
                                    break;

                                case 2:
                                    _product.transform.DOLocalJump(new Vector3(0f, 0.75f, 1f), 1f, 1, _moveInterval);
                                    break;

                            }
                            _productStack.Push(_product);
                            _agent.avoidancePriority--;

                        }
                        else
                        {
                            if (_productStack.Count > 0)
                            {
                                _productNum = _productStack.Peek()._productNum;

                                if (_productNum + 1 < _stageManager._land_machineGroup[_landNum].GetLength(0))
                                {
                                    if (_stageManager._land_machineGroup[_landNum][_productNum + 1].gameObject.activeSelf)
                                    {
                                        _target = _stageManager._land_machineGroup[_landNum][_productNum + 1]._table.transform;
                                    }
                                    else
                                    {
                                        _target = _stageManager._cupObjs[_landNum].transform;
                                        _cup = _target.GetComponent<Cup>();
                                    }
                                }
                                else
                                {
                                    _target = _stageManager._cupObjs[_landNum].transform;
                                    _cup = _target.GetComponent<Cup>();
                                }



                                SetDest(_target);
                                _staffState = StaffState.Move;

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

                    _productNum = _productStack.Peek()._productNum;

                    if (_productNum + 1 < _stageManager._land_machineGroup[_landNum].GetLength(0))
                    {
                        if (_stageManager._land_machineGroup[_landNum][_productNum + 1].gameObject.activeSelf)
                        {
                            _target = _stageManager._land_machineGroup[_landNum][_productNum + 1]._table.transform;
                        }
                        else
                        {
                            _target = _stageManager._cupObjs[_landNum].transform;
                            _cup = _target.GetComponent<Cup>();
                        }
                    }
                    else
                    {
                        _target = _stageManager._cupObjs[_landNum].transform;
                        _cup = _target.GetComponent<Cup>();
                    }



                    SetDest(_target);
                    _staffState = StaffState.Move;

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
                            _target.GetComponent<Table>().PushProduct(_trans);
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

        _target = ReFind();


        SetDest(_target);
        _staffState = StaffState.Move;


        Transform ReFind(int _val = -1)
        {

            if (_val == -1)
            {
                _num = Random.Range(0, _stageManager._land_machineGroup[_landNum].GetLength(0));
                if (_stageManager._land_machineGroup[_landNum][_num]._table._productList.Count > 0)
                {

                    return _stageManager._land_machineGroup[_landNum][_num]._table.transform;
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

                    return _stageManager._land_machineGroup[_landNum][_val]._table.transform;
                }
                else
                {

                    if (_stageManager._land_machineGroup[_landNum][_val - 1]._table._productList.Count > 0)
                    {

                        return _stageManager._land_machineGroup[_landNum][_val - 1]._table.transform;
                    }
                    else
                    {
                        return ReFind(_val - 1);
                    }
                }
            }


        }


    }

}
