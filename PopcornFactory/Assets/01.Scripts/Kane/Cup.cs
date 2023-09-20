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

        //_labotoryManager = Managers.Game._labotoryManager;

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
        Debug.Log("NextPos");

        if (_cupPosNum <= _cupPos.Length)
            transform.position = _cupPos[_cupPosNum].position;


        Debug.Log(_cupPosNum);
        _cupPosNum++;


    }

    public void NextNode(Transform _obj, int _num = 0)
    {
        //_obj.SetParent(_labotoryManager.transform);
        StartCoroutine(Cor_NextNode());
        IEnumerator Cor_NextNode()
        {

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (i > 4) _num = 3;
                switch (_num)
                {
                    case 1:
                        _obj.DOMove(_nodes[i].transform.position + _nodes[i].transform.right * -1f + _nodes[i].transform.forward * Random.Range(-0.3f, 0.3f), _moveSpeed).SetEase(Ease.Linear);
                        break;

                    case 2:
                        _obj.DOMove(_nodes[i].transform.position + _nodes[i].transform.right * 1f + _nodes[i].transform.forward * Random.Range(-0.3f, 0.3f), _moveSpeed).SetEase(Ease.Linear);
                        break;

                    default:
                        _obj.DOMove(_nodes[i].transform.position, _moveSpeed).SetEase(Ease.Linear);

                        break;
                }
                //_obj.DORotateQuaternion(_nodes[i].transform.rotation, _moveSpeed).SetEase(Ease.Linear);
                yield return new WaitForSeconds(_moveSpeed);
            }

            //_labotoryManager.PushProduct(_obj, 0.5f, _obj.GetComponent<Product>()._productType);
            _obj.DOJump(transform.GetChild(0).position, _jumpPower, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            Managers.Pool.Push(_obj.GetComponent<Poolable>()));
        }
    }


}
