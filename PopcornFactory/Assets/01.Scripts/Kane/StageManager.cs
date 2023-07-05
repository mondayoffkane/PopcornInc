using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using MondayOFF;

public class StageManager : MonoBehaviour
{
    public Vector3 _testoffset;
    public Canvas _canvas;

    public int _stageLevel = 0;

    //public List<Machine> _machineList = new List<Machine>();
    //public 
    public Transform[] _cupObjs;
    public GameObject _staff_Pref;


    GameManager _gameManager;

    public Transform _spawnPos;

    NavMeshSurface _navmeshsurface;

    public Machine[] _upgrade_Obj;

    public GameObject[] _mapObjs;
    public GameObject[] _unLockObjs;

    public List<Table> _targetList = new List<Table>();
    public List<Table> _activeTableList = new List<Table>();

    public Table[] _tableList;


    public double[] _addStaff_Upgrade_Price;
    public double[] _staffSpeed_Upgrade_Price;
    public double[] _income_Upgrade_Price;
    public double[] _addParts_Upgrade_Price;

    public int _staff_upgrade_level;
    public int _speed_Upgrade_level;
    public int _income_ugrade_level;
    public int _parts_upgrade_level;

    // ===========================================

    public double[] _cutting_Income_Upgrade_Price;
    public double[] _cutting_Speed_Upgrade_Price;
    public double[] _popcorn_Income_Upgrade_Price;
    public double[] _popcorn_Speed_Upgrade_Price;
    public double[] _seasoning_Income_Upgrade_Price;
    public double[] _seasoning_Speed_Upgrade_Price;

    public int _cutting_Income_Level;
    public int _cutting_Speed_Level;
    public int _popcorn_Income_Level;
    public int _popcorn_Speed_Level;
    public int _seasoning_Income_Level;
    public int _seasoning_Speed_Level;

    // ===========================================
    public GameObject[] _popupButtons;
    public Transform _popupPanel;


    public Transform _targetMachine_Trans;
    EventTrigger _eventTrigger;

    CameraMove _cam;
    Transform _partsbutton_Trans;

    public Vector3[] _popupOffset = new Vector3[3];

    [SerializeField] List<Staff> _staffList = new List<Staff>();

    int upgradeCount = 0;

    UI_GameScene _gameUi;

    /// RV /========================================

    public bool isRvDouble = false;
    public float intervalRvDouble = 0f;

    // ====================== =================================


    private void Start()
    {
        _gameManager = Managers.Game;
        _navmeshsurface = GetComponent<NavMeshSurface>();


        _targetList.Add(_tableList[0]);

        DataManager.StageData _data = Managers.Data.GetStageData(_stageLevel);

        _staff_upgrade_level = _data.Staff_Upgrade_Level;
        _income_ugrade_level = _data.Income_Upgrade_Level;
        _parts_upgrade_level = _data.Parts_Upgrade_Level;


        _speed_Upgrade_level = _data.Speed_Upgrade_Level;
        _cutting_Income_Level = _data.Cutting_Income_Level;
        _cutting_Speed_Level = _data.Cutting_Speed_Level;
        _popcorn_Income_Level = _data.Popcorn_Income_Level;
        _popcorn_Speed_Level = _data.Popcorn_Speed_Level;
        _seasoning_Income_Level = _data.Seasoning_Income_Level;
        _seasoning_Speed_Level = _data.Seasoning_Speed_Level;


        _gameUi = Managers.GameUI;
        SetTrans();
        _cam = Camera.main.transform.GetComponent<CameraMove>();

        // check stage settings , objects.. etc..
        _popupButtons = new GameObject[_upgrade_Obj.Length];

        for (int i = 0; i < _upgrade_Obj.Length; i++)
        {

            Transform _popupButton = Managers.Pool.Pop(Resources.Load<GameObject>("PopUp_Button"), _canvas.transform).transform;
            _popupButton.position = Camera.main.WorldToScreenPoint(_upgrade_Obj[i].transform.position + _popupOffset[i]);
            _popupButton.gameObject.SetActive(false);
            _popupButtons[i] = _popupButton.gameObject;


        }
        _popupButtons[0].GetComponent<Button>().AddButtonEvent(() =>
        {
            _targetMachine_Trans = _upgrade_Obj[0].transform;
            _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
            _popupPanel.gameObject.SetActive(true);
            _cam.LookTarget(_targetMachine_Trans);
        });
        _popupButtons[1].GetComponent<Button>().AddButtonEvent(() =>
        {
            _targetMachine_Trans = _upgrade_Obj[1].transform;
            _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
            _popupPanel.gameObject.SetActive(true);
            _cam.LookTarget(_targetMachine_Trans);
        });
        _popupButtons[2].GetComponent<Button>().AddButtonEvent(() =>
        {
            _targetMachine_Trans = _upgrade_Obj[2].transform;
            _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
            _popupPanel.gameObject.SetActive(true);
            _cam.LookTarget(_targetMachine_Trans);
        });



        _popupPanel = Managers.Pool.Pop(Resources.Load<GameObject>("Upgrade_Panel"), _canvas.transform).transform;
        _popupPanel.gameObject.SetActive(false);


        EventTrigger eventTrigger = _popupPanel.transform.Find("Upgrade_Button").GetComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerUp);


