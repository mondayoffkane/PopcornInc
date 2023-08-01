using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Table : MonoBehaviour
{
    public float _jumpPower = 10f;

    public List<Product> _productList = new List<Product>();

    [SerializeField] Machine _machine;


    private void OnEnable()
    {
        _machine = GetComponentInParent<Machine>();

    }

    public void PushProduct(Transform _trans, float _moveInterval = 0.5f, bool notjump = false)
    {
        _trans.SetParent(transform.parent);
        if (notjump)
        {
            _trans.DOLocalMove(new Vector3(0f, 0f, 0f), _moveInterval).SetEase(Ease.Linear)
               .OnComplete(() =>
               {
                   Managers.Pool.Push(_trans.GetComponent<Poolable>());
               });
        }
        else
        {

            _trans.DOLocalJump(new Vector3(0f, 0f, 0f), _jumpPower, 1, _moveInterval).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Managers.Pool.Push(_trans.GetComponent<Poolable>());
                });
        }

        Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);
        Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);

        _machine._currentCount++;


    }
}
