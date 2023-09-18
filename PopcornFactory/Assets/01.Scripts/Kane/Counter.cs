using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [TitleGroup("Product")] public float _jumpPower = 10f;
    [TitleGroup("Product")] public float _moveSpeed = 0.2f;
    [TitleGroup("Product")] public Transform _stackPos;
    [TitleGroup("Product")] public int _maxCount = 5;

    public Transform[] _stackPoses;

    [ShowInInspector]
    public Stack<CinemaProduct>[] _productStacks;

    public bool isPlayerIn = false;
    public bool isStaff = false;

    public Customer _customer;


    CinemaManager _cinemaManager;

    // ============================
    void Start()
    {
        _productStacks = new Stack<CinemaProduct>[3];
        for (int i = 0; i < _productStacks.Length; i++)
        {
            _productStacks[i] = new Stack<CinemaProduct>();
        }

        if (_cinemaManager == null) _cinemaManager = transform.parent.GetComponent<CinemaManager>();

        StartCoroutine(Cor_Update());
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            if (_customer != null)
            {
                if (isPlayerIn || isStaff)
                {

                    if (_customer.CustomerState == Customer.State.Order && _customer.OrderCount > 0 && _productStacks[_customer._productType].Count > 0)
                    {
                        _customer.PushProduct(_productStacks[_customer._productType].Pop());
                    }
                }
                yield return new WaitForSeconds(0.5f);

                if (_customer.OrderCount <= 0)
                {
                    _cinemaManager.FindCinema();

                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

    public void PushProduct(CinemaProduct _product)
    {
        DOTween.Kill(_product.transform);

        int _num = (int)_product._cinemaProductType;

        _productStacks[_num].Push(_product);
        _product.transform.SetParent(_stackPoses[_num]);

        _product.transform.DOLocalJump(Vector3.up * _productStacks[_num].Count, _jumpPower, 1, _moveSpeed)
            .OnComplete(() =>
            {

                _product.transform.localEulerAngles = Vector3.zero;


            });

    }




}
