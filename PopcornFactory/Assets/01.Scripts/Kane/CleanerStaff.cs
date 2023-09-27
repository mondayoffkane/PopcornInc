using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerStaff : CinemaStaff
{


    public float _currentTerm = 0f;
    public float _maxTerm = 1f;


    public float _sleepTime = 10f;

    [SerializeField] Room _targetRoom;





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
                    //_target.GetComponent<CleanObject>().RoomClear(true);


                    //for (int i = 0; i < _targetRoom._cleanObjects.Length; i++)
                    //{
                    //    if (_targetRoom._cleanObjects[i].isClean == false)
                    //    {
                    //        _targetRoom = _cinemaManager._roomList[i];
                    //        SetDest(_targetRoom._cleanObjects[i].transform.position);
                    //        _target = _targetRoom._cleanObjects[i].transform;
                    //        _staffState = CinemaStaffState.Move;
                    //        break;
                    //    }
                    //    else
                    //    {
                    SetDest(_waitPos);

                    //add _animator.SetBool("Cleaning", false);
                    _staffState = CinemaStaffState.Wait;
                    _currentTerm = 0f;
                    _target = null;
                    break;

                    //}
                    //}


                }
                break;

        }


    }


    void FindWork()
    {
        int _num = Random.Range(0, 2);
        if (_num == 1)
        {
            for (int i = 0; i < _cinemaManager._roomList.Count; i++)
            {
                if (_cinemaManager._roomList[i]._isUnlock)
                {
                    for (int j = 0; j < _cinemaManager._roomList[i]._cleanObjects.Length; j++)
                    {
                        if (_cinemaManager._roomList[i]._cleanObjects[j].isClean == false)
                        {
                            _targetRoom = _cinemaManager._roomList[i];
                            SetDest(_cinemaManager._roomList[i]._cleanObjects[j].transform.position);
                            _target = _cinemaManager._roomList[i]._cleanObjects[j].transform;
                            _staffState = CinemaStaffState.Move;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            StartCoroutine(Cor_Sleep());
        }


        IEnumerator Cor_Sleep()
        {
            SetDest(_waitPos);
            _staffState = CinemaStaffState.Sleep;
            yield return new WaitForSeconds(_sleepTime);
            _staffState = CinemaStaffState.Wait;

        }
    }




}
