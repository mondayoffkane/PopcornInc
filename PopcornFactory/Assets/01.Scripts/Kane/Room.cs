using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
//using UnityEngine.UI;

public class Room : EventObject
{
    public int _spawnCount = 6;


    public int _roomNumber = 0;

    public Transform _baseGroup;


    //public bool alwaysOpen = false;

    public bool isReady = true;

    [SerializeField] Transform _seatGroup;
    public Transform[] _seats;
    public int _currentCount = 0;
    //public int _seatCount = 0;

    public List<Customer> _customerList;

    public float _moviePlayTime = 10f;


    public GameObject _screen;

    public bool _isUnlock = false;
    public GameObject[] _onObjs;
    public GameObject[] _offObjs;

    public Transform _cleanGroup;
    public CleanObject[] _cleanObjects;
    [SerializeField] int _cleanObjectCount;
    [SerializeField] int _cleanCount = 0;

    public int _upgradeLevel = 0;

    public bool istutorial = false;

    // ===============================================================



    [TitleGroup("Room Meshes")] public Mesh[] _SeatMeshes;

    public GameObject[] _roomGroups;

    public Transform _spawnPos;

    [SerializeField] GameObject _fogParticle;

    // ================================ ===============================
    private void Start()
    {
        if (_fogParticle == null) _fogParticle = transform.Find("Fog_Particle").gameObject;


        if (_spawnPos == null) _spawnPos = transform.Find("SpawnPos");
        if (_spawnPos.GetComponent<MoneyZone>() == null) _spawnPos.gameObject.AddComponent<MoneyZone>();
        _spawnPos.GetComponent<MoneyZone>().SetInit(new Vector3(4f, 2f, 3f), 2, 2);


        _customerList = new List<Customer>();

        if (_baseGroup == null) _baseGroup = transform.GetChild(0);


        if (_screen == null) _screen = _baseGroup.transform.Find("Canvas").Find("Screen").gameObject;
        _screen.SetActive(false);

        _seatGroup = _baseGroup.transform.Find("SeatGroup");
        _cleanGroup = _baseGroup.transform.Find("CleanGroup");


        if (_seats == null)
            SetSeat();

        _unlockEvent.AddListener(() =>
        {
            Unlock(true);
            Managers.Game._cinemaManager.GetComponent<UnityEngine.AI.NavMeshSurface>().BuildNavMesh();
            //_upgradeLevel++;
            //ES3.Save<int>("Room_" + _roomNumber, _upgradeLevel);
        });


        _baseGroup.gameObject.SetActive(false);
        for (int i = 0; i < _roomGroups.Length; i++)
        {
            _roomGroups[i].SetActive(false);
        }

        _cleanObjectCount = _cleanGroup.childCount;
        _cleanObjects = new CleanObject[_cleanObjectCount];

        for (int i = 0; i < _cleanObjectCount; i++)
        {
            _cleanObjects[i] = _cleanGroup.GetChild(i).GetComponent<CleanObject>();
            _cleanObjects[i]._room = this;
        }


        LoadData();


    }

    public void LoadData()
    {
        Managers.Game._cinemaManager.GetComponent<UnityEngine.AI.NavMeshSurface>().BuildNavMesh();
        _upgradeLevel = ES3.Load<int>("Room_" + _roomNumber, 0);
        _isUnlock = ES3.Load<bool>($"Room_{_roomNumber}_isUnlock", false);

        //if (alwaysOpen)
        //{
        //    _isUnlock = true;
        //    if (_upgradeLevel < 1)
        //        _upgradeLevel = 1;
        //}




        if (_isUnlock)
        {
            isReady = true;

            Unlock(false);

        }
        else
        {
            isReady = false;

        }




    }

    [Button]
    public void SetSeat()
    {
        _seats = new Transform[_seatGroup.childCount];

        for (int i = 0; i < _seats.Length; i++)
        {
            _seats[i] = _seatGroup.GetChild(i);
        }
    }


    public Vector3 EmptySeat(Customer customer)
    {

        _customerList.Add(customer);
        if (_customerList.Count >= _seats.Length) isReady = false;
        return _seats[_customerList.Count - 1].position;
    }

