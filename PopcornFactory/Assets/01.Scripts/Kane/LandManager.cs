using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.IO;
using System;
using MondayOFF;


public class LandManager : MonoBehaviour
{

    public int _landNum = 0;
    public GameObject _staff_Pref;
    public Transform[] _spawnPosGroup;

    //[FoldoutGroup("UI_Description_1")] public string _name;
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
    [FoldoutGroup("UI_Description_2")] public double[] _staffHire_Upgrade_Price;
    [FoldoutGroup("UI_Description_2")] public double[] _staffSpeed_Upgrade_Price;
    [FoldoutGroup("UI_Description_2")] public int _staff_hire_level;
    [FoldoutGroup("UI_Description_2")] public int _staff_speed_level;
    [FoldoutGroup("UI_Description_2")] public int _staff_max_level = 100;

    [Required]
    public Cup _cup;

    // Serializefield
    [SerializeField] List<Staff> _staffList = new List<Staff>();

    StageManager _stagemanager;

    bool isMoreWorker = false;


    //[Button]
    public void CreateDataTable2()
    {

        _staffHire_Upgrade_Price = new double[_staff_max_level];

        for (int i = 0; i < _staffHire_Upgrade_Price.Length; i++)
        {
            if (i == 0)
            {
                _staffHire_Upgrade_Price[i] = _scrollUpgrade1_base;
            }
            else
            {
                _staffHire_Upgrade_Price[i] = Math.Truncate(_staffHire_Upgrade_Price[i - 1] * _scrollUpgrade1_scope + 1);
            }
        }

        _staffSpeed_Upgrade_Price = new double[10];
        for (int i = 0; i < _staffSpeed_Upgrade_Price.Length; i++)
        {
            if (i == 0)
            {
                _staffSpeed_Upgrade_Price[i] = _scrollUpgrade2_base;
            }
            else
            {
                _staffSpeed_Upgrade_Price[i] = Math.Truncate(_staffSpeed_Upgrade_Price[i - 1] * _scrollUpgrade2_scope);
            }
        }
    }




    //private void Start()
    //{
    //    Init();
    //}




    public void Init()
    {
        LoadData();
        _stagemanager = Managers.Game._stageManager;

        ReadDataTable();

        //CreateDataTable2();

        Land_Setting();
    }

    [Button]
    public void ReadDataTable()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("UpgradeLand - Worker");

        _staff_max_level = data.Count;
        _staffHire_Upgrade_Price = new double[_staff_max_level];

        for (int i = 0; i < _staff_max_level; i++)
        {
            _staffHire_Upgrade_Price[i] = double.Parse(data[i][_landNum + "_Price"].ToString());
        }

