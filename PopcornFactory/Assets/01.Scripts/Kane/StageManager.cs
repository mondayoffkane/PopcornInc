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

    //public GameObject _staff_Pref;
    //public Transform _spawnPos;
    public Transform[] _spawnPosGroup;

    [FoldoutGroup("SetObjects_1")] public Transform[] _cupObjs;
    //[FoldoutGroup("SetObjects_1")] public Machine[] _machineList;
    [FoldoutGroup("SetObjects_1")][ShowInInspector] public List<GameObject> _mapObjs = new List<GameObject>();
    [FoldoutGroup("SetObjects_1")] public GameObject _landGroup; // 랜드 그룹
    [FoldoutGroup("SetObjects_1")] public List<LandManager> _landManagers = new List<LandManager>();
    [FoldoutGroup("SetObjects_1")] public Transform[] _machineGroup; // 랜드별 머신 그룹
    [FoldoutGroup("SetObjects_1")][ShowInInspector] public Machine[][] _land_machineGroup; // 모든 랜드의 머신 배열
    //[FoldoutGroup("SetObjects_1")] public int _total_machineCount; // 모든 머신 수
    //[FoldoutGroup("SetObjects_1")] public GameObject[] _mapOutline;
    [FoldoutGroup("SetObjects_1")][ReadOnly] public List<Machine> _machineList = new List<Machine>();


    //[FoldoutGroup("Upgrade_1")] public double[] _addStaff_Upgrade_Price;
    //[FoldoutGroup("Upgrade_1")] public double[] _staffSpeed_Upgrade_Price;
    //[FoldoutGroup("Upgrade_1")] public double[] _income_Upgrade_Price;
    [FoldoutGroup("Upgrade_1")] public double[] _addParts_Upgrade_Price;

    //[FoldoutGroup("Upgrade_1")] public int _staff_upgrade_level;
    //[FoldoutGroup("Upgrade_1")] public int _speed_Upgrade_level;
    //[FoldoutGroup("Upgrade_1")] public int _income_ugrade_level;
    [FoldoutGroup("Upgrade_1")] public int _parts_upgrade_level;

    // ===========================================
    //public double _nextStagePrice;



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
    //Transform _partsbutton_Trans;

    public Vector3[] _popupOffset = new Vector3[5];
    [SerializeField] Vector3 _addPartsButtonPos;

    [SerializeField] List<Staff> _staffList = new List<Staff>();

    [SerializeField] int upgradeCount = 0;

    UI_GameScene _gameUi;
    bool isEnd = true;
    [SerializeField] int _fullUpg_count = 0;
    [SerializeField] int _rvRailNum = -1;
    [SerializeField] bool isFirst = true;
    [SerializeField] int _playTime = 0;
    /// RV /========================================

    public bool isRvDouble = false;
    public float intervalRvDouble = 0f;

    public double bigMoney = 0d;

    public float Init_IS_term = 180f;
    //public bool isMoreWorker = false;
    public float _bigmoney_term = 20f;
    public bool isBigMoney = false;
    // ====================== =================================


    private void Start()
    {
        //Debug.Log("Spawn");

        if (_gameManager == null) _gameManager = Managers.Game;
        _gameUi = Managers.GameUI;

        //EventTracker.TryStage(_stageLevel);
        _navmeshsurface = GetComponent<NavMeshSurface>();



        // add map list








        //_gameUi.NextStageButton.gameObject.SetActive(false);


        DataManager.StageData _data = Managers.Data.GetStageData(_stageLevel);



        if (_data.isFirst)
        {
            EventTracker.TryStage(_stageLevel);
            isFirst = false;
        }
        //_staff_upgrade_level = _data.Staff_Upgrade_Level;
        //_income_ugrade_level = _data.Income_Upgrade_Level;
        _parts_upgrade_level = _data.Parts_Upgrade_Level;
        _playTime = _data.PlayTime;

        //_speed_Upgrade_level = _data.Speed_Upgrade_Level;
        //FullUpgrade(false);

        // 0816  machinGroup Init and add list

        //_total_machineCount = 0;

        _land_machineGroup = new Machine[_machineGroup.Length][];
        for (int i = 0; i < _machineGroup.Length; i++)
        {
            _land_machineGroup[i] = new Machine[_machineGroup[i].childCount];
            for (int j = 0; j < _machineGroup[i].transform.childCount; j++)
            {

                _land_machineGroup[i][j] = _machineGroup[i].GetChild(j).GetComponent<Machine>();
                //_total_machineCount++;
                _machineList.Add(_machineGroup[i].GetChild(j).GetComponent<Machine>());
            }
        }


        // mapobj list init  and, add list 
        _mapObjs.Clear();

        for (int i = 0; i < _landGroup.transform.childCount; i++)
        {
            for (int j = 0; j < _landGroup.transform.GetChild(i).childCount; j++)
            {
                _mapObjs.Add(_landGroup.transform.GetChild(i).GetChild(j).gameObject);
            }
            _landManagers.Add(_landGroup.transform.GetChild(i).GetComponent<LandManager>());

        }

        if (_mapObjs.Count > 0)
            _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(true);




        AddScrollContent();

        SetTrans();
        _cam = Camera.main.transform.GetComponent<CameraMove>();

        //Debug.Log("Totalcount :" + _total_machineCount);



        // check stage settings , objects.. etc..
        _popupButtons = new GameObject[_machineList.Count];

        if (_canvas == null) _canvas = GameObject.FindGameObjectWithTag("Popup_Canvas").GetComponent<Canvas>();
        //for (int i = 0; i < _totalCount; i++)
        //{

        //    Transform _popupButton = Managers.Pool.Pop(Resources.Load<GameObject>("PopUp_Button"), _canvas.transform).transform;
        //    _popupButton.position = Camera.main.WorldToScreenPoint(_tempmachineGroup[i].transform.position + _popupOffset[i]);
        //    _popupButton.gameObject.SetActive(false);
        //    _popupButtons[i] = _popupButton.gameObject;

        //    _tempmachineGroup[i]._machineNum = i;

        //}


        for (int i = 0; i < _machineList.Count; i++)
        {
            Transform _popupButton = Managers.Pool.Pop(Resources.Load<GameObject>("PopUp_Button"), _canvas.transform).transform;
            _popupButton.position = Camera.main.WorldToScreenPoint(_machineList[i].transform.position + _popupOffset[i]);
            _popupButton.gameObject.SetActive(false);
            _popupButtons[i] = _popupButton.gameObject;

        }








        //_tempmachineGroup[0].isAutoSpawn = true;


        _popupPanel = _gameUi.Upgrade_Panel.transform;
        _popupPanel.gameObject.SetActive(false);








        if (_stageLevel == 0)
        {
            _gameUi.RV_Income_Double.gameObject.SetActive(false);
            _gameUi.BigMoneyButton.gameObject.SetActive(false);
        }
        else
        {
            _gameUi.RV_Income_Double.gameObject.SetActive(true);
            _gameUi.BigMoneyButton.gameObject.SetActive(true);
        }

        for (int i = 0; i < _landManagers.Count; i++)
        {
            _landManagers[i].Init();
        }



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



        //IEnumerator Cor_start()
        //{
        //    _cam.GetComponent<AudioListener>().enabled = false;
        //    yield return new WaitForSeconds(3f);
        //    _cam.GetComponent<AudioListener>().enabled = true;
        //}


        IEnumerator Cor_Interstial()
        {
            yield return new WaitForSeconds(60f);

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

    //public void MachinePressFalse()
    //{
    //    if (_targetMachine_Trans != null)
    //        _targetMachine_Trans.GetComponent<Machine>().isPress = false;
    //}



    private void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    for (int i = 0; i < _machineGroup.Length; i++)
        //    {
        //        EventTracker.LogCustomEvent("Stage_" + _stageLevel.ToString() + "_Play_Time", new Dictionary<string, string>() { { _playTime.ToString() + "s", $"Machine_{i}_level :{_machineGroup[i]._level}" } });

        //    }

        //    //EventTracker.LogCustomEvent("event_name1", new Dictionary<string, string>() { { "Col 1", "Col2" } });
        //    //EventTracker.LogCustomEvent("event_name2", new Dictionary<string, Dictionary<string, string>>() { { "color4", new Dictionary<string, string>() { { "Col 1", "Col2" } } } });

        //    //Dictionary<string, string> _dic1 = new Dictionary<string, string>();
        //    //Dictionary<string, string> _dic2 = new Dictionary<string, string>();
        //    //_dic1.Add("")

        //    //EventTracker.LogCustomEvent("Test",)
        //}


        //for (int i = 0; i < _popupButtons.Length; i++)
        //{
        //    _popupButtons[i].transform.position = Camera.main.WorldToScreenPoint(_tempmachineGroup[i].transform.position + _popupOffset[i]);
        //}

        //int n = 0;

        for (int i = 0; i < _machineList.Count; i++)
        {
            _popupButtons[i].transform.position = Camera.main.WorldToScreenPoint(_machineList[i].transform.position + _popupOffset[i]);

        }



        //foreach (Machine _obj in _machineList)
        //{
        //    _obj.gameObject.SetActive(false);
        //}
        //_machineList[0].gameObject.SetActive(true);





        if (_popupPanel.gameObject.activeSelf)
        {
            _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
        }
        //if (_parts_upgrade_level < _mapOutline.Length)
        //    _gameUi.AddParts_Upgrade_Button.transform.position = Camera.main.WorldToScreenPoint(_addPartsButtonPos);



        _gameUi.RV_Income_TimeText.text = $" {"Income X2"} \n {(intervalRvDouble / 60).ToString("F0") + ":" + (intervalRvDouble % 60).ToString("F0")}";

        if (isBigMoney == false)
        {
            _bigmoney_term -= Time.deltaTime;
            if (_bigmoney_term <= 0)
            {
                _bigmoney_term = 20f;
                isBigMoney = true;
            }
        }

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
        //foreach (Transform _obj in _cupObjs)
        //{
        //    _obj.gameObject.SetActive(false);

        //}




        foreach (Machine _obj in _machineList)
        {
            _obj.gameObject.SetActive(false);
        }
        _machineList[0].gameObject.SetActive(true);
        _machineList[0].Init();





        //_cupObjs[0].gameObject.SetActive(true);
        //_partsbutton_Trans = _cupObjs[0];

        //for (int i = 0; i < _staff_upgrade_level; i++)
        //{
        //    Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
        //    _trans.position = _spawnPosGroup[i % _spawnPosGroup.Length].position;  ///new Vector3(10f, 0.5f, 20f);
        //    _staffList.Add(_trans.GetComponent<Staff>());
        //    _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
        //}

        //if (_staff_upgrade_level == 0)
        //{
        //    Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox_Init"), _spawnPosGroup[_staff_upgrade_level % _spawnPosGroup.Length].transform).transform;
        //    _workerbox.position = _spawnPosGroup[_staff_upgrade_level % _spawnPosGroup.Length].position; // _spawnPos.position;
        //    _workerbox.localScale = Vector3.zero;
        //    _workerbox.DOScale(Vector3.one, 1f);

        //    _staff_upgrade_level++;


        //}

        //for (int i = 0; i < _mapOutline.Length; i++)
        //{
        //    _mapOutline[i].SetActive(false);
        //}
        //if (_mapOutline.Length > 0) _mapOutline[0].SetActive(true);

        for (int i = 0; i <= _parts_upgrade_level; i++)
        {
            switch (i)
            {
                case 0:
                    //_cupObjs[0].gameObject.SetActive(true);
                    break;


                case 1:
                    //_cupObjs[0].gameObject.SetActive(false);
                    //_cupObjs[1].gameObject.SetActive(true);
                    //_mapObjs[0].SetActive(true);
                    //_partsbutton_Trans = _cupObjs[1];
                    break;

                case 2:
                    //_cupObjs[1].gameObject.SetActive(false);
                    //_cupObjs[2].gameObject.SetActive(true);
                    //_mapObjs[1].SetActive(true);
                    //_partsbutton_Trans = _cupObjs[2];
                    break;

                case 3:
                    //_cupObjs[2].gameObject.SetActive(false);
                    //_cupObjs[3].gameObject.SetActive(true);
                    //_mapObjs[2].SetActive(true);
                    //_gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);

                    break;

            }
            _machineList[i].gameObject.SetActive(true);
            _machineList[i].Init();
            if (i > 0)
            {
                _mapObjs[i - 1].SetActive(true);
            }

            if (_parts_upgrade_level >= _machineList.Count - 1)
                _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);

            if (i < _land_machineGroup[0].GetLength(0))
            {

                _landManagers[0]._cup.NextPos();
            }
            else
            {
                _landManagers[1]._cup.NextPos();
                _cupObjs[0].GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                _cupObjs[0].GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }


            //if (i < _parts_upgrade_level) _mapOutline[i].SetActive(false);
            //if (i + 1 < _mapOutline.Length) _mapOutline[i + 1l].SetActive(true);

            //_machineList.Add(_tempmachineGroup[i]);
            //_tempmachineGroup[i].gameObject.SetActive(true);
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

    //public void AddStaff(bool isPay = true, GameObject _box = null)
    //{
    //    Transform _boxTrans = _spawnPosGroup[0];
    //    if (_box != null)
    //    {
    //        _boxTrans = _box.transform;
    //        Managers.Pool.Push(_box.GetComponent<Poolable>());
    //    }
    //    // add fog particle 0630

    //    Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
    //    _trans.position = _boxTrans.position; // _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);

    //    _staffList.Add(_trans.GetComponent<Staff>());
    //    _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;

    //    CheckButton();


    //}

    //public void AddIncome(bool isPay = true)
    //{
    //    if (isPay) Managers.Game.CalcMoney(-_income_Upgrade_Price[_income_ugrade_level]);
    //    _income_ugrade_level++;

    //    SaveData();
    //    CheckButton();
    //}

    public void AddParts(bool isPay = true)
    {
        if (_parts_upgrade_level < _machineList.Count - 1)
        {
            if (isPay) Managers.Game.CalcMoney(-_addParts_Upgrade_Price[_parts_upgrade_level]);
            switch (_parts_upgrade_level)
            {
                case 0:
                    //_cupObjs[0].gameObject.SetActive(false);
                    //_cupObjs[1].gameObject.SetActive(true);
                    //_cupObjs[1].localScale = Vector3.zero;
                    //_cupObjs[1].DOScale(Vector3.one, _easeInterval).SetEase(_ease);
                    //_partsbutton_Trans = _cupObjs[1];

                    break;

                case 1:
                    //_cupObjs[1].gameObject.SetActive(false);
                    //_cupObjs[2].gameObject.SetActive(true);
                    //_cupObjs[2].localScale = Vector3.zero;
                    //_cupObjs[2].DOScale(Vector3.one, _easeInterval).SetEase(_ease);
                    //_partsbutton_Trans = _cupObjs[2];
                    break;

                case 2:

                    break;

            }
            //_mapOutline[_parts_upgrade_level].SetActive(false);
            //if (_parts_upgrade_level + 1 < _mapOutline.Length) _mapOutline[_parts_upgrade_level + 1l].SetActive(true);

            //_machineList.Add(_tempmachineGroup[_parts_upgrade_level + 1]);
            //_tempmachineGroup[_parts_upgrade_level + 1].gameObject.SetActive(true);
            _machineList[_parts_upgrade_level + 1].gameObject.SetActive(true);
            _machineList[_parts_upgrade_level + 1].Init();
            //_machineGroup[_parts_upgrade_level + 1].RvRail()
            //ShowRvRailPanel(_machineGroup[_parts_upgrade_level + 1]._machineNum);
            float _size1 = _machineList[_parts_upgrade_level + 1].transform.lossyScale.x;
            //Debug.Log("_size :" + _size1);
            //_machineList[_parts_upgrade_level + 1].transform.localScale = Vector3.zero;
            //_machineList[_parts_upgrade_level + 1].transform.DOScale(Vector3.one * _size1, _easeInterval).SetEase(_ease);

            _mapObjs[_parts_upgrade_level].SetActive(true);
            float _size2 = _mapObjs[_parts_upgrade_level].transform.lossyScale.x;

            //_mapObjs[_parts_upgrade_level].transform.localScale = Vector3.zero;
            //_mapObjs[_parts_upgrade_level].transform.DOScale(Vector3.one * _size2, _easeInterval).SetEase(_ease);
            _cam.GetComponent<Camera>().DOOrthoSize(50f, _easeInterval * 0.5f);

            DOTween.Sequence().AppendInterval(_easeInterval + 3f).AppendCallback(() =>
            {
                _navmeshsurface.RemoveData();
                _navmeshsurface.BuildNavMesh();
                //isEnd = true;
            });


        }



        if (_parts_upgrade_level < _land_machineGroup[0].GetLength(0))
        {

            _landManagers[0]._cup.NextPos();
        }
        else
        {

            _landManagers[1]._cup.NextPos();
            _cupObjs[0].GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            _cupObjs[0].GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
        }

        _parts_upgrade_level++;


        if (_parts_upgrade_level >= _machineList.Count - 1)
            _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);


        _cam.LookTarget(_machineList[_parts_upgrade_level].transform);


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
        //_stagedata.Stage_Level = _stageLevel;
        //_stagedata.Staff_Upgrade_Level = _staff_upgrade_level;
        //_stagedata.Income_Upgrade_Level = _income_ugrade_level;
        _stagedata.Parts_Upgrade_Level = _parts_upgrade_level;

        //_stagedata.Speed_Upgrade_Level = _speed_Upgrade_level;
        _stagedata.isFirst = false;

        Managers.Data.SetStageData(_stagedata, _stageLevel);
    }

    public void CheckButton()
    {
        if (_gameManager == null) _gameManager = Managers.Game;

        //switch (_staff_upgrade_level)
        //{
        //    case 0:
        //        _gameUi.AddStaff_Price_Text.text = "Free";
        //        break;

        //    case int n when n >= _addStaff_Upgrade_Price.Length:
        //        _gameUi.AddStaff_Price_Text.text = "Max";
        //        break;

        //    default:
        //        _gameUi.AddStaff_Price_Text.text = $"{Managers.ToCurrencyString(_addStaff_Upgrade_Price[_staff_upgrade_level])}";
        //        break;
        //}

        //_gameUi.Income_Price_Text.text = _income_ugrade_level == _income_Upgrade_Price.Length ? "Max" : $"{Managers.ToCurrencyString(_income_Upgrade_Price[_income_ugrade_level])}";
        //_gameUi.AddParts_Price_Text.text = _parts_upgrade_level == 4 ? "Max" : $"{Managers.ToCurrencyString(_addParts_Upgrade_Price[_parts_upgrade_level])}";

        //if (_staff_upgrade_level < _addStaff_Upgrade_Price.Length)
        //{
        //    if (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level])
        //    {
        //        _gameUi.AddStaff_Upgrade_Button.interactable = true;
        //    }
        //    else
        //    {
        //        _gameUi.AddStaff_Upgrade_Button.interactable = false;
        //    }
        //}
        //else
        //{
        //    _gameUi.AddStaff_Upgrade_Button.interactable = false;
        //}

        //if (_income_ugrade_level < _income_Upgrade_Price.Length)
        //{
        //    if (_gameManager.Money >= _income_Upgrade_Price[_income_ugrade_level])
        //    {
        //        _gameUi.Income_Upgrade_Button.interactable = true;
        //    }
        //    else
        //    {
        //        _gameUi.Income_Upgrade_Button.interactable = false;
        //    }
        //}
        //else
        //{
        //    _gameUi.Income_Upgrade_Button.interactable = false;
        //}


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


            for (int i = 0; i < _machineList.Count; i++)
            {
                _popupButtons[i].SetActive(
                (_gameManager.Money >= _machineList[i]._upgradePrice[_machineList[i]._level])
                && _machineList[i].gameObject.activeSelf
                && (_machineList[i]._level < _machineList[i]._maxLevel - 1));

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

            if (_bigmoney_term >= 20f && _stageLevel > 0)
            {
                isBigMoney = false;
                BigMoneyOnOff(true);
            }


        }
        else
        {
            _gameUi.UpgradeCountText.transform.parent.GetChild(1).gameObject.SetActive(true);
            _gameUi.UpgradeCountText.text = $"{upgradeCount} Upgrades";
        }

        //if (isEnd)
        //{

        //    DOTween.Sequence()
        //        .AppendInterval(_easeInterval + 0.1f).AppendCallback(() =>
        //        {

        //if (_parts_upgrade_level < _mapOutline.Length)
        //{
        //    _addPartsButtonPos = _mapOutline[_parts_upgrade_level].transform.position;
        //Debug.Log("Scale Up");
        //_gameUi.AddParts_Upgrade_Button.transform.DOScale(Vector3.one, 0.3f);
        //};
        //        }).OnComplete(() => isEnd = false);
        //}


        //if (_gameUi.NextStageButton.gameObject.activeSelf)
        //{
        //    if (_gameManager.Money >= _nextStagePrice)
        //        _gameUi.NextStageButton.interactable = _gameManager.Money >= _nextStagePrice ? true : false;
        //}



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



    //public void UpgradeType(int _num)
    //{
    //    switch (_num)
    //    {
    //        case 0:
    //            Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);

    //            Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPos.transform).transform;
    //            _workerbox.position = _spawnPos.position;
    //            _workerbox.localScale = Vector3.zero;
    //            _workerbox.DOScale(Vector3.one, 1f);

    //            _staff_upgrade_level++;

    //            break;
    //        case 1:

    //            for (int i = 0; i < _staffList.Count; i++)
    //            {
    //                _staffList[i]._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
    //            }

    //            _speed_Upgrade_level++;
    //            break;


    //        default:

    //            break;
    //    }

    //    Managers.Sound.Play("Money");
    //    SaveData();
    //    CheckButton();
    //}




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
        isBigMoney = false;
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Big_Money" } });
        _gameManager.AddMoney(bigMoney * 5d);
        //_gameUi.BigMoneyButton.gameObject.SetActive(false);

        BigMoneyOnOff(false);
    }


    public void RV_Rail(int _num = 0)
    {
        //int _num = _parts_upgrade_level;
        _num = _rvRailNum;
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_2", "Auto_Rail" } });
        _gameUi.RvRail_Panel.SetActive(false);
        DOTween.Sequence().AppendCallback(() => { _machineList[_num].RailOnOff(true); })
            .AppendInterval(30f)
            .AppendCallback(() => { _machineList[_num].RailOnOff(false); });
    }

    public void RV_Worker()
    {
        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_2", "More Worker" } });
        _gameUi.RvWorker_Panel.SetActive(false);

        for (int i = 0; i < _landManagers.Count; i++)
        {
            _landManagers[i].Rv_Worker();
        }

        //isMoreWorker = true;
        //List<Staff> _rvStaffList = new List<Staff>();
        //DOTween.Sequence().AppendCallback(() =>
        //{
        //    for (int i = 0; i < 5; i++)
        //    {

        //        Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
        //        _trans.position = _spawnPosGroup[_staff_upgrade_level % _spawnPosGroup.Length].position;  // _spawnPos.position;
        //        _rvStaffList.Add(_trans.GetComponent<Staff>());
        //        _trans.GetComponent<Staff>()._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
        //    }
        //})
        //    .AppendInterval(30f)
        //    .AppendCallback(() =>
        //    {
        //        int _count = _rvStaffList.Count;
        //        for (int i = 0; i < _count; i++)
        //        {
        //            Staff _staff = _rvStaffList[0];
        //            _rvStaffList.Remove(_staff);
        //            Managers.Pool.Push(_staff.GetComponent<Poolable>());
        //        }
        //        isMoreWorker = false;

        //    });
    }

    public void RV_Skin()
    {

        MondayOFF.EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType_1", "Skin" } });
    }


    public void BigMoneyOnOff(bool isOn)
    {
        if (isOn)
        {
            _gameUi.BigMoneyButton.gameObject.SetActive(true);
            _gameUi.BigMoneyButton.transform.DOLocalMoveX(0, 1f).SetEase(Ease.Linear);
            _gameUi.BigMoneyButton.transform.GetChild(0).GetComponent<Text>().text = $"{"+"}{Managers.ToCurrencyString(bigMoney * 5d)}";
        }
        else
        {

            _gameUi.BigMoneyButton.transform.DOLocalMoveX(380, 1f).SetEase(Ease.Linear);
        }


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



        int _scrollUpgradeCount = (_machineList.Count + _machineGroup.Length) * 2;


        _gameUi.ScrollUpgrades = new GameObject[_scrollUpgradeCount];
        _scrollUpgrades_Price = new double[_scrollUpgradeCount];

        _gameUi.Content.GetComponent<RectTransform>().sizeDelta = new Vector3(0f, _scrollUpgradeCount * 200f + 200f);


        for (int i = 0; i < _landManagers.Count * 2; i++)
        {
            _gameUi.ScrollUpgrades[i] = Instantiate(Resources.Load<GameObject>("Upgrade_Content"), _gameUi.Content.transform);
        }
        for (int i = 0; i < _landManagers.Count; i++)
        {
            int _landNum = i;

            _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Icon").GetComponent<Image>().sprite = _landManagers[i]._upgrade1_sprite;
            _gameUi.ScrollUpgrades[(i * 2)].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _landManagers[i]._upgrade1_name;
            _gameUi.ScrollUpgrades[(i * 2)].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _landManagers[i]._upgrade1_explain;
            _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-1, 0, _landNum, 0));


            _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Icon").GetComponent<Image>().sprite = _landManagers[i]._upgrade2_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _landManagers[i]._upgrade2_name;
            _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _landManagers[i]._upgrade2_explain;
            _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-1, 1, _landNum, 1));
        }

        //_gameUi.ScrollUpgrades[0] = Instantiate(Resources.Load<GameObject>("Worker_Hire"), _gameUi.Content.transform);
        //_gameUi.ScrollUpgrades[1] = Instantiate(Resources.Load<GameObject>("Worker_Speed"), _gameUi.Content.transform);

        //_gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-2, 0));
        //_gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-1, 0));

        //_gameUi.ScrollUpgrades[2] = Instantiate(Resources.Load<GameObject>("Worker_Hire"), _gameUi.Content.transform);
        //_gameUi.ScrollUpgrades[3] = Instantiate(Resources.Load<GameObject>("Worker_Speed"), _gameUi.Content.transform);

        //_gameUi.ScrollUpgrades[2].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-2, 0));
        //_gameUi.ScrollUpgrades[3].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(-1, 0));


        for (int i = _machineGroup.Length * 2; i < _scrollUpgradeCount; i++)
        {
            _gameUi.ScrollUpgrades[i] = Instantiate(Resources.Load<GameObject>("Upgrade_Content"), _gameUi.Content.transform);
        }

        for (int i = 0; i < _machineList.Count; i++)
        {
            int n = i;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineList[i]._upgrade1_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _machineList[i]._name + " " + _machineList[i]._upgrade1_name;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _machineList[i]._name + " " + _machineList[i]._upgrade1_explain;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 0));


            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineList[i]._upgrade2_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = _machineList[i]._name + " " + _machineList[i]._upgrade2_name;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = _machineList[i]._name + " " + _machineList[i]._upgrade2_explain;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 1));

        }
    }

    public void ScrollButtonCheck()
    {

        // add

        for (int i = 0; i < _landManagers.Count; i++)
        {
            if (_landManagers[i]._staff_hire_level < _landManagers[i]._staff_max_level && _landGroup.transform.GetChild(i).gameObject.activeSelf)
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level], 2)}";
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level]);
                bigMoney = bigMoney > _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] ? _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_landManagers[i].gameObject.activeSelf)
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }

            if (_landManagers[i]._staff_speed_level < 10 && _landManagers[i].gameObject.activeSelf) // speed upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level], 2)}";
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level]);
                bigMoney = bigMoney > _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] ? _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_landManagers[i].gameObject.activeSelf)
            {
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
        }
        //if (_staff_upgrade_level < _addStaff_Upgrade_Price.Length)
        //{
        //    _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = _addStaff_Upgrade_Price[_staff_upgrade_level] == 0 ? "Free" : $"{Managers.ToCurrencyString(_addStaff_Upgrade_Price[_staff_upgrade_level], 2)}";

        //    _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //        (_gameManager.Money >= _addStaff_Upgrade_Price[_staff_upgrade_level]);

        //    bigMoney = _addStaff_Upgrade_Price[_staff_upgrade_level];

        //}
        //else
        //{
        //    _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";

        //    _gameUi.ScrollUpgrades[0].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        //}

        //if (_speed_Upgrade_level < 10)
        //{
        //    _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_staffSpeed_Upgrade_Price[_speed_Upgrade_level], 2)}";
        //    _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
        //  (_gameManager.Money >= _staffSpeed_Upgrade_Price[_speed_Upgrade_level]);

        //    bigMoney = bigMoney > _staffSpeed_Upgrade_Price[_speed_Upgrade_level] ? _staffSpeed_Upgrade_Price[_speed_Upgrade_level] : bigMoney;
        //}
        //else
        //{
        //    _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
        //    _gameUi.ScrollUpgrades[1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
        //}



        for (int i = 0; i < _machineList.Count; i++)
        {
            if (_machineList[i]._priceScopeLevel < 10 && _machineList[i].gameObject.activeSelf) // income upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel], 2)}";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel]);
                bigMoney = bigMoney > _machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel] ? _machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_machineList[i].gameObject.activeSelf)
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }

            if (_machineList[i]._spawnLevel < 10 && _machineList[i].gameObject.activeSelf) // speed upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel], 2)}";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel]);
                bigMoney = bigMoney > _machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel] ? _machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_machineList[i].gameObject.activeSelf)
            {
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }


        }
        _gameUi.BigMoneyButton.transform.GetChild(0).GetComponent<Text>().text = $"{"+"}{Managers.ToCurrencyString(bigMoney * 5d)}";
        //LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.Content.GetComponent<RectTransform>());

    }

    public void ScrollButtonUpgrade(int _num, int _typeNum, int _landNum = 0, int _tempNum = 0)
    {
        switch (_num)
        {
            //case -2:

            //    _landGroup.transform.GetChild(_tempnum).GetComponent<LandManager>().SpawnBox();

            //    //Managers.Game.CalcMoney(-_addStaff_Upgrade_Price[_staff_upgrade_level]);

            //    //Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPosGroup[_staff_upgrade_level % _spawnPosGroup.Length].transform).transform;
            //    //_workerbox.position = _spawnPosGroup[_staff_upgrade_level % _spawnPosGroup.Length].position;// _spawnPos.position;
            //    //_workerbox.localScale = Vector3.zero;
            //    //_workerbox.DOScale(Vector3.one, 1f);

            //    //_staff_upgrade_level++;

            //    //if (_staff_upgrade_level % 5 == 0 && isMoreWorker == false)
            //    //{

            //    //ShowRvWorkerPanel();
            //    //}
            //    break;

            case -1:

                //_landGroup.transform.GetChild(_tempnum).GetComponent<LandManager>().AddSpeed();

                //Managers.Game.CalcMoney(-_staffSpeed_Upgrade_Price[_speed_Upgrade_level]);
                //for (int i = 0; i < _staffList.Count; i++)
                //{
                //    _staffList[i]._speed = 5f + (float)_speed_Upgrade_level * 0.5f;
                //}

                //_speed_Upgrade_level++;

                if (_tempNum == 0)
                {
                    _landManagers[_landNum].SpawnBox();
                }
                else
                {
                    _landManagers[_landNum].AddSpeed();
                }


                break;

            default:
                _machineList[_num].UpgradeMachine2(_typeNum);


                break;
        }

        Managers.Sound.Play("Money");
        SaveData();
        CheckButton();

    }

    //[Button]
    //public void ToNextStage()
    //{
    //    EventTracker.ClearStage(_stageLevel);
    //    _gameManager.CalcMoney(-_nextStagePrice);
    //    _gameManager.NextStage();
    //}

    //public void FullUpgrade(bool isfirst = true)
    //{
    //    if (isfirst)
    //        _fullUpg_count++;
    //    if (_fullUpg_count >= _machineList.Count)
    //    {
    //        int _count = _staffList.Count;
    //        for (int i = 0; i < _count; i++)
    //        {
    //            Staff _staff = _staffList[0];
    //            _staffList.Remove(_staff);
    //            Managers.Pool.Push(_staff.GetComponent<Poolable>());
    //        }


    //        _staff_upgrade_level = _addStaff_Upgrade_Price.Length;

    //        if (_stageLevel < 1)
    //        {
    //            DOTween.Sequence().AppendInterval(2f).AppendCallback(() =>
    //            {
    //                _gameUi.NextStageButton.transform.localScale = Vector3.zero;
    //                _gameUi.NextStageButton.gameObject.SetActive(true);
    //                _gameUi.NextStageButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutCirc);
    //                _gameUi.NextStageButton.interactable = false;
    //                _gameUi.NextStageButton.transform.Find("Image").Find("NextStagePriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_nextStagePrice, 2)}";
    //                _gameUi.NextStageButton.AddButtonEvent(() => ToNextStage());

    //            });
    //        }
    //        else
    //        {

    //        }

    //    }
    //    CheckButton();


    //}

    //public void ShowRvRailPanel(int _num)
    //{
    //    //OffPopup();
    //    DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
    //    {
    //        _rvRailNum = _num;
    //        _gameUi.RvRail_Panel.SetActive(true);
    //    });
    //    MondayOFF.EventTracker.LogCustomEvent("RV_ShowCount", new Dictionary<string, string> { { "Rv_ShowPanel", "Rv_Rail" } });


    //}

    //public void ShowRvWorkerPanel()
    //{
    //    DOTween.Sequence().AppendInterval(5f).AppendCallback(() =>
    //    {
    //        if (_targetMachine_Trans != null)
    //            _targetMachine_Trans.GetComponent<Machine>().isPress = false;
    //        OffPopup();
    //        _gameUi.RvWorker_Panel.SetActive(true);

    //    });
    //    MondayOFF.EventTracker.LogCustomEvent("RV_ShowCount", new Dictionary<string, string> { { "Rv_ShowPanel", "Rv_Worker" } });


    //}




}
