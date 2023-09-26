using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.AI;
//using static UnityEditor.Recorder.OutputPath;


public class CinemaStaff : MonoBehaviour
{
    public Transform _target;
    public float _minDist = 0.5f;
    public CinemaManager _cinemaManager;
    public bool isArrive = false;
    public float Current_ChargingTime = 0f;

    public int _productType;
    public int OrderCount = 5;
    public Transform StackPos;
    public float Stack_Interval = 0.2f;
    public float BaseUp_Interval = 0.5f;

    public Stack<CinemaProduct> _productStack = new Stack<CinemaProduct>();

    public Vector3 _waitPos;
    [SerializeField] Vector3 Init_StackPointPos;

    public enum CinemaStaffState
    {
        //OrderCheck,
        Wait,
        Move,
        Pick,
        PickMove,
        Cleaning,
        Sleep
    }
    public CinemaStaffState _staffState;


    public enum CinemaStaffType
    {
        Counter,
        Cleaner
    }
    public CinemaStaffType _staffType;

    public Animator _animator;
    public NavMeshAgent _agent;

    //private void Start()
    //{
    //    StackPos = transform.Find("StackPos");
    //    Init_StackPointPos = StackPos.transform.localPosition;
    //}

    public virtual void SetInit(CinemaManager _cinemamanager, CinemaStaffType _stafftype, Vector3 _waitpos)
    {
        StackPos = transform.Find("StackPos");
        Init_StackPointPos = StackPos.transform.localPosition;


        _cinemaManager = _cinemamanager;

        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _staffType = _stafftype;

        _staffState = CinemaStaffState.Wait;

        _agent.Warp(_waitpos);
        _waitPos = _waitpos;
    }

    public void SetDest(Vector3 _destiny)
    {

        _animator.SetBool("Walk", true);
        _agent.destination = _destiny;
        isArrive = false;

    }



}
