using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

//using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Mesh[] _meshes;
    SkinnedMeshRenderer _skinnedMesh;
    public float _minDist = 0.5f;

    public CinemaManager _cinemaManager;

    public bool isArrive = false;

    public NavMeshAgent _agent;

    public float Current_ChargingTime = 0f;


    public int _productType;
    public int OrderCount = 5;

    public Transform StackPos;

    public float Stack_Interval = 0.2f;

    public float BaseUp_Interval = 0.5f;


    public Stack<CinemaProduct> _productStack = new Stack<CinemaProduct>();

    [SerializeField] Vector3 Init_StackPointPos;

    public Room _room;

    public float _distance = 0f;

    public enum State
    {
        Init,
        Wait,
        Order,
        Move,
        Seat,
        Exit
    }
    public State CustomerState;
    public Animator _animator;
    // ===============================
    private void Start()
    {
        StackPos = transform.Find("StackPos");
        Init_StackPointPos = StackPos.transform.localPosition;
    }



    public void SetInit(CinemaManager _cinemamanager, int _type, int _count)
    {
        isArrive = false;
        _productStack.Clear();


        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        _cinemaManager = _cinemamanager;
        CustomerState = State.Init;
        _productType = _type;
        OrderCount = _count;


        if (_animator == null) _animator = GetComponent<Animator>();
        _animator.SetBool("Walk", true);
        _animator.SetBool("Pick", false);

        _room = null;


        if (_skinnedMesh == null) _skinnedMesh = transform.Find("Customer").GetComponent<SkinnedMeshRenderer>();
        _skinnedMesh.sharedMesh = _meshes[Random.Range(0, 2)];
    }
    [Button]
    public void SetDest(Vector3 _destiny)
    {

        _animator.SetBool("Walk", true);
        //_agent.destination = _destiny;

        _agent.SetDestination(_destiny);


        isArrive = false;

        //PrintPos();

        if (CustomerState == State.Wait)
            transform.DORotate(new Vector3(0f, 180f, 0f), 0.4f).SetEase(Ease.Linear);
    }

    //[Button]
    //public void PrintPos()
    //{
    //    Debug.Log("Transform pos :" + transform.position);
    //    Debug.Log("Destiny : " + _agent.destination);
    //    Debug.Log("Distance : " + _distance);
    //    Debug.Log("Remain Distnace :" + _agent.remainingDistance);
    //}

    private void Update()
    {
        if (!_agent.pathPending)
        {
            _distance = _agent.remainingDistance;

            if ((_agent.remainingDistance <= _minDist)
                //&& (_agent.remainingDistance >= 0.001f)
                && isArrive == false)
            {
                isArrive = true;
                switch (CustomerState)
                {
                    case State.Init:
                        //isArrive = true;

                        //Debug.Log(_agent.remainingDistance);

                        _animator.SetBool("Walk", false);

                        transform.DORotate(new Vector3(0f, 180f, 0f), 0.4f).SetEase(Ease.Linear);

                        CustomerState = State.Wait;

                        break;

                    case State.Wait:

                        _animator.SetBool("Walk", false);
                        break;

                    case State.Order:

                        _animator.SetBool("Walk", false);
                        break;

                    case State.Move:
                        int _count = StackPos.childCount;

                        // delete
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        transform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.Linear);
                        for (int i = 0; i < _count; i++)
                        {
                            Transform _obj = StackPos.GetChild(0);
                            //_obj.SetParent(_chargingTable.StackPoint);
                            _obj.localPosition = new Vector3(0f, _obj.localPosition.y, 0f);
                            _obj.localEulerAngles = new Vector3(0f, 45f, 0f);
                        }
                        _animator.SetBool("Walk", false);
                        _animator.SetBool("Pick", false);
                        //_animator.SetBool("Charge", true);



                        CustomerState = State.Seat;
                        _room.SeatCustomer();
                        break;

                    case State.Seat:

                        //if (_productStack.Count > 0)
                        //{

                        //    Current_ChargingTime += Time.deltaTime;
                        //    if (Current_ChargingTime >= /*Total_ChargingTime*/ _chargInterval)
                        //    {
                        //        //_chargingTable.SpawnMoney();
                        //        Current_ChargingTime = 0;
                        //        _productStack.Pop().gameObject.SetActive(false);
                        //    }
                        //}
                        //else
                        //{
                        //    isArrive = true;

                        //    _animator.SetBool("Walk", true);
                        //    _animator.SetBool("Charge", false);

                        //    DOTween.Sequence().AppendInterval(0.5f).OnComplete(() =>
                        //    CustomerState = State.Exit);

                        //}
                        break;

                    case State.Exit:

                        StackPos.localPosition = Init_StackPointPos;
                        //StageManager.List_Humans.Remove(this);
                        Managers.Pool.Push(this.GetComponent<Poolable>());
                        break;
                }


            }
        }
    }


    public void PushProduct(CinemaProduct _product, float _interval = 0.5f)
    {
        _animator.SetBool("Pick", true);
        DOTween.Kill(_product.transform);
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        _product.transform.SetParent(StackPos);
        OrderCount--;
        _productStack.Push(_product);
        _product.transform.DOLocalJump(Vector3.up * ((_productStack.Count - 1) * Stack_Interval), 10, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), _interval).SetEase(Ease.Linear))
                                         .OnComplete(() =>
                                         {

                                             _product.transform.localEulerAngles = Vector3.zero;


                                         });
    }

    public void SetExit()
    {
        _animator.SetBool("Pick", false);

        isArrive = false;
        SetDest(_cinemaManager._doors[1].position);
        Managers.Pool.Push(_productStack.Pop().GetComponent<Poolable>());


        CustomerState = State.Exit;



    }



}
