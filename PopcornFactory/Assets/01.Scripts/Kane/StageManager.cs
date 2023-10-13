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
using System.Linq;


public class StageManager : MonoBehaviour
{
    public GameManager _gameManager;
    NavMeshSurface _navmeshsurface;


    // ======================
    public Canvas _canvas;

    public int _stageLevel = 0;


    public Transform[] _spawnPosGroup;

    [FoldoutGroup("SetObjects_1")] public Transform[] _cupObjs;

    [FoldoutGroup("SetObjects_1")] public GameObject[] _railObjs;

    [FoldoutGroup("SetObjects_1")][ShowInInspector] public List<GameObject> _mapObjs = new List<GameObject>();
    [FoldoutGroup("SetObjects_1")] public GameObject _landGroup; // 랜드 그룹
    [FoldoutGroup("SetObjects_1")] public List<LandManager> _landManagers = new List<LandManager>();
    [FoldoutGroup("SetObjects_1")] public Transform[] _machineGroup; // 랜드별 머신 그룹
    [FoldoutGroup("SetObjects_1")][ShowInInspector] public Machine[][] _land_machineGroup; // 모든 랜드의 머신 배열

    [FoldoutGroup("SetObjects_1")][ReadOnly] public List<Machine> _machineList = new List<Machine>();


    [FoldoutGroup("Upgrade_1")] public double[] _addParts_Upgrade_Price;

    [FoldoutGroup("Upgrade_1")] public int _parts_upgrade_level;

    // ===========================================


    ///
    //[FoldoutGroup("Upgrade_3")] public double[] _scrollUpgrades_Price;
    public AnimationCurve _ease;
    public float _easeInterval = 0.75f;

    public double[] _MachineUpgrade_Price;


    // ===========================================
    public GameObject[] _popupButtons;
    public Transform _popupPanel;


    public Transform _targetMachine_Trans;
    EventTrigger _eventTrigger;

    public CameraMove _islandCam;
    public Camera _cinemaCam;
    //Transform _partsbutton_Trans;

    public Vector3[] _popupOffset = new Vector3[5];
    [SerializeField] Vector3 _addPartsButtonPos;

    [SerializeField] List<Staff> _staffList = new List<Staff>();

    [SerializeField] int upgradeCount = 0;

    UI_GameScene _gameUi;
    bool isEnd = true;
    //[SerializeField] int _fullUpg_count = 0;
    //[SerializeField] int _rvRailNum = -1;
    [SerializeField] bool isFirst = true;
    public int _playTime = 0;
    public int _islandTime = 0;
    public int _cinemaTime = 0;

    public int _IsCount = 0;
    public int _RvCount = 0;
    public Material _beltMat;

    //public Transform _labButton;
    public Vector3 _labButton_offset;
    public bool isCinema = false;
    /// RV /========================================

    public bool isRvDouble = false;
    public float intervalRvDouble = 0f;

    public double bigMoney = 0d;

    public float Init_IS_term = 180f;
    //public bool isMoreWorker = false;
    public float _bigmoney_term = 20f;
    public float _bigmoney_Showterm = 30f;
    public float _maxTerm = 50f;
    public bool isBigMoney = false;
    // ====================== =================================


    GameObject[] _recipe = new GameObject[4];
    bool[] _recipeNum = new bool[4];

    public int _noAds = 0;

    public bool[] _rewardChecks = new bool[4];


