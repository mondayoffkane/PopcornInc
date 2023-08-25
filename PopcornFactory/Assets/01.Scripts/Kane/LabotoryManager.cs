using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LabotoryManager : MonoBehaviour
{

    public float _jumpPower = 5f;


    public void PushProduct(Transform _trans, float _moveInterval = 0.5f)
    {

        _trans.SetParent(transform);
        _trans.DOLocalJump(Vector3.zero, _jumpPower, 1, _moveInterval).SetEase(Ease.Linear)
            .OnComplete(() =>
            {

                Managers.Pool.Push(_trans.GetComponent<Poolable>());
            });

        //Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);
        //Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);


    }

}
