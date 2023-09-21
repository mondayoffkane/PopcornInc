using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CounterStaff : CinemaStaff
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

                CheckOrder();
                break;


            case CinemaStaffState.Move:
                if (_agent.remainingDistance <= _minDist)
                {
                    _staffState = CinemaStaffState.Pick;

                    //add _animator.SetBool("Cleaning", true);
                }

                break;



            case CinemaStaffState.Pick:
                _currentTerm += Time.deltaTime;
                if (_currentTerm >= _maxTerm && _productStack.Count < 1)
                {
                    PushProduct(_target.GetComponent<CinemaMachine>()._productStack.Pop());


                    _staffState = CinemaStaffState.PickMove;
                    SetDest(_waitPos);

                    _currentTerm = 0f;
                    _target = _cinemaManager._counter.transform;
                }
                break;

            case CinemaStaffState.PickMove:
                if (_agent.remainingDistance <= _minDist)
                {
                    _cinemaManager._counter.PushProduct(_productStack.Pop());
                    _cinemaManager._counter.PopProduct();

                    _animator.SetBool("Pick", false);

                    _staffState = CinemaStaffState.Wait;
                    _target = null;
                }
                break;

        }


    }


    void CheckOrder()
    {
        if (_cinemaManager._counter._customer != null)
        {

            SetDest(_cinemaManager._cinemaMachines[_cinemaManager._counter._customer._productType]._checkZone.position);
            _target = _cinemaManager._cinemaMachines[_cinemaManager._counter._customer._productType].transform;
            _staffState = CinemaStaffState.Move;


        }
    }

    public void PushProduct(CinemaProduct _product, float _interval = 0.5f)
    {
        _animator.SetBool("Pick", true);
        DOTween.Kill(_product.transform);
        Stack_Interval = _product.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        _product.transform.SetParent(StackPos);

        _product.transform.DOLocalJump(Vector3.up * (_productStack.Count * Stack_Interval), 1, 1, _interval).SetEase(Ease.Linear)
                                         .Join(_product.transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), _interval).SetEase(Ease.Linear))
                                         .OnComplete(() =>
                                         {
                                             _productStack.Push(_product);

                                             _product.transform.localEulerAngles = Vector3.zero;


                                         });
    }


}
