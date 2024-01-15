using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Sirenix.OdinInspector;
//using UnityEngine.ProBuilder.Shapes;
using DG.Tweening;
using MondayOFF;

public class CinemaManager : MonoBehaviour
{

    public Transform[] _rvSpawnPos = new Transform[4];
    public int _rvCount = 0;

    public JoyStickController _joystick;



    [TitleGroup("Interact_Group")] public InteractArea[] _interactAreas;
    [TitleGroup("Interact_Group")] public int _interactLevel = 0;

    [TitleGroup("Machine")] public CinemaMachine[] _cinemaMachines;


    [TitleGroup("Customer")] public GameObject _customerPref;
    //[TitleGroup("Customer")] public int _currentCustomerCount;
    [TitleGroup("Customer")] public int _maxCustomerCount = 5;
    [TitleGroup("Customer")] public List<Customer> _customerList;

    [TitleGroup("Room")] public List<Room> _roomList;
    [TitleGroup("Room")] public int _roomNum = 0;
    [TitleGroup("Room")] public double[] _roomUnlockPrices;
    [TitleGroup("Room")] public double[] _roomScope;
    [TitleGroup("Room")] public double[] _roomProfits;






    public Counter _counter;
    public Transform _waitingPos;
    public float _watingTerm = 1f;

    [TitleGroup("Counter")] public GameObject _orderPanel;
    [TitleGroup("Counter")] public Image _orderImg;
    [TitleGroup("Counter")] public Text _orderText;
    [TitleGroup("Counter")] public List<CounterStaff> _counterStaffList;


    [TitleGroup("Cleaner")] public List<CleanerStaff> _cleanerStaffList;


    public Transform[] _doors = new Transform[2];



    public Room _upgradeTarget;

    public Player _player;

    [SerializeField] bool _isRvSpawnReady = true;
    [SerializeField] GameObject _rvInteractPref;

    public RvInteract _rvInteractTarget;

    public int _counterStaffLevel = 0;
    public int _cleanerStaffLevel = 0;
    /// =============
    [Header("Serialized")]
    Transform _customerGroup;
    [SerializeField] int _loopCount = 0;

    // ======== RV ======== =============================================
    public bool isRvDouble = false;

    public int _cinemaRvNum = 0;


    public float _rvMoneyPrice = 200;

    // ================ =============================================

    StageManager _stageManager;

    private void Start()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();

        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (_waitingPos == null) _waitingPos = transform.Find("WatingPos");
        if (_counter == null) _counter = transform.Find("Cinema_Counter").GetComponent<Counter>();

        _doors[0] = transform.Find("Enter_Door");
        _doors[1] = transform.Find("Exit_Door");

        _customerList = new List<Customer>();
        _customerGroup = new GameObject().transform;
        _customerGroup.name = "CustomerGroup";

        _counterStaffList = new List<CounterStaff>();
        _cleanerStaffList = new List<CleanerStaff>();

        //_orderPanel = transform.Find("OrderPanel").gameObject;
        //_orderImg = _orderPanel.transform.Find("OrderImg").GetComponent<Image>();
        //_orderText = _orderPanel.transform.Find("OrderText").GetComponent<Text>();


        _roomList = new List<Room>();

        Transform _roomGroup = transform.Find("RoomGroup");
        for (int i = 0; i < _roomGroup.childCount; i++)
        {
            _roomList.Add(_roomGroup.GetChild(i).GetComponent<Room>());
            _roomGroup.GetChild(i).GetComponent<Room>().LoadData();
        }

        _stageManager = Managers.Game._stageManager;


        LoadData();

        foreach (InteractArea _area in _interactAreas)
        {
            _area.gameObject.SetActive(false);
        }
        if (_interactLevel < _interactAreas.Length)
            _interactAreas[_interactLevel].gameObject.SetActive(true);




        MachineOpen();

        if (_rvInteractPref == null) _rvInteractPref = Resources.Load<GameObject>("RVInteractArea");

        _isRvSpawnReady = false;
        DOTween.Sequence().AppendInterval(60f).AppendCallback(() => _isRvSpawnReady = true);

