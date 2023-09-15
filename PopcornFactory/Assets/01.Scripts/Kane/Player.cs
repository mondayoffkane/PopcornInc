using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;



public class Player : MonoBehaviour
{

    [TitleGroup("Product")] public float _jumpPower = 10f;
    [TitleGroup("Product")] public float _moveSpeed = 1f;
    [TitleGroup("Product")] public Transform _stackPos;
    [TitleGroup("Product")] public int _maxCount = 5;
    public bool isReady = true;
    [SerializeField] float _stackY = 0.5f;

    //public Vector3 _offset;

    [ShowInInspector] public Stack<CinemaProduct> _productStack;


    public enum PlayerState
    {
        Idle,
        Walk,
        Pick,
        PickWalk

    }
    public PlayerState _playerState;

    Animator _animator;

    // ======================================

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();

        _productStack = new Stack<CinemaProduct>();

        if (_stackPos == null) _stackPos = transform.Find("StackPos");

    }


    void Update()
    {

    }



    private void OnTriggerStay(Collider other)
    {

        switch (other.tag)
        {
            case "CinemaMachine":

                CinemaMachine _machine = other.GetComponent<CinemaMachine>();
                if (_machine._productStack.Count > 0 && _productStack.Count < _maxCount && isReady)
                    PushProduct(_machine._productStack.Pop());



                break;



            default:

                break;


        }
    }


    void PushProduct(CinemaProduct _product)
    {
        DOTween.Kill(_product.transform);


        _productStack.Push(_product);
        _product.transform.SetParent(_stackPos);
        isReady = false;
        _product.transform.DOLocalJump(Vector3.up * _productStack.Count, _jumpPower, 1, _moveSpeed)
            .OnComplete(() =>
            {
                isReady = true;
                _product.transform.localEulerAngles = Vector3.zero;


            });

    }

    void PopProduct(Transform _trans)
    {

    }
}
