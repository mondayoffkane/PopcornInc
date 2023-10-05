using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Jobs.LowLevel.Unsafe;
using MoreMountains.NiceVibrations;

public class CleanObject : MonoBehaviour
{

    public bool isClean = true;

    [SerializeField] float _maxTerm = 1f;
    [SerializeField] float _currentTerm = 0f;

    [Header("UI")] public Image _circleGuage;

    public Room _room;

    public Transform[] _objs;

    public GameObject _uiGroup;

    public enum Type
    {
        Seat,
        Screen
    }
    public Type _ObjType;

    /// =============================================
    int _cnt = 0;
    private void Start()
    {
        //RoomClear(true);
        _uiGroup = transform.Find("Canvas").gameObject;
        _uiGroup.SetActive(false);
        _circleGuage.fillAmount = 0f;
        GetComponent<Renderer>().enabled = false;
    }


    public void RoomClear(bool _isClean)
    {
        isClean = _isClean;
        _uiGroup.SetActive(!isClean);

        if (_isClean)
        {
            foreach (Transform _trans in _objs)
            {
                //switch (_ObjType)
                //{
                //    case Type.Seat:
                _trans.transform.DORotate(Vector3.up * 180f, 0.5f);

                //    break;

                //case Type.Screen:

                //    break;
                //}
            }
            _room.ClearObj( /*num */);
            _uiGroup.SetActive(false);
        }
        else
        {

            foreach (Transform _trans in _objs)
            {
                switch (_ObjType)
                {
                    case Type.Seat:
                        _trans.transform.DORotate(Vector3.up * Random.Range(90f, 270f), 0.5f);

                        break;

                    case Type.Screen:
                        _trans.transform.DORotate(new Vector3(0f, 180f, 4f), 0.5f);
                        break;
                }

            }
            _uiGroup.SetActive(true);
        }

        _circleGuage.fillAmount = (_currentTerm / _maxTerm);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") || other.CompareTag("Staff"))
    //    {

    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().isCleaner == true)
            {
                Cleaning(true);

            }
            else
            {
                Cleaning();
            }
        }
        else if (other.CompareTag("Staff"))
        {
            Cleaning();
        }

    }

    public void Cleaning(bool isAll = false)
    {
        _cnt++;
        if (isAll)
        {
            for (int i = 0; i < _room._cleanObjects.Length; i++)
            {
                _room._cleanObjects[i].Cleaning();

            }
        }
        else
        {
            if (isClean == false)
            {
                if (_cnt % 5 == 0)
                    Managers.Game.Vibe();
                //MMVibrationManager.Haptic(HapticTypes.LightImpact);
                _currentTerm += Time.deltaTime;
                _circleGuage.fillAmount = (_currentTerm / _maxTerm);
                if (_currentTerm >= _maxTerm)
                {

                    _currentTerm = 0;
                    RoomClear(true);

                }
            }
        }


    }



}
