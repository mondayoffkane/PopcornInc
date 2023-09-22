using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerStaff : CinemaStaff
{


    public float _currentTerm = 0f;
    public float _maxTerm = 1f;






    private void Start()
    {

    }

    private void Update()
    {

        if (_agent.remainingDistance <= _minDist)
        {
            _animator.SetBool("Walk", false);
        }

        switch (_staffState)
        {
            case CinemaStaffState.Wait:

                FindWork();
                break;


            case CinemaStaffState.Move:
                if (_agent.remainingDistance <= _minDist)
                {
                    _staffState = CinemaStaffState.Cleaning;

                    //add _animator.SetBool("Cleaning", true);
                }

                break;

            case CinemaStaffState.Cleaning:
                _currentTerm += Time.deltaTime;
                _target.GetComponent<CleanObject>().Cleaning();
                if (_currentTerm >= _maxTerm)
                {
                    _target.GetComponent<CleanObject>().RoomClear(true);

                    SetDest(_waitPos);

                    //add _animator.SetBool("Cleaning", false);
                    _staffState = CinemaStaffState.Wait;
                    _currentTerm = 0f;
                    _target = null;
                }
                break;

        }


    }


    void FindWork()
    {
        for (int i = 0; i < _cinemaManager._roomList.Count; i++)
        {
            if (_cinemaManager._roomList[i]._isUnlock)
            {
                for (int j = 0; j < _cinemaManager._roomList[i]._cleanObjects.Length; j++)
                {
                    if (_cinemaManager._roomList[i]._cleanObjects[j].isClean == false)
                    {
                        SetDest(_cinemaManager._roomList[i]._cleanObjects[j].transform.position);
                        _target = _cinemaManager._roomList[i]._cleanObjects[j].transform;
                        _staffState = CinemaStaffState.Move;
                        break;
                    }
                }
            }
        }
    }




}
