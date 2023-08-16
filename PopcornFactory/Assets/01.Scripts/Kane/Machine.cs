using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    public int _level;
    [FoldoutGroup("UI_Description_1")] public string _name;
    [FoldoutGroup("UI_Description_1")] public Sprite _upgrade1_sprite;
    [FoldoutGroup("UI_Description_1")] public string _upgrade1_name = "Income";
    [FoldoutGroup("UI_Description_1")] public string _upgrade1_explain = "Income Up";
    [FoldoutGroup("UI_Description_1")] public Sprite _upgrade2_sprite;
    [FoldoutGroup("UI_Description_1")] public string _upgrade2_name = "Speed";
    [FoldoutGroup("UI_Description_1")] public string _upgrade2_explain = "Speed Up";

    [FoldoutGroup("UI_Description_2")] public double _scrollUpgrade1_base = 500;
    [FoldoutGroup("UI_Description_2")] public double _scrollUpgrade2_base = 700;
    [FoldoutGroup("UI_Description_2")] public double _scrollUpgrade1_scope = 1.2d;
    [FoldoutGroup("UI_Description_2")] public double _scrollUpgrade2_scope = 1.5d;
    [FoldoutGroup("UI_Description_2")] public double[] _scrollUpgrade1_Price;
    [FoldoutGroup("UI_Description_2")] public double[] _scrollUpgrade2_Price;
    [FoldoutGroup("UI_Description_2")] public int _priceScopeLevel;
    [FoldoutGroup("UI_Description_2")] public int _spawnLevel;

    public int _machineNum = 0;
    public Mesh _productMesh;
    public Material _productMat;

    public double _priceScope = 0.2d;

    public float _interval = 0.5f;

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





    Transform _panel;
    GameObject _popupButton;
    Text _levelText, _productText, _product_PriceText, _intervalText, _upgrade_PriceText;
    Button _upgradeButton;
    Image _guageImg;


    GameManager _gamemanager;

    public bool isPress = false;


    public float _jumpPower = 5f;

    public GameObject _popcorn_Pref;


    float _x, _z;

    //public Product.ProductType _machineType;

    public bool isAutoSpawn = false;

    public int _currentCount = 0;

    public Transform _spawnPos;
    public float _term = 0.1f;
    float _currentterm = 0.1f;

    //public int _machineNum = -1;

    // ==========================================================================
    public int max_count = 100;
    //private void OnEnable()
    //{
    //    Init();

    //}

    public void Init()
    {
        _x = _table.GetComponent<BoxCollider>().bounds.size.x * 0.5f;
        _z = _table.GetComponent<BoxCollider>().bounds.size.z * 0.5f;
        StartCoroutine(Cor_Update());

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
            _interval = 1f - (0.05f * _spawnLevel);
            yield return new WaitForSeconds(_interval);

            if (isAutoSpawn || _currentCount > 0)
            {
                //Debug.Log(transform.name);
                if (isRail)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Transform _corn = Managers.Pool.Pop(_popcorn_Pref, _table.transform).transform;
                        _corn.GetComponent<Product>().SetType(_machineNum, _productMesh, _productMat, _productPrice[_level] * (1 + _priceScope * _priceScopeLevel));
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
                        for (int i = 0; i < (_level / 20) + 1; i++)
                        {
                            Managers.Sound.Play("Effect_6");
                            Transform _corn = Managers.Pool.Pop(_popcorn_Pref, _table.transform).transform;
                            _corn.GetComponent<Product>().SetType(_machineNum, _productMesh, _productMat, _productPrice[_level] * (1 + _priceScope * _priceScopeLevel));
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

                        }
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
                _productPrice[i] = _productBase + _productPrice[i - 1] * 0.5d + _productScope * i + 0.5d;
            }
        }

    }

    [Button]
    public void CreateDataTable2()
    {

        _scrollUpgrade1_Price = new double[9];

        for (int i = 0; i < _scrollUpgrade1_Price.Length; i++)
        {
            if (i == 0)
            {
                _scrollUpgrade1_Price[i] = _scrollUpgrade1_base;
            }
            else
            {
                _scrollUpgrade1_Price[i] = _scrollUpgrade1_Price[i - 1] * _scrollUpgrade1_scope;
            }
        }

        _scrollUpgrade2_Price = new double[9];
        for (int i = 0; i < _scrollUpgrade2_Price.Length; i++)
        {
            if (i == 0)
            {
                _scrollUpgrade2_Price[i] = _scrollUpgrade2_base;
            }
            else
            {
                _scrollUpgrade2_Price[i] = _scrollUpgrade2_Price[i - 1] * _scrollUpgrade2_scope;
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
        _product_PriceText.text = $"{Managers.ToCurrencyString(_productPrice[_level] * (1 + _priceScope * _priceScopeLevel))}";
        _intervalText.text = $"{_interval}s";

        _upgrade_PriceText.text = _level < (_maxLevel - 1)
            ? $"{Managers.ToCurrencyString(_upgradePrice[_level])}" : "Max";

        _guageImg.fillAmount = ((_level + 1) % 10) * 0.1f; //== 0 ? 1f : ((_level + 1) % 10) * 0.1f;
        if (_level >= _maxLevel - 1)
            _guageImg.fillAmount = 1f; //== 0 ? 1f : ((_level + 1) % 10) * 0.1f;

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


            //if (_level >= _maxLevel - 1)
            //{
            //    RailOnOff(true);
            //_gamemanager._stageManager.FullUpgrade();
            //}
            // add gem
            if ((_level + 1) % 10 == 0)
            {
                _gamemanager.CalcGem(1);
            }

            //switch (_maxLevel)
            //{
            //    case int n when (n < 26):
            //        if (_level % 20 == 0 && isRail == false)
            //            _gamemanager._stageManager.ShowRvRailPanel(_machineNum);
            //        break;

            //    case int n when (n > 25 && n < 51):
            //        if (_level % 25 == 0 && isRail == false)
            //            _gamemanager._stageManager.ShowRvRailPanel(_machineNum);
            //        break;

            //    case int n when (n > 50 && n < 101):
            //        if (_level % 30 == 0 && isRail == false)
            //            _gamemanager._stageManager.ShowRvRailPanel(_machineNum);
            //        break;

            //    case int n when (n > 100 && n < 151):
            //        if (_level % 50 == 0 && isRail == false)
            //            _gamemanager._stageManager.ShowRvRailPanel(_machineNum);
            //        break;

            //    default:

            //        break;
            //}


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

    public void UpgradeMachine2(int _typeNum)
    {
        switch (_typeNum)
        {
            case 0:
                _gamemanager.CalcMoney(-_scrollUpgrade1_Price[_priceScopeLevel]);
                _priceScopeLevel++;

                break;

            case 1:
                _gamemanager.CalcMoney(-_scrollUpgrade2_Price[_spawnLevel]);
                _spawnLevel++;
                break;

            default:

                break;
        }

        SaveData();
    }


    public void SaveData()
    {
        //ES3.Save<int>(_name.ToString(), _level);
        DataManager.MachineData _data = new DataManager.MachineData();
        _data.Machin_Level = _level;
        _data.PriceScope_Level = _priceScopeLevel;
        _data.Spawn_Level = _spawnLevel;

        Managers.Data.SetMachineData(_gamemanager._stageManager._stageLevel, _name, _data);
    }

    public void LoadData()
    {
        DataManager.MachineData _data = Managers.Data.GetMachineData(_gamemanager._stageManager._stageLevel, _name);

        _level = _data.Machin_Level;
        _priceScopeLevel = _data.PriceScope_Level;
        _spawnLevel = _data.Spawn_Level;

        //if (_level >= _maxLevel - 1)
        //{

        //RailOnOff(true);
        //_gamemanager._stageManager.FullUpgrade();
        //}
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

    //public void RvRail(float _term = 1f)
    //{
    //    StartCoroutine(Cor_RvRail(_term));
    //}

    //IEnumerator Cor_RvRail(float _term = 0f)
    //{
    //    yield return new WaitForSeconds(_term);
    //    _gamemanager._stageManager.ShowRvRailPanel(_machineNum);
    //}

}