    public void SeatCustomer()
    {
        _currentCount++;
        if (_currentCount >= _seats.Length)
        {
            StartCoroutine(Cor_PlayMovie());
        }
    }

    IEnumerator Cor_PlayMovie()
    {

        _screen.SetActive(true);
        yield return new WaitForSeconds(_moviePlayTime);
        _screen.SetActive(false);
        EndMovie();
    }

    [Button]
    public void EndMovie()
    {
        if (TutorialManager._instance._tutorialLevel == 10)
        {
            TutorialManager._instance.Tutorial();
        }

        _cleanCount = 0;
        foreach (Customer _customer in _customerList)
        {
            _customer.SetExit();
        }

        _customerList.Clear();
        _currentCount = 0;

        foreach (CleanObject _obj in _cleanObjects)
        {
            _obj.RoomClear(false);
        }

        isReady = false;

        SpawnMoney();

    }

    [Button]
    public void SpawnMoney()
    {

        //int _count;
        //switch (_upgradeLevel)
        //{
        //    case 1:
        //        _count = _spawnCount;
        //        break;

        //    case 2:
        //        _count = _spawnCount * 2;
        //        break;

        //    case 3:
        //        _count = _spawnCount * 5;
        //        break;

        //    default:
        //        _count = _spawnCount;
        //        break;
        //}

        _spawnPos.GetComponent<MoneyZone>().PopMoney(transform, 3 * _upgradeLevel, _spawnCount * _upgradeLevel);
    }




    public void Unlock(bool isLevelUp)
    {
        _fogParticle.SetActive(false);
        _fogParticle.SetActive(true);
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);

        //if (_upgradeLevel < 3)
        //{
        //    if (_onObjs.Length > 0)
        //    {
        //        for (int i = 0; i < _onObjs.Length; i++)
        //        {
        //            _onObjs[i].SetActive(true);
        //        }
        //    }
        //}


        if (_offObjs.Length > 0)
        {
            for (int i = 0; i < _offObjs.Length; i++)
            {
                _offObjs[i].SetActive(false);
            }

        }


        if (_baseGroup == null) _baseGroup = transform.GetChild(0);
        _baseGroup.gameObject.SetActive(true);
        if (isLevelUp)
        {
            _upgradeLevel++;
            ES3.Save<int>("Room_" + _roomNumber, _upgradeLevel);

            // add show room upgrade panel

            Managers.Game._cinemaManager._upgradeTarget = this;

            if (_upgradeLevel == 2)
            {
                Managers.GameUI.RoomUpgrade_Panel.SetActive(true);
                Managers.Game._cinemaManager._joystick.isFix = true;
            }

            if (_upgradeLevel < 2)
                RoomChange(_upgradeLevel - 1);
        }
        else
        {

            RoomChange(_upgradeLevel - 1);
        }

        if (_isUnlock == false)
        {
            _isUnlock = true;

            ES3.Save<bool>($"Room_{_roomNumber}_isUnlock", _isUnlock);
            isReady = true;

        }



    }



    public void ClearObj(int _num = 0)
    {
        _cleanCount++;


        if (_cleanCount >= _cleanObjectCount)
        {
            isReady = true;
            if (TutorialManager._instance._tutorialLevel == 10)
            {
                TutorialManager._instance.Tutorial_Comple();

            }
        }


    }

    [Button]
    public void RoomChange(int _num, bool isfix = false)
    {

        if (_num < 0) _num = 0;


        for (int i = 0; i < _seats.Length; i++)
        {
            if (_num < _SeatMeshes.Length)
                _seats[i].GetComponent<MeshFilter>().sharedMesh = _SeatMeshes[_num];
        }


        for (int i = 0; i < _roomGroups.Length; i++)
        {
            _roomGroups[i].SetActive(false);
        }

        _roomGroups[_num].SetActive(true);

        if (isfix)
        {
            _upgradeLevel = _num + 1;
            ES3.Save<int>("Room_" + _roomNumber, _upgradeLevel);
        }

    }



}
