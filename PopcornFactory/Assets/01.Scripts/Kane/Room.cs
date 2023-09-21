using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
//using UnityEngine.UI;

public class Room : EventObject
{
    public bool alwaysOpen = false;
    public bool isReady = true;

    [SerializeField] Transform _seatGroup;
    public Transform[] _seats;
    public int _currentCount = 0;
    //public int _seatCount = 0;

    public List<Customer> _customerList;

    public float _moviePlayTime = 5f;


    public GameObject _screen;


    public bool _isUnlock = false;
    public GameObject _offObj;

    public Transform _cleanGroup;
    public CleanObject[] _cleanObjects;
    [SerializeField] int _cleanObjectCount;
    [SerializeField] int _cleanCount = 0;

    // ================================
    private void Start()
    {
        _customerList = new List<Customer>();

        if (_screen == null) _screen = transform.Find("Canvas").Find("Screen").gameObject;
        _screen.SetActive(false);

        _seatGroup = transform.Find("SeatGroup");
        _cleanGroup = transform.Find("CleanGroup");


        if (_seats == null)
            SetSeat();

        _unlockEvent.AddListener(() => Unlock());


        LoadData();
        _cleanObjectCount = _cleanGroup.childCount;
        _cleanObjects = new CleanObject[_cleanObjectCount];

        for (int i = 0; i < _cleanObjectCount; i++)
        {
            _cleanObjects[i] = _cleanGroup.GetChild(i).GetComponent<CleanObject>();
            _cleanObjects[i]._room = this;
        }

    }

    void LoadData()
    {
        if (alwaysOpen)
        {
            _isUnlock = true;
            //ES3.Save<bool>("isOpen", isOpen);
        }


        //isOpen = ES3.Load<bool>("isOpen", false);
        if (_isUnlock)
        {
            isReady = true;
            transform.localScale = Vector3.one;
        }
        else
        {
            isReady = false;
            transform.localScale = Vector3.zero;
        }
    }

    [Button]
    public void SetSeat()
    {
        _seats = new Transform[transform.childCount];

        for (int i = 0; i < _seats.Length - 1; i++)
        {
            _seats[i] = transform.GetChild(i);
        }
    }


    public Vector3 EmptySeat(Customer customer)
    {

        _customerList.Add(customer);
        if (_customerList.Count >= _seats.Length) isReady = false;
        return _seats[_customerList.Count - 1].position;
    }

    public void SeatCustomer()
    {
        _currentCount++;
        if (_currentCount >= _seats.Length)
        {
            StartCoroutine(Cor_PlayMovie());
        }
    }

    IEnumerator Cor_PlayMovie()
    {
        _screen.SetActive(true);
        yield return new WaitForSeconds(_moviePlayTime);
        _screen.SetActive(false);
        EndMovie();
    }

    [Button]
    public void EndMovie()
    {
        foreach (Customer _customer in _customerList)
        {
            _customer.SetExit();
        }

        _customerList.Clear();
        _currentCount = 0;

        foreach (CleanObject _obj in _cleanObjects)
        {
            _obj.RoomClear(false);
        }

        //isReady = true;

    }

    public void Unlock()
    {
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        _isUnlock = true;
        isReady = true;
        _offObj.SetActive(false);
    }

    public void ClearObj(int _num = 0)
    {
        _cleanCount++;


        if (_cleanCount >= _cleanObjectCount)
        {
            isReady = true;
        }


    }


}
