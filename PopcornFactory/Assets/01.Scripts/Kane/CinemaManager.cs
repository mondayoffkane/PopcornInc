using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.ProBuilder.Shapes;
using DG.Tweening;

public class CinemaManager : MonoBehaviour
{
    [TitleGroup("Machine")] public CinemaMachine[] _cinemaMachines;


    [TitleGroup("Customer")] public GameObject _customerPref;
    //[TitleGroup("Customer")] public int _currentCustomerCount;
    [TitleGroup("Customer")] public int _maxCustomerCount = 5;
    [TitleGroup("Customer")] public List<Customer> _customerList;

    [TitleGroup("Room")] public List<Room> _roomList;
    [TitleGroup("Room")] public int _roomNum = 0;
    [TitleGroup("Room")] public double[] _roomUnlockPrices;
    [TitleGroup("Room")] public double[] _roomScope;
    [TitleGroup("Room")] public double[] _roomProfits;






    public Counter _counter;
    public Transform _waitingPos;
    public float _watingTerm = 1f;

    [TitleGroup("Counter")] public GameObject _orderPanel;
    [TitleGroup("Counter")] public Image _orderImg;
    [TitleGroup("Counter")] public Text _orderText;



    /// =============
    [Header("Serialized")]
    Transform _customerGroup;
    Transform[] _doors = new Transform[2];
    [SerializeField] int _loopCount = 0;

    // ================

    private void Start()
    {
        if (_waitingPos == null) _waitingPos = transform.Find("WatingPos");
        if (_counter == null) _counter = transform.Find("Cinema_Counter").GetComponent<Counter>();

        _doors[0] = transform.Find("Enter_Door");
        _doors[1] = transform.Find("Exit_Door");

        _customerList = new List<Customer>();
        _customerGroup = new GameObject().transform;
        _customerGroup.name = "CustomerGroup";

        //_orderPanel = transform.Find("OrderPanel").gameObject;
        //_orderImg = _orderPanel.transform.Find("OrderImg").GetComponent<Image>();
        //_orderText = _orderPanel.transform.Find("OrderText").GetComponent<Text>();


        _roomList = new List<Room>();

        Transform _roomGroup = transform.Find("RoomGroup");
        for (int i = 0; i < _roomGroup.childCount; i++)
        {
            _roomList.Add(_roomGroup.GetChild(i).GetComponent<Room>());
        }


        StartCoroutine(Cor_Update());
    }


    IEnumerator Cor_Update()
    {

        while (true)
        {

            if (_customerList.Count < _maxCustomerCount)
            {
                AddCustomer();
            }

            if (_customerList[0].CustomerState == Customer.State.Wait)
            {
                _customerList[0].CustomerState = Customer.State.Order;
                _counter._customer = _customerList[0];
                OrderOnOff(true, 0, 1); // 

            }

            yield return new WaitForSeconds(1f);
        }

    }

    void AddCustomer()
    {
        Customer _customer = Managers.Pool.Pop(_customerPref, _customerGroup.transform).GetComponent<Customer>();
        _customerList.Add(_customer);
        // need to modify
        _customer.SetInit(this, 0, 1);
        _customer.GetComponent<NavMeshAgent>().Warp(_doors[0].position);

        _customer.SetDest(_waitingPos.position + new Vector3(0f, 0f, _watingTerm) * (_customerList.Count - 1));


    }

    void OrderOnOff(bool isOn, int _type = 0, int _count = 0)
    {
        if (isOn)
        {
            //_orderPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
            // add 
            //_orderImg = 
        }
        else
        {
            //_orderPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
        }

    }


    public void FindCinema()
    {

        if (_loopCount >= _roomList.Count)
            return;

        if (_roomList[_roomNum].isReady)
        {
            _customerList[0].SetDest(_roomList[_roomNum].Seat());
            _customerList[0].CustomerState = Customer.State.Move;
            _customerList.RemoveAt(0);


            int _count = _customerList.Count;
            for (int i = 0; i < _count; i++)
            {
                _customerList[i].SetDest(_waitingPos.position + new Vector3(0f, 0f, _watingTerm) * i);
                _customerList[i].CustomerState = Customer.State.Wait;
            }
        }
        else
        {
            _roomNum++;
            if (_roomNum >= _roomList.Count) _roomNum = 0;
            _loopCount++;
            FindCinema();
        }


    }




}
