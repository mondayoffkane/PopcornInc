using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    public Table _table;

    public bool isRail = false;
    public Transform _railGroup;
    public float _moveSpeed = 0.5f;
    public float _railJumpPower = 0f;
    public Machine _nextMachine;
    public Cup _cup;

    public double _upgradeBase = 1d, _productBase = 1d;
    public double _upgradeScope = 0.5d, _productScope = 0.5d;
    public int _maxLevel = 150;
    public double[] _upgradePrice;
    public double[] _productPrice;

    public int _level;
    public string _name;

    Transform _panel;
    GameObject _popupButton;
    Text _levelText, _productText, _product_PriceText, _intervalText, _upgrade_PriceText;
    Button _upgradeButton;
    Image _guageImg;


    GameManager _gamemanager;

    public bool isPress = false;

    public float _interval = 0.5f;
    public float _jumpPower = 5f;

    public GameObject _popcorn_Pref;


    float _x, _z;

    public Product.ProductType _machineType;

    public bool isAutoSpawn = false;

    public int _currentCount = 0;

    public Transform _spawnPos;
    public float _term = 0.1f;
    float _currentterm = 0.1f;

    public double _priceScope = 0.2d;
    public int _priceScopeLevel = 0;
    // ==========================================================================
    public int max_count = 20;
    private void OnEnable()
    {
        _x = _table.GetComponent<BoxCollider>().bounds.size.x * 0.5f;
        _z = _table.GetComponent<BoxCollider>().bounds.size.z * 0.5f;
        StartCoroutine(Cor_Update());


        // add get data

        //_productText.text = $"{_name}";
        _gamemanager = Managers.Game;
        LoadData();

    }

    private void Update()
    {
        if (isPress) // touch press
        {
            _currentterm -= Time.deltaTime;
            if (_currentterm <= 0)
            {
                _currentterm = _term;
                UpgradeMachine();
            }
        }
        else
        {
            _currentterm = _term;
        }
    }

    IEnumerator Cor_Update()
    {
        //WaitForSeconds _term = new WaitForSeconds(_interval);
        while (true)
        {
            yield return new WaitForSeconds(_interval);

            if (isAutoSpawn || _currentCount > 0)
            {

                if (isRail)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Transform _corn = Managers.Pool.Pop(_popcorn_Pref, _table.transform).transform;
                        _corn.GetComponent<Product>().SetType(_machineType, _productPrice[_level] * (1 + _priceScope * _priceScopeLevel));
                        if (_spawnPos != null)
                        {
                            _corn.position = _spawnPos.position;
                        }
                        else
                        {
                            _corn.position = transform.position;
                        }
                        NextNode(_corn, i);
                        _currentCount--;
                    }
                }
                else
                {
                    if (_table._productList.Count < max_count)
                    {
                        Managers.Sound.Play("Effect_6");
                        Transform _corn = Managers.Pool.Pop(_popcorn_Pref, _table.transform).transform;
                        _corn.GetComponent<Product>().SetType(_machineType, _productPrice[_level] * (1 + _priceScope * _priceScopeLevel));
                        if (_spawnPos != null)
                        {
                            _corn.position = _spawnPos.position;
                        }
                        else
                        {
                            _corn.position = transform.position;
                        }

                        _corn.DOLocalJump(new Vector3(Random.Range(-_x, _x), 0f, Random.Range(-_z, _z)), _jumpPower, 1, 1f)
                            .SetEase(Ease.Linear).OnComplete(() => _table._productList.Add(_corn.GetComponent<Product>()));

                        _currentCount--;
                    }
                }

            }

        }
    }


    [Button]
    public void CreateDataTable()
    {

        _upgradePrice = new double[_maxLevel];

        for (int i = 0; i < _upgradePrice.Length; i++)
        {
            if (i == 0)
            {
                _upgradePrice[i] = _upgradeBase;
            }
            else
            {
                _upgradePrice[i] = _upgradePrice[i - 1] + _upgradeScope * i + 1;
            }
        }

        _productPrice = new double[_maxLevel];
        for (int i = 0; i < _productPrice.Length; i++)
        {
            if (i == 0)
            {
                _productPrice[i] = _productBase;
            }
            else
            {
                _productPrice[i] = _productPrice[i - 1] * 0.5d + _productScope * i + 0.5d;
            }
        }

    }

    public void CheckPrice(Transform _Panel)
    {

        if (_panel == null)
        {
            _panel = _Panel; //transform.Find("Canvas").transform;
            //_popupButton = _panel.transform.Find("PopUp_Button").gameObject;
            _levelText = _panel.Find("LevelText").GetComponent<Text>();
            _productText = _panel.Find("ProductText").GetComponent<Text>();
            _product_PriceText = _panel.Find("Product_PriceText").GetComponent<Text>();
            _intervalText = _panel.Find("Interval_Img").Find("IntervalText").GetComponent<Text>();
            _upgradeButton = _panel.Find("Upgrade_Button").GetComponent<Button>();
            _upgrade_PriceText = _upgradeButton.transform.Find("Upgrade_PriceText").GetComponent<Text>();
            _guageImg = _panel.Find("Guage_Group").Find("GuageImg").GetComponent<Image>();


        }

        _levelText.text = $"Level  {(_level + 1).ToString()}";
        _productText.text = $"{_name}";
        _product_PriceText.text = $"{Managers.ToCurrencyString(_productPrice[_level])}";
        _intervalText.text = $"{_interval}s";

        _upgrade_PriceText.text = _level < (_maxLevel - 1)
            ? $"{Managers.ToCurrencyString(_upgradePrice[_level])}" : "Max";

        _guageImg.fillAmount = ((_level + 1) % 10) * 0.1f; //== 0 ? 1f : ((_level + 1) % 10) * 0.1f;

        if ((_gamemanager.Money >= _upgradePrice[_level]) && (_level < _maxLevel - 1))
        {
            _upgradeButton.interactable = true;
        }
        else
        {
            _upgradeButton.interactable = false;
        }


    }

    public void UpgradeMachine()
    {
        if (_gamemanager.Money >= _upgradePrice[_level] && (_level < _maxLevel - 1))
        {
            _gamemanager.CalcMoney(-_upgradePrice[_level]);

            _level++;
            if (_level >= _maxLevel - 1)
            {
                RailOnOff(true);
            }
            // add gem
            if ((_level + 1) % 10 == 0)
            {
                _gamemanager.CalcGem(1);
            }


            for (int i = 0; i < _table._productList.Count; i++)
            {
                _table._productList[i]._price = _productPrice[_level];
            }

            CheckPrice(_panel);
            _gamemanager._stageManager.CheckButton();
            Managers.Sound.Play("Money");
            SaveData();
        }
    }


    public void SaveData()
    {
        ES3.Save<int>(_name.ToString(), _level);
    }

    public void LoadData()
    {
        _level = ES3.Load<int>(_name.ToString(), 0);

        if (_level >= _maxLevel - 1)
        {
            RailOnOff(true);
        }
    }




    public void NextNode(Transform _obj, int _num = 0)
    {
        StartCoroutine(Cor_NextNode());
        IEnumerator Cor_NextNode()
        {

            switch (_num)
            {
                case 1:
                    _obj.DOJump(_railGroup.GetChild(0).position + _railGroup.GetChild(0).right * -1f + _railGroup.GetChild(0).forward * Random.Range(-0.3f, 0.3f), _railJumpPower, 1, _moveSpeed).SetEase(Ease.Linear);
                    break;

                case 2:
                    _obj.DOJump(_railGroup.GetChild(0).position + _railGroup.GetChild(0).right * 1f + _railGroup.GetChild(0).forward * Random.Range(-0.3f, 0.3f), _railJumpPower, 1, _moveSpeed).SetEase(Ease.Linear);
                    break;

                default:
                    _obj.DOJump(_railGroup.GetChild(0).position, _railJumpPower, 1, _moveSpeed).SetEase(Ease.Linear);

                    break;
            }
            yield return new WaitForSeconds(_moveSpeed);

            for (int i = 1; i < _railGroup.childCount; i++)
            {
                switch (_num)
                {
                    case 1:
                        _obj.DOMove(_railGroup.GetChild(i).position + _railGroup.GetChild(i).right * -1f + _railGroup.GetChild(i).forward * Random.Range(-0.3f, 0.3f), _moveSpeed).SetEase(Ease.Linear);
                        break;

                    case 2:
                        _obj.DOMove(_railGroup.GetChild(i).position + _railGroup.GetChild(i).right * 1f + _railGroup.GetChild(i).forward * Random.Range(-0.3f, 0.3f), _moveSpeed).SetEase(Ease.Linear);
                        break;

                    default:
                        _obj.DOMove(_railGroup.GetChild(i).position, _moveSpeed).SetEase(Ease.Linear);

                        break;
                }
                _obj.DORotateQuaternion(_railGroup.GetChild(i).rotation, _moveSpeed).SetEase(Ease.Linear);
                yield return new WaitForSeconds(_moveSpeed);
            }

            _table._productList.Remove(_obj.GetComponent<Product>());
            if (_nextMachine != null && _nextMachine.gameObject.activeSelf == true)
            {
                _nextMachine._table.PushProduct(_obj, _moveSpeed, true);
            }
            else
            {
                _cup.PushProduct(_obj);
            }
        }
    }

    [Button]
    public void RailOnOff(bool isOn)
    {
        isRail = isOn;
        _railGroup.gameObject.SetActive(isRail);
        if (isOn)
        {
            int _cnt = _table._productList.Count;
            for (int i = 0; i < _cnt; i++)
            {
                Product _product = _table._productList[0];
                _table._productList.RemoveAt(0);
                Managers.Pool.Push(_product.GetComponent<Poolable>());
            }

        }

    }
}