    // ============================================================
    private void Start()
    {



        MondayOFF.IAPManager.RestorePurchase();

        if (_gameManager == null) _gameManager = Managers.Game;
        _gameUi = Managers.GameUI;


        _noAds = PlayerPrefs.GetInt("NoAds");
        if (_noAds == 0)
        {
            EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", "IsCount_0" } });
            EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", "RvCount_0" } });
        }
        else
        {
            AdsManager.DisableAdType(AdType.Banner | AdType.Interstitial);
            _gameUi.NoAds_Button.gameObject.SetActive(false);

        }

        _navmeshsurface = GetComponent<NavMeshSurface>();


        DataManager.StageData _data = Managers.Data.GetStageData(_stageLevel);

        _rewardChecks = ES3.Load<bool[]>("_rewardChecks", _rewardChecks);
        //TutorialManager._instance.Tutorial();


        if (_data.isFirst)
        {
            EventTracker.TryStage(_stageLevel);
            isFirst = false;
        }

        _parts_upgrade_level = _data.Parts_Upgrade_Level;
        _playTime = _data.PlayTime;
        _islandTime = _data.IslandTime;
        _cinemaTime = _data.CinemaTime;

        _gameManager.CalcGem(0);

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


        if (_parts_upgrade_level < 2)
        {
            Managers.GameUI.Cinema_Button.interactable = false;


            Managers.GameUI.Cinema_Button.transform.GetChild(0).gameObject.SetActive(true);
            Managers.GameUI.Cinema_Button.transform.GetChild(1).gameObject.SetActive(false);
            Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetComponent<Image>().color = Managers.GameUI.Cinema_Button.colors.disabledColor;
            Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetChild(0).GetComponent<Text>().color =
               Managers.GameUI.Cinema_Button.colors.disabledColor;
            Managers.GameUI.Cinema_Button.transform.GetChild(0).GetComponent<Text>().color = Managers.GameUI.Cinema_Button.colors.disabledColor;

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
        _islandCam = Camera.main.transform.GetComponent<CameraMove>();
        _cinemaCam = GameObject.FindGameObjectWithTag("CinemaCam").GetComponent<Camera>();

        // check stage settings , objects.. etc..
        _popupButtons = new GameObject[_machineList.Count];

        if (_canvas == null) _canvas = GameObject.FindGameObjectWithTag("Popup_Canvas").GetComponent<Canvas>();



        for (int i = 0; i < _machineList.Count; i++)
        {
            Transform _popupButton = Managers.Pool.Pop(Resources.Load<GameObject>("PopUp_Button"), _canvas.transform).transform;
            _popupButton.position = Camera.main.WorldToScreenPoint(_machineList[i].transform.position + _popupOffset[i]);
            _popupButton.gameObject.SetActive(false);
            _popupButtons[i] = _popupButton.gameObject;

        }




        _popupPanel = _gameUi.Upgrade_Panel.transform;
        _popupPanel.gameObject.SetActive(false);







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


        StartCoroutine(Cor_PlaytimeCheck());

        IEnumerator Cor_PlaytimeCheck()
        {
            WaitForSeconds _term = new WaitForSeconds(1f);
            while (true)
            {
                yield return _term;

                _playTime++;
                if (isCinema)
                {
                    _cinemaTime++;
                }
                else
                {
                    _islandTime++;
                }

                Managers.GameUI.CurrentPlayTime_Text.text = $"{_playTime / 60} : {_playTime % 60}";


                if (_playTime % 10 == 0)
                {
                    EventTracker.LogCustomEvent("Time", new Dictionary<string, string> { { "AllTime", $"PT:{_playTime}_Land:{_islandTime}_Cinema:{_cinemaTime}" } });
                }


                switch (_playTime)
                {
                    case 180:
                        _rewardChecks[0] = true;
                        break;

                    case 600:
                        _rewardChecks[1] = true;
                        break;

                    case 1200:
                        _rewardChecks[2] = true;
                        break;

                    case 1800:
                        _rewardChecks[3] = true;
                        break;
                }

                ES3.Save<bool[]>("_rewardChecks", _rewardChecks);

                bool isExist = false;
                for (int i = 0; i < _rewardChecks.Length; i++)
                {
                    if (_rewardChecks[i] == true)
                    {
                        isExist = true;
                        break;
                    }
                }
                Managers.GameUI.PlayTimeReward_Button.transform.GetChild(0).gameObject.SetActive(isExist);
                Managers.GameUI.PlayTimeReward_Accept_Button.interactable = isExist;
                Managers.GameUI.PlayTime_Guage.fillAmount = (float)((float)(_playTime) / 1800f);



                SaveData();
            }
        }



        StartCoroutine(Cor_Interstial());



        //IEnumerator Cor_start()
        //{
        //    _cam.GetComponent<AudioListener>().enabled = false;
        //    yield return new WaitForSeconds(3f);
        //    _cam.GetComponent<AudioListener>().enabled = true;
        //}


        IEnumerator Cor_Interstial()
        {
            if (_parts_upgrade_level < 1)
            {
                yield return new WaitForSeconds(Init_IS_term);
            }
            else
            {
                yield return new WaitForSeconds(60f);
            }


            WaitForSeconds _s60 = new WaitForSeconds(60f);
            WaitForSeconds _s1 = new WaitForSeconds(1f);
            while (true)
            {
                if (_noAds == 1)
                {
                    break;
                }

                //Debug.Log(AdsManager.IsInterstitialReady());

                if (AdsManager.IsInterstitialReady())
                {
                    Managers.GameUI.AdBreak_Panel.SetActive(true);
                    Managers.GameUI.AdBreak_Panel.transform.Find("AdMoney_Text").GetComponent<Text>().text
                        = Managers.ToCurrencyString(bigMoney * 0.5d);
                    Managers.Game.CalcMoney(bigMoney * 0.5d);
                    yield return _s1;
                    Managers.GameUI.AdBreak_Panel.SetActive(false);
                }

                if (AdsManager.ShowInterstitial() == true && _noAds == 0)
                {
                    _IsCount++;

                    EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", $"IsCount_{_IsCount}" } });
                    yield return _s60;
                }
                else
                {
                    yield return _s1;
                }

            }

        }

        int _index = 0;
        for (int i = 0; i < _gameUi.Recipe_Content.transform.childCount; i++)
        {
            for (int j = 0; j < _gameUi.Recipe_Content.transform.GetChild(i).childCount; j++)
            {
                _recipe[_index] = _gameUi.Recipe_Content.transform.GetChild(i).GetChild(j).gameObject;
                _recipe[_index].SetActive(false);
                _index++;
            }
        }

        LoadRecipe();
        _gameUi.Recipe_Button.gameObject.SetActive(false);
        if (_recipeNum[0]) _gameUi.Recipe_Button.gameObject.SetActive(true);

        for (int i = 0; i < _recipeNum.Length; i++)
        {

            if (_recipeNum[i]) _recipe[i].SetActive(true);
        }


    }


    public void GetPlayTimeReward()
    {
        for (int i = 0; i < _rewardChecks.Length; i++)
        {
            if (_rewardChecks[i])
            {
                EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "PlayTimeReward", i.ToString() } });
                switch (i)
                {
                    case 0:
                        Managers.Game.CalcGem(10);

                        break;

                    case 1:
                        Managers.Game._cinemaManager.SampleCleaner();
                        break;

                    case 2:
                        Managers.Game.CalcMoney(1000, 1);
                        break;

                    case 3:
                        Managers.Game.CalcGem(30);
                        break;

                    default:

                        break;
                }
                _rewardChecks[i] = false;
            }
        }

        ES3.Save<bool[]>("_rewardChecks", _rewardChecks);
        Managers.GameUI.PlayTimeReward_Accept_Button.interactable = false;
    }


    void OnPointerDown(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = true;
        _targetMachine_Trans.GetComponent<Machine>().UpgradeMachine();
        _islandCam.isClick = false;
    }

    void OnPointerUp(PointerEventData data)
    {
        _targetMachine_Trans.GetComponent<Machine>().isPress = false;
    }


    private void Update()
    {
        if (!isCinema)
        {


            for (int i = 0; i < _machineList.Count; i++)
            {

                _popupButtons[i].transform.position = Camera.main.WorldToScreenPoint(_machineList[i].transform.position + _popupOffset[i]);

            }

            //_labButton.transform.position = Camera.main.WorldToScreenPoint(_gameManager._labotoryManager.transform.position + _labButton_offset);


            if (_popupPanel.gameObject.activeSelf)
            {
                _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
            }

            _gameUi.RV_Income_TimeText.text = $" {MathF.Truncate(intervalRvDouble / 60f).ToString("F0") + " : " + (intervalRvDouble % 60).ToString("F0")}";
            //Debug.Log((intervalRvDouble / 60f));




            if (isBigMoney == false)
            {
                if (_parts_upgrade_level > 0)
                {
                    _bigmoney_term -= Time.deltaTime;
                    if (_bigmoney_term <= 0 && upgradeCount < 2)
                    {
                        _bigmoney_term = _maxTerm;
                        isBigMoney = true;
                        BigMoneyOnOff(true);
                        EventTracker.LogCustomEvent("RV_ShowCount", new Dictionary<string, string> { { "Rv_ShowPanel", "BigMoney" } });
                    }
                }
            }
            else
            {
                _bigmoney_Showterm -= Time.deltaTime;
                if (_bigmoney_Showterm <= 0)
                {
                    _bigmoney_Showterm = _maxTerm;
                    isBigMoney = false;
                    BigMoneyOnOff(false);
                }
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



        foreach (Machine _obj in _machineList)
        {
            _obj.gameObject.SetActive(false);
        }
        _machineList[0].gameObject.SetActive(true);
        //_machineList[0].Init();


        foreach (Transform _cup in _cupObjs)
        {
            _cup.gameObject.SetActive(false);
        }
        _cupObjs[0].gameObject.SetActive(true);


        foreach (GameObject _obj in _railObjs)
        {
            _obj.SetActive(false);
        }
        _beltMat.DOOffset(Vector2.zero, 0f);


        DOTween.Sequence().Append(_beltMat.DOOffset(new Vector2(0f, -1f), 1f).SetLoops(-1, LoopType.Restart));

        for (int i = 0; i <= _parts_upgrade_level; i++)
        {
            switch (i)
            {
                case 0:

                    break;

                case 2:
                    Managers.GameUI.Cinema_Button.interactable = true;
                    Managers.GameUI.Cinema_Button.transform.GetChild(0).gameObject.SetActive(false);
                    //Managers.GameUI.Cinema_Button.transform.GetChild(1).gameObject.SetActive(true);
                    Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetComponent<Image>().color = Managers.GameUI.Cinema_Button.colors.normalColor;
                    Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetChild(0).GetComponent<Text>().color = Managers.GameUI.Cinema_Button.colors.normalColor; Managers.GameUI.Cinema_Button.transform.GetChild(0).GetComponent<Text>().color = Managers.GameUI.Cinema_Button.colors.normalColor;
                    break;
                default:

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


            switch (i)
            {
                case int n when n < _land_machineGroup[0].GetLength(0):
                    _landManagers[0]._cup.NextPos();
                    if (i == (_land_machineGroup[0].GetLength(0) - 1))
                    {
                        //_landManagers[0]._cup.transform.GetChild(0).gameObject.SetActive(false);
                        _landManagers[0]._cup.isRail = true;
                        RailOn(0);
                    }
                    break;

                case int n when n < _land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0):
                    if (_landManagers[1]._cup.gameObject.activeSelf == false)
                    {
                        _landManagers[1]._cup.gameObject.SetActive(true);
                    }
                    _landManagers[1]._cup.NextPos();

                    if (i == (_land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) - 1))
                    {
                        //_landManagers[1]._cup.transform.GetChild(0).gameObject.SetActive(false);
                        _landManagers[1]._cup.isRail = true;
                        RailOn(1);
                    }
                    break;

                case int n when n < _land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) + _land_machineGroup[2].GetLength(0):
                    if (_landManagers[2]._cup.gameObject.activeSelf == false)
                    {
                        _landManagers[2]._cup.gameObject.SetActive(true);
                    }
                    _landManagers[2]._cup.NextPos();

                    if (i == (_land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) + _land_machineGroup[2].GetLength(0) - 1))
                    {
                        //_landManagers[2]._cup.transform.GetChild(0).gameObject.SetActive(false);
                        _landManagers[2]._cup.isRail = true;
                        RailOn(2);
                    }
                    break;

            }



        }


        _navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();


        CheckButton();



        _gameManager.CalcMoney(0);
        _gameManager.CalcMoney(0, 1);
        _navmeshsurface.RemoveData();
        _navmeshsurface.BuildNavMesh();
        SaveData();
    }

    public void RailOn(int _num)
    {
        _railObjs[_num].gameObject.SetActive(true);
        //switch (_num)
        //{
        //    case 0:
        //        //_gameManager._labotoryManager.Init();
        //        break;

        //    case 1:
        //        //_gameManager._labotoryManager._laboratory_list[0].SetActive(true);
        //        //_labButton.gameObject.SetActive(true);
        //        break;

        //    case 2:
        //        //_gameManager._labotoryManager._laboratory_list[1].SetActive(true);

        //        break;
        //}

    }

    //===========Upgrade Button Func ================


    public void AddParts(bool isPay = true)
    {
        if (_parts_upgrade_level < _machineList.Count - 1)
        {
            Managers.Sound.Play("addparts");
            if (isPay) Managers.Game.CalcMoney(-_addParts_Upgrade_Price[_parts_upgrade_level]);
            switch (_parts_upgrade_level)
            {
                case 0:
                    //CustomReviewManager.instance.StoreReview();

                    break;

                case 1:
                    CustomReviewManager.instance.StoreReview();
                    break;

                case 2:
                    //CustomReviewManager.instance.StoreReview();
                    //Debug.Log("Popup Store Review");
                    break;

            }

            _machineList[_parts_upgrade_level + 1].gameObject.SetActive(true);
            _machineList[_parts_upgrade_level + 1].Init();

            float _size1 = _machineList[_parts_upgrade_level + 1].transform.lossyScale.x;


            _mapObjs[_parts_upgrade_level].SetActive(true);
            float _size2 = _mapObjs[_parts_upgrade_level].transform.lossyScale.x;


            _islandCam.GetComponent<Camera>().DOOrthoSize(50f, _easeInterval * 0.5f);

            DOTween.Sequence().AppendInterval(_easeInterval + 3f).AppendCallback(() =>
            {
                _navmeshsurface.RemoveData();
                _navmeshsurface.BuildNavMesh();
                //isEnd = true;
            });


        }



        switch (_parts_upgrade_level)
        {
            case int n when n < _land_machineGroup[0].GetLength(0) - 1:
                _landManagers[0]._cup.NextPos();
                if (_parts_upgrade_level == (_land_machineGroup[0].GetLength(0) - 2))
                {
                    //_landManagers[0]._cup.transform.GetChild(0).gameObject.SetActive(false);
                    _landManagers[0]._cup.isRail = true;
                    RailOn(0);

                }
                break;

            case int n when n < _land_machineGroup[0].GetLength(0) - 1 + _land_machineGroup[1].GetLength(0):
                if (_landManagers[1]._cup.gameObject.activeSelf == false)
                {
                    _landManagers[1]._cup.gameObject.SetActive(true);
                    _islandCam.GetComponent<Camera>().DOOrthoSize(75f, 0.5f);
                }
                _landManagers[1]._cup.NextPos();
                if (_parts_upgrade_level == (_land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) - 2))
                {
                    //_landManagers[1]._cup.transform.GetChild(0).gameObject.SetActive(false);
                    _landManagers[1]._cup.isRail = true;
                    RailOn(1);
                    //if (TutorialManager._instance._tutorialLevel == 8)
                    //{
                    //    StartCoroutine(Cor_Tuto());
                    //}

                    //IEnumerator Cor_Tuto()
                    //{
                    //    yield return new WaitForSeconds(1f);
                    //    _islandCam.LookTarget(_gameManager._labotoryManager.transform);
                    //    _islandCam.GetComponent<Camera>().DOOrthoSize(45f, 0.5f);
                    //    yield return new WaitForSeconds(0.5f);
                    //    TutorialManager._instance.Tutorial();
                    //}

                }
                break;

            case int n when n < _land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) + _land_machineGroup[2].GetLength(0) - 1:
                if (_landManagers[2]._cup.gameObject.activeSelf == false)
                {
                    _landManagers[2]._cup.gameObject.SetActive(true);
                }
                _landManagers[2]._cup.NextPos();

                if (_parts_upgrade_level == (_land_machineGroup[0].GetLength(0) + _land_machineGroup[1].GetLength(0) + _land_machineGroup[2].GetLength(0) - 2))
                {
                    //_landManagers[2]._cup.transform.GetChild(0).gameObject.SetActive(false);
                    _landManagers[2]._cup.isRail = true;
                    RailOn(2);

                }
                break;

        }



        _parts_upgrade_level++;

        if (_parts_upgrade_level == 2)
        {
            Managers.GameUI.Cinema_Button.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (_parts_upgrade_level >= 2)
        {
            Managers.GameUI.Cinema_Button.interactable = true;
            Managers.GameUI.Cinema_Button.transform.GetChild(0).gameObject.SetActive(false);
            //Managers.GameUI.Cinema_Button.transform.GetChild(1).gameObject.SetActive(true);
            Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetComponent<Image>().color = Managers.GameUI.Cinema_Button.colors.normalColor;
            Managers.GameUI.Cinema_Button.transform.Find("CinemaImg").GetChild(0).GetComponent<Text>().color = Managers.GameUI.Cinema_Button.colors.normalColor; Managers.GameUI.Cinema_Button.transform.GetChild(0).GetComponent<Text>().color = Managers.GameUI.Cinema_Button.colors.normalColor;
        }

        _gameManager._cinemaManager.MachineOpen();

        EventTracker.LogCustomEvent("Upgrade", new Dictionary<string, string> { { $"Land_Upgrade_Level", $"AddLand_Level_{_parts_upgrade_level}" } });



        EventTracker.LogCustomEvent("AddLand", new Dictionary<string, string> { { $"Land_Upgrade_Level", $"AddLevel_{_parts_upgrade_level}_{_playTime}s" } });





        if (_parts_upgrade_level >= _machineList.Count - 1)
            _gameUi.AddParts_Upgrade_Button.gameObject.SetActive(false);


        _islandCam.LookTarget(_machineList[_parts_upgrade_level].transform);


        SaveData();
        CheckButton();
    }

    [Button]
    public void RemoveMesh()
    {
        _navmeshsurface.RemoveData();

    }
    [Button]
    public void RebuildMesh()
    {

        _navmeshsurface.BuildNavMesh();
    }
    // -====================================================

    public void SaveData()
    {
        DataManager.StageData _stagedata = new DataManager.StageData();
        _stagedata.Parts_Upgrade_Level = _parts_upgrade_level;
        _stagedata.PlayTime = _playTime;
        _stagedata.isFirst = false;
        _stagedata.IslandTime = _islandTime;
        _stagedata.CinemaTime = _cinemaTime;

        Managers.Data.SetStageData(_stagedata, _stageLevel);
    }

    public void CheckButton()
    {


        if (_gameManager == null) _gameManager = Managers.Game;

        //Debug.Log(_addParts_Upgrade_Price[_parts_upgrade_level]);
        _gameUi.AddParts_Price_Text.text = _parts_upgrade_level == _machineList.Count ? "Max" : $"{Managers.ToCurrencyString(_addParts_Upgrade_Price[_parts_upgrade_level])}";

        if (_parts_upgrade_level < _addParts_Upgrade_Price.Length - 1)
        {
            if (_gameManager.Money >= _addParts_Upgrade_Price[_parts_upgrade_level])
            {
                _gameUi.AddParts_Upgrade_Button.interactable = true;
                _gameUi.AddParts_Upgrade_Button.transform.Find("IslandImg").GetComponent<Image>().color = _gameUi.AddParts_Upgrade_Button.colors.normalColor;
                _gameUi.AddParts_Upgrade_Button.transform.Find("Dot").gameObject.SetActive(true);
                if (TutorialManager._instance._tutorialLevel == 6)
                {
                    TutorialManager._instance.Tutorial();
                }
            }
            else
            {
                _gameUi.AddParts_Upgrade_Button.interactable = false;
                _gameUi.AddParts_Upgrade_Button.transform.Find("IslandImg").GetComponent<Image>().color = _gameUi.AddParts_Upgrade_Button.colors.disabledColor;
                _gameUi.AddParts_Upgrade_Button.transform.Find("Dot").gameObject.SetActive(false);
            }
        }
        else
        {
            _gameUi.AddParts_Upgrade_Button.interactable = false;
            _gameUi.AddParts_Upgrade_Button.transform.Find("IslandImg").GetComponent<Image>().color = _gameUi.AddParts_Upgrade_Button.colors.disabledColor;
            _gameUi.AddParts_Upgrade_Button.transform.Find("Dot").gameObject.SetActive(false);
        }

        /// ===========================================


        try
        {


            for (int i = 0; i < _machineList.Count; i++)
            {
                _popupButtons[i].SetActive(
                (_gameManager.Money >= _machineList[i]._upgradePrice[_machineList[i]._level])
                && _machineList[i].gameObject.activeSelf
                && (_machineList[i]._level < _machineList[i]._maxLevel - 1) && !isCinema);

            }

            if (_popupButtons[0].activeSelf && TutorialManager._instance._tutorialLevel == 4)
            {
                DOTween.Sequence()
.Append(_islandCam.transform.DOMove(new Vector3(4.2f, 144.4f, -190f), 0.5f).SetEase(Ease.Linear))
.Append(_islandCam.GetComponent<Camera>().DOOrthoSize(45f, 0.5f))
      .AppendCallback(() => { _islandCam.isFix = true; })
.OnComplete(() => _islandCam.isFix = false);
                TutorialManager._instance.Tutorial();
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

        }

        else if (upgradeCount > 2 && _parts_upgrade_level > 0 && _bigmoney_term <= 0f)
        {
            _gameUi.BigMoneyButton.gameObject.SetActive(true);
            _gameUi.BigMoneyButton.transform.DOLocalMoveX(0, 1f).SetEase(Ease.Linear);
            _gameUi.BigMoneyButton.transform.GetChild(0).GetComponent<Text>().text = $"{"+"}{Managers.ToCurrencyString(bigMoney * 5d)}";
        }

        else
        {
            _gameUi.UpgradeCountText.transform.parent.GetChild(1).gameObject.SetActive(true);
            _gameUi.UpgradeCountText.text = $"{upgradeCount} Upgrades";

            if (TutorialManager._instance._tutorialLevel == 1)
            {
                TutorialManager._instance.Tutorial();
            }

        }




    }






    public void SelecteTarget(Transform _trasnform)
    {
        _targetMachine_Trans = _trasnform;
        _targetMachine_Trans.GetComponent<Machine>().CheckPrice(_popupPanel);
        _popupPanel.position = Camera.main.WorldToScreenPoint(_targetMachine_Trans.position + Vector3.up * 20f);
        _popupPanel.gameObject.SetActive(true);
        _islandCam.LookTarget(_targetMachine_Trans);
    }


    public void OffPopup()
    {

        _popupPanel.gameObject.SetActive(false);
        _gameUi.OffPopup();

        // add ohter popup uis
    }






    //////////////////////////////////
    /// RV
    ///

    public void RV_Income_Double()
    {
        if (!isRvDouble)
        {
            EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "Doble_Money" } });

            _RvCount++;

            EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", $"RvCount_{_RvCount}" } });


            DOTween.Sequence().AppendCallback(() =>
            {
                isRvDouble = true;
                DOTween.To(() => 300f, x => intervalRvDouble = x, 0, 300f).SetEase(Ease.Linear);

                _gameUi.RV_Income_Double.interactable = false;

                _gameUi.RV_Income_Double.transform.GetChild(0).gameObject.SetActive(true);

            })
                .AppendInterval(300f).
                AppendCallback(() =>
                {
                    isRvDouble = false;
                    intervalRvDouble = 300f;
                    _gameUi.RV_Income_Double.interactable = true;
                    _gameUi.RV_Income_Double.transform.GetChild(0).gameObject.SetActive(false);
                });



        }
    }

    public void RV_BigMoney()
    {
        isBigMoney = false;
        EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "Big_Money" } });
        _RvCount++;

        EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", $"RvCount_{_RvCount}" } });

        _gameManager.AddMoney(bigMoney * 5d);
        _gameUi.BigMoneyButton.gameObject.SetActive(false);

        BigMoneyOnOff(false);
    }


    //public void RV_Rail(int _num = 0)
    //{
    //    //int _num = _parts_upgrade_level;
    //    _num = _rvRailNum;

    //    _gameUi.RvRail_Panel.SetActive(false);
    //    DOTween.Sequence().AppendCallback(() => { _machineList[_num].RailOnOff(true); })
    //        .AppendInterval(30f)
    //        .AppendCallback(() => { _machineList[_num].RailOnOff(false); });
    //}

    //public void RV_Worker()
    //{

    //    _gameUi.RvWorker_Panel.SetActive(false);

    //    for (int i = 0; i < _landManagers.Count; i++)
    //    {
    //        _landManagers[i].Rv_Worker();
    //    }

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
    //}

    public void RV_Skin()
    {

        EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "Skin" } });
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
            isBigMoney = false;

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
        //_scrollUpgrades_Price = new double[_scrollUpgradeCount];

        //_gameUi.Content.GetComponent<RectTransform>().sizeDelta = new Vector3(0f, _scrollUpgradeCount * 200f + 200f);


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



        for (int i = _machineGroup.Length * 2; i < _scrollUpgradeCount; i++)
        {
            _gameUi.ScrollUpgrades[i] = Instantiate(Resources.Load<GameObject>("Upgrade_Content"), _gameUi.Content.transform);
        }

        for (int i = 0; i < _machineList.Count; i++)
        {
            int n = i;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineList[i]._upgrade1_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = /*_machineList[i]._name + " " +*/ _machineList[i]._upgrade1_name;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = /*_machineList[i]._name + " " +*/ _machineList[i]._upgrade1_explain;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 0));


            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Icon").GetComponent<Image>().sprite = _machineList[i]._upgrade2_sprite;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("Upgrade_Name").GetComponent<Text>().text
                = /*_machineList[i]._name + " " +*/ _machineList[i]._upgrade2_name;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("Upgrade_Explain").GetComponent<Text>().text
                = /*_machineList[i]._name + " " +*/ _machineList[i]._upgrade2_explain;
            _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().AddButtonEvent(() => ScrollButtonUpgrade(n, 1));

        }


        for (int i = 0; i < _gameUi.ScrollUpgrades.Length; i++)
        {
            _gameUi.ScrollUpgrades[i].SetActive(false);
        }



    }

    public void ScrollButtonCheck()
    {
        // ======= Scroll OnOff=========================
        for (int i = 0; i < _landManagers.Count; i++)
        {
            if (i == 0)
            {
                _gameUi.ScrollUpgrades[(i * 2)].SetActive(true);
                _gameUi.ScrollUpgrades[(i * 2) + 1].SetActive(true);
            }
            else
            {
                if (_landManagers[i].transform.GetChild(0).gameObject.activeSelf)
                {
                    _gameUi.ScrollUpgrades[(i * 2)].SetActive(true);
                    _gameUi.ScrollUpgrades[(i * 2) + 1].SetActive(true);
                }
            }
        }

        int _count = 0;
        for (int i = 0; i < _gameUi.ScrollUpgrades.Length; i++)
        {
            if (_gameUi.ScrollUpgrades[i].activeSelf) _count++;
        }


        _gameUi.Content.GetComponent<RectTransform>().sizeDelta = new Vector3(0f, _count * 200f + 200f);


        for (int i = 0; i < _machineList.Count; i++)
        {
            if (_machineList[i].gameObject.activeSelf)
            {
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].SetActive(true);
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].SetActive(true);
            }
        }


        // add


        bigMoney = _landManagers[0]._staffHire_Upgrade_Price[_landManagers[0]._staffHire_Upgrade_Price.Length - 1];


        for (int i = 0; i < _landManagers.Count; i++)
        {
            if (_landManagers[i]._staff_hire_level < _landManagers[i]._staff_max_level - 1 && _landManagers[i].transform.GetChild(0).gameObject.activeSelf)
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level], 2)}";
                _gameUi.ScrollUpgrades[(i * 2)].GetComponent<UpgradeContent>()._price = _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level];
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level]);
                bigMoney = bigMoney > _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] ? _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (i == 0 && _landManagers[i]._staff_hire_level < _landManagers[i]._staff_max_level - 1)
            {
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level], 2)}";
                _gameUi.ScrollUpgrades[(i * 2)].GetComponent<UpgradeContent>()._price = _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level];
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level]);
                bigMoney = bigMoney > _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] ? _landManagers[i]._staffHire_Upgrade_Price[_landManagers[i]._staff_hire_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_landManagers[i].transform.GetChild(0).gameObject.activeSelf && i != 0)
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;

                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2)].GetComponent<UpgradeContent>()._price = 9999999999;
                _gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
                _gameUi.ScrollUpgrades[(i * 2)].SetActive(false);
            }

            if (_landManagers[i]._staff_speed_level < 1 && _landManagers[i].transform.GetChild(0).gameObject.activeSelf) // speed upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level], 2)}";

                _gameUi.ScrollUpgrades[(i * 2) + 1].GetComponent<UpgradeContent>()._price = _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level];

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level]);
                bigMoney = bigMoney > _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] ? _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (i == 0 && _landManagers[i]._staff_speed_level < _landManagers[i]._staffSpeed_Upgrade_Price.Length - 1)
            {
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level], 2)}";

                _gameUi.ScrollUpgrades[(i * 2) + 1].GetComponent<UpgradeContent>()._price = _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level];

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable =
               (_gameManager.Money >= _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level]);
                bigMoney = bigMoney > _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] ? _landManagers[i]._staffSpeed_Upgrade_Price[_landManagers[i]._staff_speed_level] : bigMoney;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else if (!_landManagers[i].transform.GetChild(0).gameObject.activeSelf && i != 0)
            {

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Unlock";
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;

                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<RectTransform>());
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + 1].GetComponent<UpgradeContent>()._price = 9999999999;
                _gameUi.ScrollUpgrades[(i * 2) + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2)].transform.Find("List_Upgrade").GetComponent<RectTransform>());
                _gameUi.ScrollUpgrades[(i * 2) + 1].SetActive(false);
            }
        }



        for (int i = 0; i < _machineList.Count; i++)
        {
            if (_machineList[i]._priceScopeLevel < 1 && _machineList[i].gameObject.activeSelf) // income upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel], 2)}";

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].GetComponent<UpgradeContent>()._price = _machineList[i]._scrollUpgrade1_Price[_machineList[i]._priceScopeLevel];

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
                //_gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").gameObject.SetActive(false);
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].GetComponent<UpgradeContent>()._price = 9999999999;
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 2].transform.Find("List_Upgrade").GetComponent<RectTransform>());
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2].gameObject.SetActive(false);
            }

            if (_machineList[i]._spawnLevel < 1 && _machineList[i].gameObject.activeSelf) // speed upgrade
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = $"{Managers.ToCurrencyString(_machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel], 2)}";

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].GetComponent<UpgradeContent>()._price = _machineList[i]._scrollUpgrade2_Price[_machineList[i]._spawnLevel];

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


                //_gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").gameObject.SetActive(false);
            }
            else
            {

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").Find("PriceText").GetComponent<Text>().text = "Max";
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].GetComponent<UpgradeContent>()._price = 9999999999;

                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].transform.Find("List_Upgrade").GetComponent<Button>().interactable = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.ScrollUpgrades[(i * 2) + 3].transform.Find("List_Upgrade").GetComponent<RectTransform>());
                _gameUi.ScrollUpgrades[(i * 2) + _machineGroup.Length * 2 + 1].gameObject.SetActive(false);
            }


        }
        _gameUi.BigMoneyButton.transform.GetChild(0).GetComponent<Text>().text = $"{"+"}{Managers.ToCurrencyString(bigMoney * 5d)}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(_gameUi.Content.GetComponent<RectTransform>());

        int _count2 = 0;
        for (int i = 0; i < _gameUi.ScrollUpgrades.Length; i++)
        {
            if (_gameUi.ScrollUpgrades[i].activeSelf) _count2++;
        }


        _gameUi.Content.GetComponent<RectTransform>().sizeDelta = new Vector3(0f, _count2 * 200f + 200f);

        // ========Scroll Sort========================

        List<UpgradeContent> _SortList = new List<UpgradeContent>();

        UpgradeContent[] _upgradecontent_arr = new UpgradeContent[_gameUi.ScrollUpgrades.Length];

        for (int i = 0; i < _gameUi.ScrollUpgrades.Length; i++)
        {
            _upgradecontent_arr[i] = _gameUi.ScrollUpgrades[i].GetComponent<UpgradeContent>();
        }


        _SortList = _upgradecontent_arr.ToList().OrderByDescending(x => x._price).ToList();

        for (int i = 0; i < _SortList.Count; i++)
        {
            _SortList[i].transform.SetSiblingIndex(0);
        }

        // =======================================

    }

    public void ScrollButtonUpgrade(int _num, int _typeNum, int _landNum = 0, int _tempNum = 0)
    {
        switch (_num)
        {


            case -1:



                if (_tempNum == 0)
                {
                    _landManagers[_landNum].SpawnBox();
                    if (TutorialManager._instance._tutorialLevel == 2 && _landManagers[0]._staff_hire_level == 3)
                    {
                        TutorialManager._instance.Tutorial_Comple();
                        TutorialManager._instance.Tutorial(false);
                    }
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


    //public void ShowRvRailPanel(int _num)
    //{
    //    //OffPopup();
    //    DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
    //    {
    //        _rvRailNum = _num;
    //        _gameUi.RvRail_Panel.SetActive(true);
    //    });
    //   

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



    //}

    public void isCinemaOn(bool isOn)
    {
        isCinema = isOn;
        _islandCam.GetComponent<Camera>().enabled = !isCinema;
        _cinemaCam.enabled = isCinema;

        AdsManager.ChangeAdvertyCamera(isOn ? _cinemaCam : _islandCam.GetComponent<Camera>());
        if (isOn)
            Popup_Off(true);
    }

    public void Popup_Off(bool isOn)
    {

        for (int i = 0; i < _machineList.Count; i++)
        {
            _popupButtons[i].SetActive(!isOn);

        }

    }


    public void SaveRecipe(int _num)
    {

        if (_recipeNum[_num] == false)
        {
            _recipeNum[_num] = true;
            ES3.Save("Recipe_" + _num, _recipeNum[_num]);
            _recipe[_num].SetActive(true);
            _gameUi.Recipe_Button.transform.GetChild(0).gameObject.SetActive(true);

        }
    }
    public void LoadRecipe()
    {
        for (int i = 0; i < _recipeNum.Length; i++)
        {
            _recipeNum[i] = ES3.Load("Recipe_" + i, false);

        }
    }


    public void GameRvCount()
    {
        _RvCount++;

        EventTracker.LogCustomEvent("AdsCount", new Dictionary<string, string> { { "AdsCount", $"RvCount_{_RvCount}" } });
    }




}
