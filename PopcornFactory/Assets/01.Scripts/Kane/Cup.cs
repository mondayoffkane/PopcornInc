using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MondayOFF;


public class Cup : MonoBehaviour
{

    public float _jumpPower = 10f;

    [SerializeField] Transform _popcornCup;


    public int _cupPosNum = 0;
    public Transform[] _cupPos;
    public bool isfirst = false;

    public bool isRail = false;
    public Transform[] _nodes;
    public float _moveSpeed = 0.5f;


    //LabotoryManager _labotoryManager;
    private void Start()
    {
        transform.GetChild(1).gameObject.SetActive(false);

        _popcornCup = transform.Find("PopcornCup");

        foreach (Transform _node in _nodes)
        {
            _node.GetComponent<Renderer>().enabled = false;
        }
    }


    public void PushProduct(Transform _trans, float _moveInterval = 0.5f)
    {
        if (isRail == false)
        {
            _trans.SetParent(transform);
            _trans.DOLocalJump(_popcornCup.localPosition, _jumpPower, 1, _moveInterval).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Managers.Pool.Push(_trans.GetComponent<Poolable>());
                });

            Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);

            Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);
        }
        else
        {
            NextNode(_trans, Random.Range(0, 3));
            Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);
            Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);
        }


    }


    [Button]
    public void NextPos()
    {

        if (_cupPosNum <= _cupPos.Length)
        {
            transform.position = _cupPos[_cupPosNum].position;
            transform.rotation = _cupPos[_cupPosNum].rotation;
        }

        _cupPosNum++;


    }

    public void NextNode(Transform _obj, int _num = 0)
    {
        DOTween.Sequence().Append(_obj.DOJump(_nodes[0].transform.position + _nodes[0].transform.right * Random.Range(-1f, 1f) + _nodes[0].transform.forward * Random.Range(-0.3f, 0.3f), _jumpPower, 1, 0.5f).SetEase(Ease.Linear))
            .Append(_obj.DOMove(_nodes[1].transform.position, _moveSpeed).SetEase(Ease.Linear))
            .OnComplete(() => _obj.DOJump(_cupPos[_cupPos.Length - 1].position, _jumpPower, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Managers.Pool.Push(_obj.GetComponent<Poolable>());
                Managers.Game._stageManager.SaveRecipe((int)_obj.GetComponent<Product>()._productType);
            }));

    }





}
