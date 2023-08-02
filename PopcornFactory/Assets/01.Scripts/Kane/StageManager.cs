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
    public GameManager _gameManager;
    NavMeshSurface _navmeshsurface;

    // ======================
    public Canvas _canvas;

    public int _stageLevel = 0;

    public GameObject _staff_Pref;
    public Transform _spawnPos;

    [FoldoutGroup("SetObjects_1")] public Transform[] _cupObjs;
    [FoldoutGroup("SetObjects_1")] public Machine[] _machineGroup;
    [FoldoutGroup("SetObjects_1")] public GameObject[] _mapObjs;
    [FoldoutGroup("SetObjects_1")] public GameObject[] _mapOutline;
    [FoldoutGroup("SetObjects_1")] public List<Machine> _machineList = new List<Machine>();


    [FoldoutGroup("Upgrade_1")] public double[] _addStaff_Upgrade_Price;
    [FoldoutGroup("Upgrade_1")] public double[] _staffSpeed_Upgrade_Price;
    [FoldoutGroup("Upgrade_1")] public double[] _income_Upgrade_Price;
    [FoldoutGroup("Upgrade_1")] public double[] _addParts_Upgrade_Price;

    [FoldoutGroup("Upgrade_1")] public int _staff_upgrade_level;
    [FoldoutGroup("Upgrade_1")] public int _speed_Upgrade_level;
    [FoldoutGroup("Upgrade_1")] public int _income_ugrade_level;
    [FoldoutGroup("Upgrade_1")] public int _parts_upgrade_level;

    // ===========================================
    public double _nextStagePrice;



    ///
    [FoldoutGroup("Upgrade_3")] public double[] _scrollUpgrades_Price;
    public AnimationCurve _ease;
    public float _easeInterval = 0.75f;

    public double[] _MachineUpgrade_Price;


    // ===========================================
    public GameObject[] _popupButtons;
    public Transform _popupPanel;


    [SerializeField] Transform _targetMachine_Trans;
    EventTrigger _eventTrigger;

    CameraMove _cam;
    Transform _partsbutton_Trans;

    public Vector3[] _popupOffset = new Vector3[4];
    [SerializeField] Vector3 _addPartsButtonPos;

    [SerializeField] List<Staff> _staffList = new List<Staff>();

    [SerializeField] int upgradeCount = 0;

    UI_GameScene _gameUi;
    bool isEnd = true;
    [SerializeField] int _fullUpg_count = 0;
    /// RV /========================================

    public bool isRvDouble = false;
    public float intervalRvDouble = 0f;

    public double bigMoney = 0d;

    public float Init_IS_term = 180f;
    // ====================== =================================


    private void Start()
    {
        if (_gameManager == null) _gameManager = Managers.Game;
        _gameUi = Managers.GameUI;

        _navmeshsurface = GetComponent<NavMeshSurface>();

        AddScrollContent();

        _gameUi.NextStageButton.gameObject.SetActive(false);


        DataManager.StageData _data = Managers.Data.GetStageData(_stageLevel);

        _staff_upgrade_level = _data.Staff_Upgrade_Level;
        _income_ugrade_level = _data.Income_Upgrade_Level;
        _parts_upgrade_level = _data.Parts_Upgrade_Level;


        _speed_Upgrade_level = _data.Speed_Upgrade_Level;
        //FullUpgrade(false);

        SetTrans();
        _cam = Camera.main.transform.GetComponent<CameraMove>();

        // check stage settings , objects.. etc..
        _popupButtons = new GameObject[_machineGroup.Length];

        if (_canvas == null) _canvas = GameObject.FindGameObjectWithTag("Popup_Canvas").GetComponent<Canvas>();
        for (int i = 0; i < _machineGroup.Length; i++)
        {

            Transform _popupButton = Managers.Pool.Pop(Resources.Load<GameObject>("PopUp_Button"), _canvas.transform).transform;
            _popupButton.position = Camera.main.WorldToScreenPoint(_machineGroup[i].transform.position + _popupOffset[i]);
            _popupButton.gameObject.SetActive(false);
            _popupButtons[i] = _popupButton.gameObject;


        }

        _machineGroup[0].isAutoSpawn = true;


        _popupPanel = _gameUi.Upgrade_Panel.transform;
        _popupPanel.gameObject.SetActive(false);

        if (_mapObjs.Length > 0)
            _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(true);



        EventTrigger eventTrigger = _popupPanel.transform.Find("Upgrade_Button").GetComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerUp);




        StartCoroutine(Cor_Interstial());


        IEnumerator Cor_start()
        {
            _cam.GetComponent<AudioListener>().enabled = false;
            yield return new WaitForSeconds(3f);
            _cam.GetComponent<AudioListener>().enabled = true;
        }


        IEnumerator Cor_Interstial()
        {
            //yield return new WaitForSeconds(Init_IS_term);

            if (_stageLevel > 0)
            {
                WaitForSeconds _s60 = new WaitForSeconds(60f);
                WaitForSeconds _s1 = new WaitForSeconds(1f);
                while (true)
                {
                    if (AdsManager.ShowInterstitial() == true)
                    {
                        yield return _s60;
                    }
                    else
                    {
                        yield return _s1;
                    }
                }
            }
        }

    }

    void OnPointerDown(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = true;
        _targetMachine_Trans.GetComponent<Machine>().UpgradeMachine();
        _cam.GetComponent<CameraMove>().isClick = false;
    }

    void OnPointerUp(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = false;
    }



    private void Update()
    {
        for (int i = 0; i < _popupButtons.Length; i++)
        {
            _popupButtons[i].transform.position = Camera.main.WorldToScreenPoint(_machineGroup[i].transform.position + _popupOffset[i]);
        }

        if (_popupPanel.gameObject.activeSelf)
        {
            _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
        }
        if (_parts_upgrade_level < _mapOutline.Length)
            _gameUi.AddParts_Upgrade_Button.transform.position = Camera.main.WorldToScreenPoint(_addPartsButtonPos);



        _gameUi.RV_Income_TimeText.text = $" {"Income X2"} \n {(intervalRvDouble / 60).ToString("F0") + ":" + (intervalRvDouble % 60).ToString("F0")}";



    }

    public void SetTrans()
    {


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
        foreach (Machine _obj in _machineGroup)
        {
            _obj.gameObject.SetActive(false);
        }
        _machineGroup[0].gameObject.SetActive(true);


        _cupObjs[0].gameObject.SetActive(true);
        _partsbutton_Trans = _cupObjs[0];

        for (int i = 0; i < _staff_upgrade_level; i++)
        {
            Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
            _trans.position = _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);
            _staffList.Add(_trans.GetComponent<Staff>());
            _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
        }

        if (_staff_upgrade_level == 0)
        {
            Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox_Init"), _spawnPos.transform).transform;
            _workerbox.position = _spawnPos.position;
            _workerbox.localScale = Vector3.zero;
            _workerbox.DOScale(Vector3.one, 1f);

            _staff_upgrade_level++;


        }

        for (int i = 0; i < _mapOutline.Length; i++)
        {
            _mapOutline[i].SetActive(false);
        }
        if (_mapOutline.Length > 0) _mapOutline[0].SetActive(true);

        for (int i = 0; i <= _parts_upgrade_level; i++)
        {
            switch (i)
            {
                case 0:
                    _cupObjs[0].gameObject.SetActive(true);
                    break;


                case 1:
                    _cupObjs[0].gameObject.SetActive(false);
                    _cupObjs[1].gameObject.SetActive(true);
                    _mapObjs[0].SetActive(true);
                    _partsbutton_Trans = _cupObjs[1];
                    break;

                case 2:
                    _cupObjs[1].gameObject.SetActive(false);
                    _cupObjs[2].gameObject.SetActive(true);
                    _mapObjs[1].SetActive(true);
                    _partsbutton_Trans = _cupObjs[2];
                    break;

                case 3:
                    _cupObjs[2].gameObject.SetActive(false);
                    _cupObjs[3].gameObject.SetActive(true);
                    _mapObjs[2].SetActive(true);
                    _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);

                    break;

            }
            if (i < _parts_upgrade_level) _mapOutline[i].SetActive(false);
            if (i + 1 < _mapOutline.Length) _mapOutline[i + 1l].SetActive(true);

            _machineList.Add(_machineGroup[i]);
            _machineGroup[i].gameObject.SetActive(true);
        }
        //_machineList.Add(_machineGroup[0]);

        _navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();


        CheckButton();



        _gameManager.CalcMoney(0);
        _navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();
        SaveData();
    }

    //===========Upgrade Button Func ================

    public void AddStaff(bool isPay = true, GameObject _box = null)
    {

        if (_box != null) Managers.Pool.Push(_box.GetComponent<Poolable>());
        // add fog particle 0630

        Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
        _trans.position = _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);
        _staffList.Add(_trans.GetComponent<Staff>());
        _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;

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
        if (_parts_upgrade_level < _machineGroup.Length - 1)
        {
            if (isPay) Managers.Game.CalcMoney(-_addParts_Upgrade_Price[_parts_upgrade_level]);
            switch (_parts_upgrade_level)
            {
                case 0:
                    _cupObjs[0].gameObject.SetActive(false);
                    _cupObjs[1].gameObject.SetActive(true);
                    _cupObjs[1].localScale = Vector3.zero;
                    _cupObjs[1].DOScale(Vector3.one, _easeInterval).SetEase(_ease);
                    _partsbutton_Trans = _cupObjs[1];

                    break;

                case 1:
                    _cupObjs[1].gameObject.SetActive(false);
                    _cupObjs[2].gameObject.SetActive(true);
                    _cupObjs[2].localScale = Vector3.zero;
                    _cupObjs[2].DOScale(Vector3.one, _easeInterval).SetEase(_ease);
                    _partsbutton_Trans = _cupObjs[2];
                    break;

                case 2:
                    _cupObjs[2].gameObject.SetActive(false);
                    _cupObjs[3].gameObject.SetActive(true);
                    _cupObjs[3].localScale = Vector3.zero;
                    _cupObjs[3].DOScale(Vector3.one, _easeInterval).SetEase(_ease);
                    _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);
                    break;

            }
            _mapOutline[_parts_upgrade_level].SetActive(false);
            if (_parts_upgrade_level + 1 < _mapOutline.Length) _mapOutline[_parts_upgrade_level + 1l].SetActive(true);

            _machineList.Add(_machineGroup[_parts_upgrade_level + 1]);
            _machineGroup[_parts_upgrade_level + 1].gameObject.SetActive(true);
            float _size1 = _machineGroup[_parts_upgrade_level + 1].transform.lossyScale.x;
            _machineGroup[_parts_upgrade_level + 1].transform.localScale = Vector3.zero;
            _machineGroup[_parts_upgrade_level + 1].transform.DOScale(Vector3.one * _size1, _easeInterval).SetEase(_ease);

            _mapObjs[_parts_upgrade_level].SetActive(true);
            float _size2 = _mapObjs[_parts_upgrade_level].transform.lossyScale.x;

            _mapObjs[_parts_upgrade_level].transform.localScale = Vector3.zero;
            _mapObjs[_parts_upgrade_level].transform.DOScale(Vector3.one * _size2, _easeInterval).SetEase(_ease);
            _cam.GetComponent<Camera>().DOOrthoSize(50f, _easeInterval * 0.5f);

            DOTween.Sequence().AppendInterval(_easeInterval).AppendCallback(() =>
            {
                _navmeshsurface.RemoveData();
                _navmeshsurface.BuildNavMesh();
                isEnd = true;
            });


        }


        _parts_upgrade_level++;

        DOTween.Sequence().Append(_gameUi.AddParts_Upgrade_Button.transform.DOScale(Vector3.zero, 0.3f));
        _cam.LookTarget(_machineGroup[_parts_upgrade_level].transform);


        SaveData();
        CheckButton();
    }

    [Button]
    public void RemoveMesh()
    {
        _navmeshsurface.RemoveData();
        //_navmeshsurface.BuildNavMesh();
    }
    [Button]
    public void RebuildMesh()
    {
        //_navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();
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


            for (int i = 0; i < _machineGroup.Length; i++)
            {
                _popupButtons[i].SetActive(
                (_gameManager.Money >= _machineGroup[i]._upgradePrice[_machineGroup[i]._level])
                && _machineGroup[i].gameObject.activeSelf
                && (_machineGroup[i]._level < _machineGroup[i]._maxLevel - 1)
                && _staff_upgrade_level > 1);

            }
        }
        catch { }

        try
        {
            _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
        }
        catch { }

        ScrollButtonCheck();

        upgradeCount = 0;
        for (int i = 0; i < _gameUi.ScrollUpgrades.Length; i++)
        {
            upgradeCount = _gameUi.ScrollUpgrades[i].transform.Find("List_Upgrade").GetComponent<Button>().interactable ? upgradeCount + 1 : upgradeCount;
        }



        if ((upgradeCount == 0) && bigMoney != 0)
        {
            _gameUi.UpgradeCountText.transform.parent.GetChild(1).gameObject.SetActive(false);
            _gameUi.UpgradeCountText.text = $"-";


            _gameUi.BigMoneyButton.gameObject.SetActive(true);
            _gameUi.BigMoneyButton.transform.DOLocalMoveX(0, 1f).SetEase(Ease.Linear);
            _gameUi.BigMoneyButton.transform.GetChild(0).GetComponent<Text>().text = $"{"+"}{Managers.ToCurrencyString(bigMoney * 5d)}";
        }
        else
        {
            _gameUi.UpgradeCountText.transform.parent.GetChild(1).gameObject.SetActive(true);
            _gameUi.UpgradeCountText.text = $"{upgradeCount} Upgrades";
            _gameUi.BigMoneyButton.transform.DOLocalMoveX(380, 1f).SetEase(Ease.Linear);

        }

        if (isEnd)
        {

            DOTween.Sequence()
                .AppendInterval(_easeInterval + 0.1f).AppendCallback(() =>
                {

                    if (_parts_upgrade_level < _mapOutline.Length)
                    {
                        _addPartsButtonPos = _mapOutline[_parts_upgrade_level].transform.position;
                        _gameUi.AddParts_Upgrade_Button.transform.DOScale(Vector3.one, 0.3f);
                    };
                }).OnComplete(() => isEnd = false);
        }


        if (_gameUi.NextStageButton.gameObject.activeSelf)
        {
            if (_gameManager.Money >= _nextStagePrice)
                _gameUi.NextStageButton.interactable = _gameManager.Money >= _nextStagePrice ? true : false;
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


            default:

                break;
        }

        Managers.Sound.Play("Money");
        SaveData();
        CheckButton();
    }




    //////////////////////////////////
    /// RV
    ///

    public void RV_Income_Double()
    {
        if (!isRvDouble)
        {
            MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Doble_Money" } });
            DOTween.Sequence().AppendCallback(() =>
            {
                isRvDouble = true;
                DOTween.To(() => 300f, x => intervalRvDouble = x, 0, 300f).SetEase(Ease.Linear);

                _gameUi.RV_Income_Double.interactable = false;
                _gameUi.RV_Income_Double.transform.GetChild(0).gameObject.SetActive(false);
                _gameUi.RV_Income_Double.transform.GetChild(1).gameObject.SetActive(true);

            })
                .AppendInterval(300f).
                AppendCallback(() =>
                {
                    isRvDouble = false;
                    intervalRvDouble = 300f;
                    _gameUi.RV_Income_Double.interactable = true;
                    _gameUi.RV_Income_Double.transform.GetChild(0).gameObject.SetActive(true);
                    _gameUi.RV_Income_Double.transform.GetChild(1).gameObject.SetActive(false);
                });

        }
    }

    public void RV_BigMoney()
    {
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Big_Money" } });
        _gameManager.AddMoney(bigMoney * 5d);
        _gameUi.BigMoneyButton.gameObject.SetActive(false);
    }


    public void RV_Rail()
    {
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_2", "Auto_Rail" } });
        _gameUi.RvRail_Panel.SetActive(false);
        DOTween.Sequence().AppendCallback(() => { _machineGroup[_parts_upgrade_level].RailOnOff(true); })
            .AppendInterval(30f)
            .AppendCallback(() => { _machineGroup[_parts_upgrade_level].RailOnOff(false); });
    }

    public void RV_Worker()
    {
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_2", "More Worker" } });
        _gameUi.RvWorker_Panel.SetActive(false);

        List<Staff> _rvStaffList = new List<Staff>();
        DOTween.Sequence().AppendCallback(() =>
        {
            for (int i = 0; i < 5; i++)
            {

                Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
                _trans.position = _spawnPos.position;
                _rvStaffList.Add(_trans.GetComponent<Staff>());
                _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
            }
        })
            .AppendInterval(30f)
            .AppendCallback(() =>
            {
                int _count = _rvStaffList.Count;
                for (int i = 0; i < _count; i++)
                {
                    Staff _staff = _rvStaffList[0];
                    _rvStaffList.Remove(_staff);
                    Managers.Pool.Push(_staff.GetComponent<Poolable>());
                }


            });
    }

    public void RV_Skin()
    {

        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Skin" } });
    }


    //////////////////////////////////
    /// ENd-- RV===================================================================================
    ///


    public void AddScrollContent()
    {

        int _tempCount = _gameUi.ScrollUpgrades.Length;
        for (int i = 0; i < _tempCount; i++)
        {
            Destroy(_gameUi.ScrollUpgrades[i].gameObject);
        }
        _gameUi.ScrollUpgrades.Initialize();



        int _scrollUpgradeCount = (_machineGroup.Length + 1) * 2;

        _gameUi.ScrollUpgrades = new GameObject[_scrollUpgradeCount];
        _scrollUpgrades_Price = new double[_scrollUpgradeCount];

        _gameUi.Content.GetComponent<RectTransform>().sizeDelta = new Vector3(0f, _scrollUpgradeCount * 200f + 200f);

        _gameUi.ScrollUpgrades[0] = Instantiate(Resources.Load<GameObject>("Worker_Hire"), _gameUi.Content.transform);
        _gameUi.ScrollUpgrades[1] = Instantiate(Resources.Load<GameObject>("Worker_Speed"), _gameUi.Content.transform);

        _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-2, 0));
        _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-1, 0));


        for (int i = 2; i < _scrollUpgradeCount; i++)
        {
            _gameUi.ScrollUpgrades[i] = Instantiate(Resources.Load<GameObject>("Upgrade_Content"), _gameUi.Content.transform);
        }

        for (int i = 0; i < _machineGroup.Length; i++)
        {
            int n = i;
            _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineGroup[i]._upgrade1_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _machineGroup[i]._name + _machineGroup[i]._upgrade1_name;
            _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _machineGroup[i]._name + _machineGroup[i]._upgrade1_explain;
            _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 0));


            _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineGroup[i]._upgrade2_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _machineGroup[i]._name + _machineGroup[i]._upgrade2_name;
            _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _machineGroup[i]._name + _machineGroup[i]._upgrade2_explain;
            _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 1));

        }
    }

    public void ScrollButtonCheck()
    {

        if (_staff_upgrade_level < _addStaff_Upgrade_Price.Length)
        {
            _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = _addStaff_Upgrade_Price[_staff_upgrade_level] == 0 ? "Free" : $"{Managers.ToCurrencyString(_addStaff_Upgrade_Price[_staff_upgrade_level], 2)}";

            _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
                (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level]);

            bigMoney = _addStaff_Upgrade_Price[_staff_upgrade_level];

        }
        else
        {
            _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";

            _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }

        if (_speed_Upgrade_level < 5)
        {
            _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_staffSpeed_Upgrade_Price[_speed_Upgrade_level], 2)}";
            _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
          (_gameManager.Money >= _staffSpeed_Upgrade_Price[_speed_Upgrade_level]);

            bigMoney = bigMoney > _staffSpeed_Upgrade_Price[_speed_Upgrade_level] ? _staffSpeed_Upgrade_Price[_speed_Upgrade_level] : bigMoney;
        }
        else
        {
            _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
            _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        }



        for (int i = 0; i < _machineGroup.Length; i++)
        {
            if (_machineGroup[i]._priceScopeLevel < 5 && _machineGroup[i].gameObject.activeSelf) // income upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineGroup[i]._scrollUpgrade1_Price[_machineGroup[i]._priceScopeLevel], 2)}";
                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _machineGroup[i]._scrollUpgrade1_Price[_machineGroup[i]._priceScopeLevel]);
                bigMoney = bigMoney > _machineGroup[i]._scrollUpgrade1_Price[_machineGroup[i]._priceScopeLevel] ? _machineGroup[i]._scrollUpgrade1_Price[_machineGroup[i]._priceScopeLevel] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_machineGroup[i].gameObject.activeSelf)
            {

                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }

            if (_machineGroup[i]._spawnLevel < 5 && _machineGroup[i].gameObject.activeSelf) // speed upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineGroup[i]._scrollUpgrade2_Price[_machineGroup[i]._spawnLevel], 2)}";
                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _machineGroup[i]._scrollUpgrade2_Price[_machineGroup[i]._spawnLevel]);
                bigMoney = bigMoney > _machineGroup[i]._scrollUpgrade2_Price[_machineGroup[i]._spawnLevel] ? _machineGroup[i]._scrollUpgrade2_Price[_machineGroup[i]._spawnLevel] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_machineGroup[i].gameObject.activeSelf)
            {
                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }


        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.Content.GetComponent<RectTransform>());

    }

    public void ScrollButtonUpgrade(int _num, int _typeNum)
    {
        switch (_num)
        {
            case -2:
                Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);

                Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPos.transform).transform;
                _workerbox.position = _spawnPos.position;
                _workerbox.localScale = Vector3.zero;
                _workerbox.DOScale(Vector3.one, 1f);

                _staff_upgrade_level++;
                break;

            case -1:
                Managers.Game.CalcMoney(-_staffSpeed_Upgrade_Price[_speed_Upgrade_level]);
                for (int i = 0; i < _staffList.Count; i++)
                {
                    _staffList[i]._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
                }

                _speed_Upgrade_level++;
                break;

            default:
                _machineGroup[_num].UpgradeMachine2(_typeNum);


                break;
        }

        Managers.Sound.Play("Money");
        SaveData();
        CheckButton();

    }

    [Button]
    public void ToNextStage()
    {
        _gameManager.CalcMoney(-_nextStagePrice);
        _gameManager.NextStage();
    }

    public void FullUpgrade(bool isfirst = true)
    {
        if (isfirst)
            _fullUpg_count++;
        if (_fullUpg_count >= _machineGroup.Length)
        {
            int _count = _staffList.Count;
            for (int i = 0; i < _count; i++)
            {
                Staff _staff = _staffList[0];
                _staffList.Remove(_staff);
                Managers.Pool.Push(_staff.GetComponent<Poolable>());
            }


            _staff_upgrade_level = _addStaff_Upgrade_Price.Length;


            DOTween.Sequence().AppendInterval(2f).AppendCallback(() =>
            {
                _gameUi.NextStageButton.transform.localScale = Vector3.zero;
                _gameUi.NextStageButton.gameObject.SetActive(true);
                _gameUi.NextStageButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutCirc);
                _gameUi.NextStageButton.interactable = false;
                _gameUi.NextStageButton.transform.Find("Image").Find("NextStagePriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_nextStagePrice, 2)}";
                _gameUi.NextStageButton.AddButtonEvent(() => ToNextStage());

            });

        }
        CheckButton();


    }




}
