using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Room : MonoBehaviour
{
    public bool isReady = true;


    public Transform[] _seats;
    public int currentCount = 0;


    private void Start()
    {

        SetSeat();
    }

    [Button]
    public void SetSeat()
    {
        _seats = new Transform[transform.childCount];

        for (int i = 0; i < _seats.Length; i++)
        {
            _seats[i] = transform.GetChild(i);
        }
    }

    public bool isEmpty()
    {
        if (currentCount >= _seats.Length)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public Vector3 Seat()
    {
        currentCount++;
        if (currentCount >= _seats.Length) isReady = false;
        return _seats[currentCount - 1].position;
    }





}
