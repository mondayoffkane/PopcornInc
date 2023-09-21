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


    /// =============================================

    private void Start()
    {
        //RoomClear(true);
        _circleGuage.gameObject.SetActive(false);
        _circleGuage.fillAmount = 0f;
    }


    public void RoomClear(bool _isClean)
    {
        isClean = _isClean;
        _circleGuage.gameObject.SetActive(!isClean);

        if (_isClean)
        {
            foreach (Transform _trans in _objs)
            {
                _trans.transform.DORotate(Vector3.zero, 0.5f);
            }
            _room.ClearObj( /*num */);
        }
        else
        {
            foreach (Transform _trans in _objs)
            {
                _trans.transform.DORotate(Vector3.up * Random.Range(-180f, 180f), 0.5f);
            }
        }

        _circleGuage.fillAmount = (_currentTerm / _maxTerm);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Staff"))
        {

            _circleGuage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Staff"))
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



}