        _upgrade_Obj[1]._interval = 1f - 0.1f * _popcorn_Speed_Level;
        _upgrade_Obj[1]._interval = 1f - 0.1f * _popcorn_Speed_Level;
        _upgrade_Obj[2]._interval = 1f - 0.1f * _seasoning_Speed_Level;

        _gameUi.UpgradeCountText.transform.parent.gameObject.SetActive(false);

        StartCoroutine(Cor_start());

        IEnumerator Cor_start()
        {
            _cam.GetComponent<AudioListener>().enabled = false;
            yield return new WaitForSeconds(3f);
            _cam.GetComponent<AudioListener>().enabled = true;
        }
    }

    void OnPointerDown(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = true;
        _targetMachine_Trans.GetComponent<Machine>().UpgradeMachine();
    }

    void OnPointerUp(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = false;
    }



    private void Update()
    {
        for (int i = 0; i < _popupButtons.Length; i++)
        {
            _popupButtons[i].transform.position = Camera.main.WorldToScreenPoint(_upgrade_Obj[i].transform.position + _popupOffset[i]);
        }

        if (_popupPanel.gameObject.activeSelf)
        {
            _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
        }

        _gameUi.AddParts_Upgrade_Button.transform.position
      = Camera.main.WorldToScreenPoint(_partsbutton_Trans.position + new Vector3(8f, -8f, 0f));


    }

    public void SetTrans()
    {
        transform.eulerAngles = Vector3.up * 30f;
        transform.position = new Vector3(-10.5f, 0f, 0f);

        SettingMap();
    }


    public void SettingMap()
    {
        foreach (GameObject _obj in _mapObjs)
        {
            _obj.SetActive(false);
        }
        foreach (Transform _obj in _cupObjs)
        {
            _obj.gameObject.SetActive(false);

        }
        foreach (GameObject _obj in _unLockObjs)
        {
            _obj.SetActive(false);
        }

        _cupObjs[0].gameObject.SetActive(true);
        _partsbutton_Trans = _cupObjs[0];

        for (int i = 0; i < _staff_upgrade_level; i++)
        {
            Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
            _trans.position = _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);
            _staffList.Add(_trans.GetComponent<Staff>());
            _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
        }




        for (int i = 0; i < _parts_upgrade_level; i++)
        {
            switch (i)
            {
                case 0:
                    _cupObjs[0].gameObject.SetActive(false);
                    _mapObjs[0].SetActive(true);
                    _cupObjs[1].gameObject.SetActive(true);
                    _targetList.Add(_tableList[1]);
                    _partsbutton_Trans = _cupObjs[1];
                    break;

                case 1:
                    _cupObjs[1].gameObject.SetActive(false);
                    _mapObjs[1].SetActive(true);
                    _mapObjs[2].SetActive(true);

                    _targetList.Add(_tableList[2]);
                    _activeTableList.Add(_tableList[2]);
                    _cupObjs[2].gameObject.SetActive(true);
                    _partsbutton_Trans = _cupObjs[2];
                    _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);


                    break;

                    //case 2:
                    //    _targetList.Add(_tableList[3]);
                    //    _activeTableList.Add(_tableList[3]);
                    //    break;

                    //case 3:
                    //    _targetList.Add(_tableList[4]);
                    //    _activeTableList.Add(_tableList[4]);
                    //    break;
            }

            _unLockObjs[i].SetActive(true);


            _navmeshsurface.RemoveData();
            _navmeshsurface.BuildNavMesh();
        }


        _upgrade_Obj[0]._priceScopeLevel = _cutting_Income_Level;
        _upgrade_Obj[0]._interval = 1f - 0.1f * _cutting_Speed_Level;
        _upgrade_Obj[1]._priceScopeLevel = _popcorn_Income_Level;
        _upgrade_Obj[1]._interval = 1f - 0.1f * _popcorn_Speed_Level;
        _upgrade_Obj[2]._priceScopeLevel = _seasoning_Income_Level;
        _upgrade_Obj[2]._interval = 1f - 0.1f * _seasoning_Speed_Level;


        CheckButton();



        _gameManager.CalcMoney(0);
        _navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();
    }

    //===========Upgrade Button Func ================

    public void AddStaff(bool isPay = true, GameObject _box = null)
    {
        //if (isPay) Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);
        if (_box != null) Managers.Pool.Push(_box.GetComponent<Poolable>());
        // add fog particle 0630

        Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
        _trans.position = _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);
        _staffList.Add(_trans.GetComponent<Staff>());
        _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;


        //_staff_upgrade_level++;
        //SaveData();
        CheckButton();

    }

    public void AddIncome(bool isPay = true)
    {
        if (isPay) Managers.Game.CalcMoney(-_income_Upgrade_Price[_income_ugrade_level]);
        _income_ugrade_level++;

        SaveData();
        CheckButton();
    }

    public void AddParts(bool isPay = true)
    {
        if (_parts_upgrade_level < _unLockObjs.Length)
        {
            if (isPay) Managers.Game.CalcMoney(-_addParts_Upgrade_Price[_parts_upgrade_level]);
            switch (_parts_upgrade_level)
            {
                case 0:
                    _cupObjs[0].gameObject.SetActive(false);
                    _mapObjs[0].SetActive(true);
                    _cupObjs[1].gameObject.SetActive(true);
                    _targetList.Add(_tableList[1]);
                    _partsbutton_Trans = _cupObjs[1];
                    break;

                case 1:
                    _cupObjs[1].gameObject.SetActive(false);
                    _mapObjs[1].SetActive(true);
                    _mapObjs[2].SetActive(true);

                    _targetList.Add(_tableList[2]);
                    _activeTableList.Add(_tableList[2]);
                    _cupObjs[2].gameObject.SetActive(true);
                    //_partsbutton_Trans = _cupObjs[2];
                    _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);

                    break;

                    //case 2:
                    //    _targetList.Add(_tableList[3]);
                    //    _activeTableList.Add(_tableList[3]);
                    //    break;

                    //case 3:
                    //    _targetList.Add(_tableList[4]);
                    //    _activeTableList.Add(_tableList[4]);
                    //    break;
            }

            _unLockObjs[_parts_upgrade_level].SetActive(true);

            _navmeshsurface.RemoveData();
            _navmeshsurface.BuildNavMesh();

            _parts_upgrade_level++;
            SaveData();
            CheckButton();
        }
    }


    // -====================================================

    public void SaveData()
    {
        DataManager.StageData _stagedata = new DataManager.StageData();
        _stagedata.Stage_Level = _stageLevel;
        _stagedata.Staff_Upgrade_Level = _staff_upgrade_level;
        _stagedata.Income_Upgrade_Level = _income_ugrade_level;
        _stagedata.Parts_Upgrade_Level = _parts_upgrade_level;

        _stagedata.Speed_Upgrade_Level = _speed_Upgrade_level;
        _stagedata.Cutting_Income_Level = _cutting_Income_Level;
        _stagedata.Cutting_Speed_Level = _cutting_Speed_Level;
        _stagedata.Popcorn_Income_Level = _popcorn_Income_Level;
        _stagedata.Popcorn_Speed_Level = _popcorn_Speed_Level;
        _stagedata.Seasoning_Income_Level = _seasoning_Income_Level;
        _stagedata.Seasoning_Speed_Level = _seasoning_Speed_Level;

        Managers.Data.SetStageData(_stagedata);
    }

    public void CheckButton()
    {
        if (_gameManager == null) _gameManager = Managers.Game;

        switch (_staff_upgrade_level)
        {
            case 0:
                _gameUi.AddStaff_Price_Text.text = "Free";
                break;

            case int n when n >= _addStaff_Upgrade_Price.Length:
                _gameUi.AddStaff_Price_Text.text = "Max";
                break;

            default:
                _gameUi.AddStaff_Price_Text.text = $"{Managers.ToCurrencyString(_addStaff_Upgrade_Price[_staff_upgrade_level])}";
                break;
        }

        _gameUi.Income_Price_Text.text = _income_ugrade_level == _income_Upgrade_Price.Length ? "Max" : $"{Managers.ToCurrencyString(_income_Upgrade_Price[_income_ugrade_level])}";
        _gameUi.AddParts_Price_Text.text = _parts_upgrade_level == 4 ? "Max" : $"{Managers.ToCurrencyString(_addParts_Upgrade_Price[_parts_upgrade_level])}";

        if (_staff_upgrade_level < _addStaff_Upgrade_Price.Length)
        {
            if (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level])
            {
                _gameUi.AddStaff_Upgrade_Button.interactable = true;
            }
            else
            {
                _gameUi.AddStaff_Upgrade_Button.interactable = false;
            }
        }
        else
        {
            _gameUi.AddStaff_Upgrade_Button.interactable = false;
        }

        if (_income_ugrade_level < _income_Upgrade_Price.Length)
        {
            if (_gameManager.Money >= _income_Upgrade_Price[_income_ugrade_level])
            {
                _gameUi.Income_Upgrade_Button.interactable = true;
            }
            else
            {
                _gameUi.Income_Upgrade_Button.interactable = false;
            }
        }
        else
        {
            _gameUi.Income_Upgrade_Button.interactable = false;
        }


        if (_parts_upgrade_level < _addParts_Upgrade_Price.Length - 1)
        {
            if (_gameManager.Money >= _addParts_Upgrade_Price[_parts_upgrade_level])
            {
                _gameUi.AddParts_Upgrade_Button.interactable = true;
            }
            else
            {
                _gameUi.AddParts_Upgrade_Button.interactable = false;
            }
        }
        else
        {
            _gameUi.AddParts_Upgrade_Button.interactable = false;
        }

        /// ===========================================


        try
        {


            for (int i = 0; i < _upgrade_Obj.Length; i++)
            {
                _popupButtons[i].SetActive(
                (_gameManager.Money >= _upgrade_Obj[i]._upgradePrice[_upgrade_Obj[i]._level])
                && _upgrade_Obj[i].gameObject.activeSelf
                && _upgrade_Obj[i]._level < _upgrade_Obj[i]._maxLevel - 1);

            }
        }
        catch { }

        try
        {
            _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
        }
        catch { }


        /// add Upgrade scroll  Button check

        if (_staff_upgrade_level < _addStaff_Upgrade_Price.Length)
        {
            _gameUi.Worker_Hire.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = _addStaff_Upgrade_Price[_staff_upgrade_level] == 0 ? "Free" : $"{Managers.ToCurrencyString(_addStaff_Upgrade_Price[_staff_upgrade_level], 3)}";

            _gameUi.Worker_Hire.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
                (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level]);
        }
        else
        {
            _gameUi.Worker_Hire.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";

            _gameUi.Worker_Hire.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_speed_Upgrade_level < 5)
        {
            _gameUi.Worker_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_staffSpeed_Upgrade_Price[_speed_Upgrade_level], 3)}";
            _gameUi.Worker_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _staffSpeed_Upgrade_Price[_speed_Upgrade_level]);
        }
        else
        {
            _gameUi.Worker_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Worker_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_cutting_Income_Level < 5)
        {
            _gameUi.Upg1_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_cutting_Income_Upgrade_Price[_cutting_Income_Level], 3)}";
            _gameUi.Upg1_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _cutting_Income_Upgrade_Price[_cutting_Income_Level]);
        }
        else
        {
            _gameUi.Upg1_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg1_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_cutting_Speed_Level < 5)
        {
            _gameUi.Upg1_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_cutting_Speed_Upgrade_Price[_cutting_Speed_Level], 3)}";
            _gameUi.Upg1_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _cutting_Income_Upgrade_Price[_cutting_Speed_Level]);

        }
        else
        {
            _gameUi.Upg1_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg1_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_popcorn_Income_Level < 5)
        {
            _gameUi.Upg2_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_popcorn_Income_Upgrade_Price[_popcorn_Income_Level], 3)}";
            _gameUi.Upg2_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
           (_gameManager.Money >= _popcorn_Income_Upgrade_Price[_popcorn_Income_Level]);

        }
        else
        {
            _gameUi.Upg2_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg2_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_popcorn_Speed_Level < 5)
        {
            _gameUi.Upg2_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_popcorn_Speed_Upgrade_Price[_popcorn_Speed_Level], 3)}";
            _gameUi.Upg2_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _popcorn_Speed_Upgrade_Price[_popcorn_Speed_Level]);

        }
        else
        {
            _gameUi.Upg2_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg2_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_seasoning_Income_Level < 5)
        {
            _gameUi.Upg3_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_seasoning_Income_Upgrade_Price[_seasoning_Income_Level], 3)}";
            _gameUi.Upg3_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
         (_gameManager.Money >= _seasoning_Income_Upgrade_Price[_seasoning_Income_Level]);
        }
        else
        {
            _gameUi.Upg3_Income.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg3_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_seasoning_Speed_Level < 5)
        {
            _gameUi.Upg3_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_seasoning_Speed_Upgrade_Price[_seasoning_Speed_Level], 3)}";
            _gameUi.Upg3_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _seasoning_Speed_Upgrade_Price[_seasoning_Speed_Level]);
        }
        else
        {
            _gameUi.Upg3_Speed.transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.Upg3_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        //_gameUi.Worker_Hire.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level] && (_staff_upgrade_level < _addStaff_Upgrade_Price.Length - 1));
        //_gameUi.Worker_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _staffSpeed_Upgrade_Price[_speed_Upgrade_level] && (_speed_Upgrade_level < _staffSpeed_Upgrade_Price.Length - 1));

        //_gameUi.Upg1_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _cutting_Income_Upgrade_Price[_cutting_Income_Level] && (_cutting_Income_Level < _cutting_Income_Upgrade_Price.Length));
        //_gameUi.Upg1_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _cutting_Income_Upgrade_Price[_cutting_Speed_Level] && (_cutting_Speed_Level < _cutting_Income_Upgrade_Price.Length - 1));

        //_gameUi.Upg2_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _popcorn_Income_Upgrade_Price[_popcorn_Income_Level] && (_popcorn_Income_Level < _popcorn_Income_Upgrade_Price.Length - 1));
        //_gameUi.Upg2_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _popcorn_Speed_Upgrade_Price[_popcorn_Speed_Level] && (_popcorn_Speed_Level < _popcorn_Speed_Upgrade_Price.Length - 1));

        //_gameUi.Upg3_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _seasoning_Income_Upgrade_Price[_seasoning_Income_Level] && (_seasoning_Income_Level < _seasoning_Income_Upgrade_Price.Length - 1));
        //_gameUi.Upg3_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //    (_gameManager.Money >= _seasoning_Speed_Upgrade_Price[_seasoning_Speed_Level] && (_seasoning_Speed_Level < _seasoning_Speed_Upgrade_Price.Length - 1));


        upgradeCount = 0;
        upgradeCount = _gameUi.Worker_Hire.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Worker_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg1_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg1_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg2_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg2_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg3_Income.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        upgradeCount = _gameUi.Upg3_Speed.transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;


        if (upgradeCount == 0)
        {
            _gameUi.UpgradeCountText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _gameUi.UpgradeCountText.transform.parent.gameObject.SetActive(true);
            _gameUi.UpgradeCountText.text = $"{upgradeCount} Upgrades";


        }





    }





    public void SelecteTarget(Transform _trasnform)
    {
        _targetMachine_Trans = _trasnform;
        _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
        _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
        _popupPanel.gameObject.SetActive(true);
        _cam.LookTarget(_targetMachine_Trans);
    }


    public void OffPopup()
    {
        //Debug.Log("Off popup UI");
        _popupPanel.gameObject.SetActive(false);
        _gameUi.Setting_Panel.SetActive(false);
        _gameUi.Scroll_Panel.SetActive(false);

        // add ohter popup uis
    }

    //public void SpawnWorkerBox()
    //{
    //    Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);

    //    Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPos.transform).transform;
    //    _workerbox.position = _spawnPos.position;
    //    _workerbox.localScale = Vector3.zero;
    //    _workerbox.DOScale(Vector3.one, 1f);

    //    _staff_upgrade_level++;
    //    SaveData();
    //    CheckButton();
    //}

    public void UpgradeType(int _num)
    {
        switch (_num)
        {
            case 0:
                Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);

                Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPos.transform).transform;
                _workerbox.position = _spawnPos.position;
                _workerbox.localScale = Vector3.zero;
                _workerbox.DOScale(Vector3.one, 1f);

                _staff_upgrade_level++;

                break;
            case 1:

                for (int i = 0; i < _staffList.Count; i++)
                {
                    _staffList[i]._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
                }

                _speed_Upgrade_level++;
                break;

            case 2:
                _gameManager.CalcMoney(-_cutting_Income_Upgrade_Price[_cutting_Income_Level]);
                _cutting_Income_Level++;
                _upgrade_Obj[0]._priceScopeLevel = _cutting_Income_Level;

                break;

            case 3:
                _gameManager.CalcMoney(-_cutting_Speed_Upgrade_Price[_cutting_Speed_Level]);
                _cutting_Speed_Level++;
                _upgrade_Obj[0]._interval = 1f - 0.1f * _cutting_Speed_Level;

                break;

            case 4:
                _gameManager.CalcMoney(-_popcorn_Income_Upgrade_Price[_popcorn_Income_Level]);
                _popcorn_Income_Level++;
                _upgrade_Obj[1]._priceScopeLevel = _popcorn_Income_Level;
                break;

            case 5:
                _gameManager.CalcMoney(-_popcorn_Speed_Upgrade_Price[_popcorn_Speed_Level]);
                _popcorn_Speed_Level++;
                _upgrade_Obj[1]._interval = 1f - 0.1f * _popcorn_Speed_Level;
                break;

            case 6:
                _gameManager.CalcMoney(-_seasoning_Income_Upgrade_Price[_seasoning_Income_Level]);
                _seasoning_Income_Level++;
                _upgrade_Obj[2]._priceScopeLevel = _seasoning_Income_Level;
                break;

            case 7:
                _gameManager.CalcMoney(-_seasoning_Speed_Upgrade_Price[_seasoning_Speed_Level]);
                _seasoning_Speed_Level++;
                _upgrade_Obj[2]._interval = 1f - 0.1f * _seasoning_Speed_Level;
                break;

            default:

                break;
        }

        SaveData();
        CheckButton();
    }


    [Button]
    public void setupdagrdata(int max = 10, int num = 0, double _base = 0, double _scope = 1f)
    {
        switch (num)
        {
            case 0:
                _cutting_Income_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _cutting_Income_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _cutting_Income_Upgrade_Price[i] = _cutting_Income_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;

            case 1:
                _cutting_Speed_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _cutting_Speed_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _cutting_Speed_Upgrade_Price[i] = _cutting_Speed_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;

            case 2:
                _popcorn_Income_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _popcorn_Income_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _popcorn_Income_Upgrade_Price[i] = _popcorn_Income_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;

            case 3:
                _popcorn_Speed_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _popcorn_Speed_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _popcorn_Speed_Upgrade_Price[i] = _popcorn_Speed_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;
            case 4:
                _seasoning_Income_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _seasoning_Income_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _seasoning_Income_Upgrade_Price[i] = _seasoning_Income_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;


            case 5:
                _seasoning_Speed_Upgrade_Price = new double[max];
                for (int i = 0; i < max; i++)
                {
                    if (i == 0)
                    {
                        _seasoning_Speed_Upgrade_Price[i] = _base;
                    }
                    else
                    {
                        _seasoning_Speed_Upgrade_Price[i] = _seasoning_Speed_Upgrade_Price[i - 1] * _scope + _base * i;
                    }
                }
                break;

            default:

                break;
        }

    }

    //////////////////////////////////
    /// RV
    ///

    public void RV_Income_Double()
    {
        if (!isRvDouble)
        {
            DOTween.Sequence().AppendCallback(() =>
            {
                isRvDouble = true;
                DOTween.To(() => 300f, x => intervalRvDouble = x, 0, 300f).SetEase(Ease.Linear);

                _gameUi.RV_Income_Double.interactable = false;
            })
                .AppendInterval(300f).
                AppendCallback(() =>
                {
                    isRvDouble = false;
                    intervalRvDouble = 300f;
                    _gameUi.RV_Income_Double.interactable = true;
                });

            MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Income_Double" } });
        }
    }

    public void RV_Skin()
    {

        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Skin" } });
    }

}
