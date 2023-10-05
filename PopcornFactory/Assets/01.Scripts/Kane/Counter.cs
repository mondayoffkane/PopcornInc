using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Counter : EventObject
{
    public double[] _unlockPrices;


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

    public GameObject _staff;
    public float _stackY;
    //public GameObject _playerZone;

    //[TitleGroup("Money")][SerializeField] GameObject _moneyPref;
    [TitleGroup("Money")] public Transform _moneyStackPos;
    //[TitleGroup("Money")]
    //[SerializeField] Vector3 _stackInterval = Vector3.zero;
    //[TitleGroup("Money")] public Stack<Transform> _moneyStack;
    //[TitleGroup("Money")] int _width = 3, _height = 5;



    [TitleGroup("Order UI")] public GameObject _orderPanel;
    [TitleGroup("Order UI")] public Sprite[] _sprites;
    [TitleGroup("Order UI")] public Text _text;

    [SerializeField] public Image _orderImg;


    [SerializeField] int _firstCount = 0;

    // ===========================================
    void Start()
    {
        _productStacks = new Stack<CinemaProduct>[3];
        for (int i = 0; i < _productStacks.Length; i++)
        {
            _productStacks[i] = new Stack<CinemaProduct>();
        }

        if (_cinemaManager == null) _cinemaManager = transform.parent.GetComponent<CinemaManager>();

        _staff.SetActive(false);

        // add load data

        StartCoroutine(Cor_Update());

        _unlockEvent.AddListener(() => Unlock());
        //_moneyPref = Resources.Load<GameObject>("Money_Pref");

        //_stackInterval = _moneyPref.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        //_moneyStack = new Stack<Transform>();
        if (_moneyStackPos == null) _moneyStackPos = transform.Find("MoneyStackPos");

        if (_moneyStackPos.GetComponent<MoneyZone>() == null) _moneyStackPos.gameObject.AddComponent<MoneyZone>();

        Order(false);
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
                        PopProduct();

                    }
                }
                yield return new WaitForSeconds(0.5f);

                if (_customer.OrderCount <= 0)
                {

                    int _count = 1;
                    if (_cinemaManager.FindCinema())
                    {

                        _firstCount++;
                        //if (_firstCount == 6 && TutorialManager._instance._tutorialLevel == 10)
                        //{
                        //    TutorialManager._instance.Tutorial(true, 8f);
                        //}


                        switch (_customer._productType)
                        {
                            case 0:
                                _count = 1;
                                break;
                            case 1:
                                _count = 5;
                                break;

                            case 2:
                                _count = 14;
                                break;
                        }
                        _moneyStackPos.GetComponent<MoneyZone>().PopMoney(transform, 5, _count);
                        _customer = null;
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    public void PopProduct()
    {
        if ((_customer != null) && _customer._productStack.Count < 1 && _productStacks[_customer._productType].Count > 0)
        {
            _customer.PushProduct(_productStacks[_customer._productType].Pop());
            //_customer = null;
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
        _stackY = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        _product.transform.DOLocalJump(Vector3.up * (_productStacks[_num].Count - 1) * _stackY, _jumpPower, 1, _moveSpeed)
            .OnComplete(() =>
            {
                _product.transform.localEulerAngles = Vector3.zero;

            });

    }

    public void Unlock()
    {
        isStaff = true;
        _staff.SetActive(true);
        //_playerZone.SetActive(false);
    }


    public void Order(bool isBool)
    {

        if (isBool)
        {
            _orderPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutCubic);
            _orderImg.sprite = _sprites[_customer._productType];
            _text.text = $"X {_customer.OrderCount}";
        }
        else
        {
            _orderPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
        }

    }



}
