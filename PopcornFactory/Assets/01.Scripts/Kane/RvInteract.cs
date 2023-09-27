using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using DG.Tweening;


public class RvInteract : MonoBehaviour
{
    public Mesh[] _RvMeshes;

    [SerializeField] MeshFilter _objMeshFilter;

    public enum RvType
    {
        Speed,
        BigMoney,
        Glove,
        Cleaner
    }
    public RvType _rvType;

    public bool isPlayerIn = false;



    [SerializeField] Player _player;

    public float _remainTime = 40f;
    // ===================================================



    private void OnEnable()
    {
        DOTween.Sequence().AppendInterval(_remainTime).
             AppendCallback(() => Managers.Pool.Push(transform.GetComponent<Poolable>()));



    }


    public void SetRvType(RvType _type = RvType.Speed)
    {

        //if (_fillImg == null) _fillImg = transform.Find("Canvas").Find("Fill").GetComponent<Image>();
        //_fillImg.fillAmount = 0f;

        if (_objMeshFilter == null) _objMeshFilter = transform.Find("ShowObj").GetComponent<MeshFilter>();



        _rvType = _type;
        _objMeshFilter.sharedMesh = _RvMeshes[(int)_rvType];



        //StartCoroutine(Cor_Update());
    }

    private void Update()
    {
        if (isPlayerIn)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isPlayerIn = false;
                ShowRvPanel();
            }

        }
    }


    //IEnumerator Cor_Update()
    //{
    //    while (true)
    //    {
    //        yield return Time.deltaTime;

    //        if (isPlayerIn)
    //        {
    //            _currentTime += Time.deltaTime;
    //            _fillImg.fillAmount = (_currentTime / _maxTime);

    //            if (_currentTime >= _maxTime)
    //            {
    //                isPlayerIn = false;
    //                _currentTime = 0f;
    //                ShowRvPanel();
    //            }
    //        }

    //    }
    //}

    public void ShowRvPanel()
    {
        Managers.GameUI.ShowCinemaRvPanel((int)_rvType);
        Managers.Game._cinemaManager._cinemaRvNum = (int)_rvType;

        Managers.Game._cinemaManager._rvInteractTarget = this;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }









}
