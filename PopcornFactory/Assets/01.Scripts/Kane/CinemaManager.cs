using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CinemaManager : MonoBehaviour
{

    public Transform _waitingPos;



    [TitleGroup("Customer")] public List<Customer> _customerList;


    private void Start()
    {
        if (_waitingPos == null) _waitingPos = transform.Find("WatingPos");

        _customerList = new List<Customer>();
    }



    private void Update()
    {

        //_customerList.co



    }






}
