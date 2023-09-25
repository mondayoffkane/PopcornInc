using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
//using UnityEngine.UI;

public class Room : EventObject
{

    public Transform _screenObj;

    public bool alwaysOpen = false;

    public bool isReady = true;

    [SerializeField] Transform _seatGroup;
    public Transform[] _seats;
    public int _currentCount = 0;
    //public int _seatCount = 0;

    public List<Customer> _customerList;

    public float _moviePlayTime = 5f;


    public GameObject _screen;

    public bool _isUnlock = false;
    public GameObject _offObj;

    public Transform _cleanGroup;
    public CleanObject[] _cleanObjects;
    [SerializeField] int _cleanObjectCount;
    [SerializeField] int _cleanCount = 0;

    // ===============================================================

    //[TitleGroup("Room Meshes")] public Material[] _WallMats;
    //[TitleGroup("Room Meshes")] public Material[] _FloorMats;

    [TitleGroup("Room Meshes")] public Mesh[] _SeatMeshes;
    //[TitleGroup("Room Meshes")] public Mesh[] _ScreenMeshes;
    //[TitleGroup("Room Meshes")] public Mesh[] _LightMeshes;
    //[TitleGroup("Room Meshes")] public Mesh[] _CarpetMeshes;


    //[TitleGroup("Room Meshes")] public MeshFilter _ScreenMesh;
    //[TitleGroup("Room Meshes")] public MeshFilter _LightMesh;
    //[TitleGroup("Room Meshes")] public MeshFilter _FloorMesh;
    //[TitleGroup("Room Meshes")] public MeshFilter _CarpetMesh;


    public GameObject[] _roomGroups;





    // ================================ ===============================
    private void Start()
    {
        _customerList = new List<Customer>();

        if (_screen == null) _screen = transform.Find("Canvas").Find("Screen").gameObject;
        _screen.SetActive(false);

        _seatGroup = transform.Find("SeatGroup");
        _cleanGroup = transform.Find("CleanGroup");


        if (_seats == null)
            SetSeat();

        _unlockEvent.AddListener(() => Unlock());


        LoadData();
        _cleanObjectCount = _cleanGroup.childCount;
        _cleanObjects = new CleanObject[_cleanObjectCount];

        for (int i = 0; i < _cleanObjectCount; i++)
        {
            _cleanObjects[i] = _cleanGroup.GetChild(i).GetComponent<CleanObject>();
            _cleanObjects[i]._room = this;
        }

        for (int i = 0; i < _roomGroups.Length; i++)
        {
            _roomGroups[i].SetActive(false);
        }

    }

    void LoadData()
    {
        if (alwaysOpen)
        {
            _isUnlock = true;
            //ES3.Save<bool>("isOpen", isOpen);
        }


        //isOpen = ES3.Load<bool>("isOpen", false);
        if (_isUnlock)
        {
            isReady = true;
            transform.localScale = Vector3.one;
        }
        else
        {
            isReady = false;
            transform.localScale = Vector3.zero;
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

    }

    public void Unlock()
    {
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
        _isUnlock = true;
        isReady = true;
        _offObj.SetActive(false);
    }

    public void ClearObj(int _num = 0)
    {
        _cleanCount++;


        if (_cleanCount >= _cleanObjectCount)
        {
            isReady = true;
        }


    }

    [Button]
    public void RoomUpgrade(int _num)
    {
        //Material[] _materials = GetComponent<Renderer>().sharedMaterials;

        //_materials[1] = _WallMats[1];

        //GetComponent<Renderer>().sharedMaterials = _materials;


        //_ScreenMesh.sharedMesh = _ScreenMeshes[1];
        //_LightMesh.sharedMesh = _LightMeshes[1];
        //_FloorMesh.sharedMesh = _FloorMeshes[1];


        for (int i = 0; i < _seats.Length; i++)
        {
            _seats[i].GetComponent<MeshFilter>().sharedMesh = _SeatMeshes[_num];
        }


        for (int i = 0; i < _roomGroups.Length; i++)
        {
            _roomGroups[i].SetActive(false);
        }

        _roomGroups[_num].SetActive(true);





    }



}
