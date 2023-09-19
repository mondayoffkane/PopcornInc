using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
//using UnityEngine.UI;

public class Room : UnlockObj
{
    public bool isReady = true;


    public Transform[] _seats;
    public int _currentCount = 0;
    //public int _seatCount = 0;

    public List<Customer> _customerList;

    public float _moviePlayTime = 5f;


    public GameObject _screen;



    // ================================
    private void Start()
    {
        _customerList = new List<Customer>();

        if (_screen == null) _screen = transform.Find("Canvas").Find("Screen").gameObject;
        _screen.SetActive(false);

        if (_seats == null)
            SetSeat();
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

    //public bool isEmpty()
    //{
    //    if (_currentCount >= _seats.Length)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }

    //}

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
        PopRoom();
    }

    [Button]
    public void PopRoom()
    {
        foreach (Customer _customer in _customerList)
        {
            _customer.SetExit();
        }

        _customerList.Clear();
        isReady = true;
        _currentCount = 0;

    }



}
