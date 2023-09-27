using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CounterStaff : CinemaStaff
{


    public float _currentTerm = 0f;
    public float _maxTerm = 1f;

    Counter _counter;

    public override void SetInit(CinemaManager _cinemamanager, CinemaStaffType _stafftype, Vector3 _waitpos)
    {
        base.SetInit(_cinemamanager, _stafftype, _waitpos);
        _counter = _cinemamanager._counter;
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
                    if (_target.GetComponent<CinemaMachine>()._productStack.Count > 0)
                    {
                        PushProduct(_target.GetComponent<CinemaMachine>()._productStack.Pop());


                        _staffState = CinemaStaffState.PickMove;
                        SetDest(_waitPos);

                        _currentTerm = 0f;
                        _target = _counter.transform;
                    }
                }
                break;


            case CinemaStaffState.PickMove:
                if (_agent.remainingDistance <= _minDist)
                {
                    if (_productStack.Count > 0)
                    {
                        _counter.PushProduct(_productStack.Pop());
                        _counter.PopProduct();

                        _animator.SetBool("Pick", false);

                        _staffState = CinemaStaffState.Wait;
                        _target = null;
                    }
                }
                break;

        }


    }


    void CheckOrder()
    {
        if (_counter._customer != null)
        {

            if (_counter._productStacks[_counter._customer._productType].Count > 0)
            {

                _counter.PopProduct();
                _staffState = CinemaStaffState.Wait;
                _target = null;

            }
            else if (_counter._customer._productStack.Count < 1)
            {


                SetDest(_cinemaManager._cinemaMachines[_counter._customer._productType]._checkZone.position);
                _target = _cinemaManager._cinemaMachines[_counter._customer._productType].transform;
                _staffState = CinemaStaffState.Move;

            }

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