        for (int i = 0; i < _counterStaffLevel; i++)
        {
            AddCounterStaff();
        }

        for (int i = 0; i < _cleanerStaffLevel; i++)
        {
            AddCleanerStaff();
        }


        StartCoroutine(Cor_Update());


    }

    public void MachineOpen()
    {
        switch (_stageManager._parts_upgrade_level)
        {
            case int n when n < 3:
                _cinemaMachines[1].gameObject.SetActive(false);
                _cinemaMachines[2].gameObject.SetActive(false);
                _rvMoneyPrice = 200;

                break;

            case int n when n > 2 && n < 5:
                _cinemaMachines[1].gameObject.SetActive(true);
                _cinemaMachines[2].gameObject.SetActive(false);
                _rvMoneyPrice = 400;

                break;

            case int n when n > 4:
                _cinemaMachines[1].gameObject.SetActive(true);
                _cinemaMachines[2].gameObject.SetActive(true);
                _rvMoneyPrice = 700;
                break;

        }
        Managers.GameUI.CinemaRvMoney_Text.text = $"Get {_rvMoneyPrice} Money!";
    }

    public void LoadData()
    {
        _interactLevel = ES3.Load<int>("_interactLevel", 0);
        _counterStaffLevel = ES3.Load<int>("_counterStaffLevel", 0);
        _cleanerStaffLevel = ES3.Load<int>("_cleanerStaffLevel", 0);

    }

    public void SaveData()
    {
        ES3.Save<int>("_interactLevel", _interactLevel);
    }

    public void NextInteract()
    {
        _interactLevel++;
        ES3.Save<int>("_interactLevel", _interactLevel);
        if (_interactLevel < _interactAreas.Length)
            _interactAreas[_interactLevel].gameObject.SetActive(true);
    }


    IEnumerator Cor_Update()
    {

        while (true)
        {

            if (_customerList.Count < _maxCustomerCount)
            {
                AddCustomer();
                yield return new WaitForSeconds(1f);
            }

            if (_customerList[0].CustomerState == Customer.State.Wait && _customerList[0].isArrive)
            {
                _customerList[0].CustomerState = Customer.State.Order;
                _customerList[0].transform.DORotate(new Vector3(0f, 180f, 0f), 1f).SetEase(Ease.Linear);
                _counter._customer = _customerList[0];

                _counter.Order(true);
            }

            //yield return new WaitForSeconds(2f);
            yield return null;

            SpawnRvObj();

        }

    }
    [Button]
    public void SpawnRvObj()
    {
        //if (Managers.Game.CinemaMoney < 1000)
        //{dd


        if (_isRvSpawnReady)
        {
            _isRvSpawnReady = false;
            Transform _rvObj = Managers.Pool.Pop(_rvInteractPref, transform).transform;
            _rvObj.transform.position = _rvSpawnPos[_rvCount % _rvSpawnPos.Length].position;  //_player.transform.position + new Vector3(0f, 0.7f, 5f);
            _rvCount++;
            _rvObj.GetComponent<RvInteract>().SetRvType((RvInteract.RvType)Random.Range(0, 3));

            DOTween.Sequence().AppendInterval(30f)
            .AppendCallback(() => _isRvSpawnReady = true);
        }

        //}
    }


    void AddCustomer()
    {
        Customer _customer = Managers.Pool.Pop(_customerPref, _customerGroup.transform).GetComponent<Customer>();
        _customerList.Add(_customer);
        // need to modify

        int _type = 0;
        switch (_stageManager._parts_upgrade_level)
        {
            case int n when n < 3:
                _type = 0;
                break;

            case int n when n > 2 && n < 5:
                _type = 1;
                break;

            case int n when n > 4:
                _type = 2;
                break;
        }

        _customer.SetInit(this, Random.Range(0, _type + 1), 1);
        _customer.GetComponent<NavMeshAgent>().Warp(_doors[0].position);

        _customer.SetDest(_waitingPos.position + new Vector3(0f, 0f, _watingTerm) * (_customerList.Count - 1));



        int _count = _customerList.Count;
        for (int i = 0; i < _count; i++)
        {
            _customerList[i].SetDest(_waitingPos.position + new Vector3(0f, 0f, _watingTerm) * i);

        }

    }




    public bool FindCinema()
    {

        if (_loopCount >= _roomList.Count)
        {
            _loopCount = 0;
            return false;
        }

        if (_roomList[_roomNum].isReady)
        {
            Customer _customer = _customerList[0];
            _customer.SetDest(_roomList[_roomNum].EmptySeat(_customer));
            _customer.CustomerState = Customer.State.Move;
            _customer._room = _roomList[_roomNum];
            _customerList.RemoveAt(0);



            _loopCount = 0;
            _counter.Order(false);
            return true;
        }
        else
        {
            _roomNum++;
            if (_roomNum >= _roomList.Count) _roomNum = 0;
            _loopCount++;
            return FindCinema();
        }


    }

    public void AddCounterStaff(bool isUnlock = false)
    {
        CounterStaff _staff = Managers.Pool.Pop(Resources.Load<GameObject>("Counter_Staff")).GetComponent<CounterStaff>();
        _staff.SetInit(this, CinemaStaff.CinemaStaffType.Counter, _counter.transform.position);
        _counterStaffList.Add(_staff);
        if (isUnlock)
        {
            _counterStaffLevel++;
            ES3.Save<int>("_counterStaffLevel", _counterStaffLevel);
        }
    }

    public void AddCleanerStaff(bool isUnlock = false)
    {
        CleanerStaff _staff = Managers.Pool.Pop(Resources.Load<GameObject>("Cleaner_Staff")).GetComponent<CleanerStaff>();
        _staff.SetInit(this, CinemaStaff.CinemaStaffType.Cleaner, _counter.transform.position + new Vector3(10f, 0f, 10f));
        _cleanerStaffList.Add(_staff);
        if (isUnlock)
        {
            _cleanerStaffLevel++;
            ES3.Save<int>("_cleanerStaffLevel", _cleanerStaffLevel);
        }

    }


    public void RoomUpgrade(int _num)
    {
        Managers.Sound.Play("UpgradeObj");
        _upgradeTarget.RoomChange(_num, true);
        _joystick.isFix = false;
    }

    public void CinemaRv()
    {
        Managers.Game._cinemaManager._joystick.isFix = false;



        _stageManager.GameRvCount();


        switch (_cinemaRvNum)
        {
            case 0:
                if (_joystick.Speed == 8f)
                {
                    _joystick.Speed = 12f;
                    DOTween.Sequence().AppendInterval(180f)
                       .AppendCallback(() => _joystick.Speed = 8f);

                    EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "SpeedUp" } });
                }

                break;

            case 1:
                Managers.Game.CalcMoney(_rvMoneyPrice, 1);
                EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "CinemaBigMoney_" + _rvMoneyPrice } });
                break;

            case 2:
                if (_player._maxCount == 1)
                {
                    _player._maxCount += 3;
                    DOTween.Sequence().AppendInterval(180f)
                        .AppendCallback(() => _player._maxCount = 1);
                    EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "AddCapacity" } });
                }
                break;

            case 3:
                if (_player.isCleaner == false)
                {
                    _player.CleanerOnoff(true);

                    DOTween.Sequence().AppendInterval(180f)
                       .AppendCallback(() => _player.CleanerOnoff(false));
                    EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "Cleaner" } });
                }
                break;
        }

        EventTracker.LogCustomEvent("RV", new Dictionary<string, string> { { "RvType", "Cinema_" + ((RvInteract.RvType)_cinemaRvNum).ToString() } });
        Managers.Pool.Push(_rvInteractTarget.GetComponent<Poolable>());
    }

    public void SampleCleaner()
    {
        if (_player.isCleaner == false)
        {
            _player.CleanerOnoff(true);

            DOTween.Sequence().AppendInterval(180f)
               .AppendCallback(() => _player.CleanerOnoff(false));

        }
    }


}
