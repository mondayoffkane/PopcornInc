using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class Cup : MonoBehaviour
{
    //public List<Product> _productList = new List<Product>();
    public float _jumpPower = 10f;

    [SerializeField] Transform _popcornCup;
    [SerializeField] GameObject _posObj;



    public int _currentCount = 0;
    public int _MaxCount = 100;

    [SerializeField] Text _GuageText;
    [SerializeField] Image _GuageImg;

    [SerializeField] Transform _cornObj;

    public float _guageMin, _guageMax;
    public float _cornMin, _cornMax;

    [SerializeField] float _value;


    public bool isMoveCup = false;
    public Material _beltMat;

    float _z;
    // ================================


    private void Start()
    {
        transform.GetChild(1).gameObject.SetActive(false);

        _popcornCup = transform.Find("PopcornCup");
        _posObj = transform.Find("Pos").gameObject;

        _GuageText = transform.Find("Canvas").Find("Panel").Find("GuageText").GetComponent<Text>();
        _GuageImg = transform.Find("Canvas").Find("Panel").Find("Guage").Find("GuageImg").GetComponent<Image>();

        SetPopcornPos();
    }


    public void PushProduct(Transform _trans, float _moveInterval = 0.5f)
    {
        _trans.SetParent(transform);
        _trans.DOLocalJump(_popcornCup.localPosition, _jumpPower, 1, _moveInterval).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                //_productList.Add(_trans.GetComponent<Product>());
                if (isMoveCup)
                    _currentCount++;
                Managers.Pool.Push(_trans.GetComponent<Poolable>());
            });

        Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);
        //Managers.Game.Money += _trans.GetComponent<Product>()._price;

        Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);

        if (isMoveCup)
            SetPopcornPos();
    }

    [Button]
    public void SetPopcornPos()
    {
        _value = (float)((float)_currentCount / (float)_MaxCount);
        _GuageText.text = $"{_value * 100f}%";
        _GuageImg.rectTransform.offsetMin = new Vector2(16.5f, 17f);
        _GuageImg.rectTransform.offsetMax = new Vector2(-16.5f, -317f + 300f * _value);

        _cornObj.DOLocalMoveY((-8f + 9f * _value), 0.2f).SetEase(Ease.Linear);
        _cornObj.DOScale(Vector3.one * (0.5f + 0.5f * _value), 0.2f).SetEase(Ease.Linear);

        if (_currentCount >= _MaxCount)
        {
            SellProducts();
        }
    }


    [Button]
    public void SellProducts()
    {
        if (isMoveCup)
        {
            isMoveCup = false;
            _beltMat.DOOffset(Vector2.zero, 0f);

            _z = transform.localPosition.z;

            DOTween.Sequence()
                .Append(transform.DOLocalMoveZ(_z - 15, 1f).SetEase(Ease.Linear))
                .Join(_beltMat.DOOffset(new Vector2(0f, -2f), 1f))
                .AppendCallback(() =>
                {
                    _currentCount = 0;
                    SetPopcornPos();
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _z + 15f);

                })
                .Append(transform.DOLocalMoveZ(_z, 1f).SetEase(Ease.Linear))
            .Join(_beltMat.DOOffset(new Vector2(0f, -4f), 1f))
            .OnComplete(() => isMoveCup = true);

        }




        // add fuunc
    }



}