        _staffSpeed_Upgrade_Price = new double[1];
        _staffSpeed_Upgrade_Price[0] = double.Parse(data[_landNum]["Speed_Up_Price"].ToString());


    }


    void Land_Setting()
    {

        for (int i = 0; i < _staff_hire_level; i++)
        {
            Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
            _trans.position = _spawnPosGroup[i % _spawnPosGroup.Length].position;  ///new Vector3(10f, 0.5f, 20f);

            _trans.GetComponent<Staff>().SetTrans(_trans.position);
            _staffList.Add(_trans.GetComponent<Staff>());
            _trans.GetComponent<Staff>()._speed = 5f + (float)_staff_speed_level * 0.5f;
            _trans.GetComponent<Staff>()._landNum = _landNum;

            _stagemanager.CheckButton();


        }

        if (_staff_hire_level > 1)
        {
            if (ES3.Load<int>("_tutorialLevel", 0) == 0) TutorialManager._instance.Tutorial_Comple();
        }

        if (_staff_hire_level == 0 && _landNum == 0)
        {
            Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox_Init"), _spawnPosGroup[_staff_hire_level % _spawnPosGroup.Length].transform).transform;
            _workerbox.position = _spawnPosGroup[_staff_hire_level % _spawnPosGroup.Length].position; // _spawnPos.position;
            _workerbox.localScale = Vector3.zero;
            _workerbox.DOScale(Vector3.one, 1f);

            _staff_hire_level++;

            if (TutorialManager._instance != null) TutorialManager._instance.Tutorial();

            SaveData();
        }



    }


    public void SpawnBox(bool isPay = true, GameObject _box = null)
    {
        if (isPay)
        {
            Managers.Game.CalcMoney(-_staffHire_Upgrade_Price[_staff_hire_level]);
        }

        Transform _workerbox = Managers.Pool.Pop(Resources.Load<GameObject>("WorkerBox"), _spawnPosGroup[_staff_hire_level % _spawnPosGroup.Length].transform).transform;
        _workerbox.position = _spawnPosGroup[_staff_hire_level % _spawnPosGroup.Length].position;// _spawnPos.position;
        _workerbox.localScale = Vector3.zero;
        _workerbox.DOScale(Vector3.one, 1f);
        _workerbox.GetComponent<WorkerBox>()._landNum = _landNum;

        _staff_hire_level++;


        EventTracker.LogCustomEvent("Upgrade", new Dictionary<string, string> { { $"Land_Upgrade_Level", $"Land_{_landNum}_Worker_level_{_staff_hire_level}" } });

        SaveData();
        _stagemanager.CheckButton();
    }

    public void AddStaff(GameObject _box = null)
    {
        Transform _boxTrans = _spawnPosGroup[0];
        if (_box != null)
        {
            _boxTrans = _box.transform;
            Managers.Pool.Push(_box.GetComponent<Poolable>());
        }
        // add fog particle 0630

        Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
        _trans.position = _boxTrans.position; // _spawnPos.position;  ///new Vector3(10f, 0.5f, 20f);
        _trans.GetComponent<Staff>().SetTrans(_trans.position);
        _staffList.Add(_trans.GetComponent<Staff>());
        _trans.GetComponent<Staff>()._speed = 5f + (float)_staff_speed_level * 0.5f;
        _trans.GetComponent<Staff>()._landNum = _landNum;

        _stagemanager.CheckButton();


    }

    public void AddSpeed(bool isPay = true)
    {
        if (isPay)
        {
            Managers.Game.CalcMoney(-_staffSpeed_Upgrade_Price[_staff_speed_level]);
        }

        for (int i = 0; i < _staffList.Count; i++)
        {
            _staffList[i]._speed = 5f + (float)_staff_speed_level * 5f;
        }

        _staff_speed_level++;


        EventTracker.LogCustomEvent("Upgrade", new Dictionary<string, string> { { $"Land_Upgrade_Level", $"Land_{_landNum}_Speed_level_{_staff_speed_level}" } });

        SaveData();
        _stagemanager.CheckButton();

    }



    public void Rv_Worker()
    {

        isMoreWorker = true;
        List<Staff> _rvStaffList = new List<Staff>();
        DOTween.Sequence().AppendCallback(() =>
        {
            for (int i = 0; i < 5; i++)
            {

                Transform _trans = Managers.Pool.Pop(_staff_Pref, transform).transform;
                _trans.position = _spawnPosGroup[i % _spawnPosGroup.Length].position;  // _spawnPos.position;
                _trans.GetComponent<Staff>().SetTrans(_trans.position);
                _rvStaffList.Add(_trans.GetComponent<Staff>());
                _trans.GetComponent<Staff>()._speed = 5f + (float)_staff_speed_level * 0.5f;
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
                isMoreWorker = false;

            });
    }



    public void LoadData()
    {

        _staff_hire_level = ES3.Load<int>($"Land_{_landNum}_staff_upgrade_Level", 0);
        _staff_speed_level = ES3.Load<int>($"Land_{_landNum}_staff_speed_level", 0);


    }

    public void SaveData()
    {
        ES3.Save<int>($"Land_{_landNum}_staff_upgrade_Level", _staff_hire_level);
        ES3.Save<int>($"Land_{_landNum}_staff_speed_level", _staff_speed_level);

    }



}
