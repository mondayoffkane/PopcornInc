using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CleanObject : MonoBehaviour
{

    public bool isClean = true;

    [SerializeField] float _maxTerm = 1f;
    [SerializeField] float _currentTerm = 0f;

    [Header("UI")] public Image _circleGuage;

    public Room _room;

    public Transform[] _objs;

    public GameObject _uiGroup;

    /// =============================================

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
                _trans.transform.DORotate(Vector3.up * 180f, 0.5f);
            }
            _room.ClearObj( /*num */);
            _uiGroup.SetActive(false);
        }
        else
        {
            foreach (Transform _trans in _objs)
            {
                _trans.transform.DORotate(Vector3.up * Random.Range(90f, 270f), 0.5f);
            }
            _uiGroup.SetActive(true);
        }

        _circleGuage.fillAmount = (_currentTerm / _maxTerm);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Staff"))
        {

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Staff"))
        {
            Cleaning();
        }

    }

    public void Cleaning()
    {
        if (isClean == false)
        {

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
