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
    [TitleGroup("Product")] public float _pickInterval = 0.5f;
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
                {
                    PushProduct(_machine._productStack.Pop());
                    _animator.SetBool("Pick", true);
                }


                break;

            case "Counter":
                //Debug.Log("Cor Counter");
                Counter _counter = other.GetComponent<Counter>();
                if (_productStack.Count > 0 && isReady)
                {
                    int _num = (int)_productStack.Peek()._cinemaProductType;

                    _counter.PushProduct(PopProduct());
                    DOTween.Sequence(isReady = false).AppendInterval(_pickInterval).OnComplete(() => isReady = true);

                }

                break;

            case "TrashCan":

                if (_productStack.Count > 0)
                {
                    int _count = _productStack.Count;
                    for (int i = 0; i < _count; i++)
                    {
                        Transform _trans = _productStack.Pop().transform;
                        _trans.SetParent(other.transform);
                        _trans.DOLocalJump(Vector3.zero, 5f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() => Managers.Pool.Push(_trans.GetComponent<Poolable>()));
                    }
                }

                if (_productStack.Count < 1)
                {
                    _animator.SetBool("Pick", false);
                }

                break;

            default:

                break;


        }
    }


    void PushProduct(CinemaProduct _product)
    {
        DOTween.Kill(_product.transform);

        _stackY = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

        _productStack.Push(_product);
        _product.transform.SetParent(_stackPos);
        isReady = false;
        _product.transform.DOLocalJump(Vector3.up * (_productStack.Count - 1) * _stackY, _jumpPower, 1, _moveSpeed)
            .OnComplete(() =>
            {
                isReady = true;
                _product.transform.localEulerAngles = Vector3.zero;


            });

    }

    CinemaProduct PopProduct()
    {

        CinemaProduct _product = _productStack.Pop();

        if (_productStack.Count < 1)
        {
            _animator.SetBool("Pick", false);
        }

        return _product;

    }
}
