using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public bool isNone = false;

    public static TutorialManager _instance;


    public int _tutorialLevel;


    public Vector3[] _pos;
    public Vector2[] _size;


    // ////////////////////


    public Vector3[] _arrowpos;
    public bool isFix = false;

    public Image _mask;
    public Image _arrow;
    // =====================================================

    public GameObject[] _virCams;




    public Transform _3dArrow;
    public Transform[] _3dArrowPos;

    // ===================================
    private void OnEnable()
    {
        _instance = this;


        _mask = Managers.GameUI.Mask;
        _arrow = _mask.transform.GetChild(0).GetComponent<Image>();
        _tutorialLevel = ES3.Load<int>("_tutorialLevel", 0);

        //if (_tutorialLevel < 8)
        //{
        //    Managers.GameUI.Cinema_Button.interactable = false;
        //}

        //Tutorial();

    }


    public void Tutorial(bool isOff = true, float _camTime = 4f)
    {
        if (Managers.Game._stageManager.isCinema == false && _tutorialLevel < 8)
        {
            if (!isNone)
            {

                if (isFix == false)
                {
                    if (Managers.Game._stageManager._targetMachine_Trans != null)
                        Managers.Game._stageManager._targetMachine_Trans.GetComponent<Machine>().isPress = false;

                    //Debug.Log("tutorial Leve : " + _tutorialLevel);
                    if (isOff)
                    {
                        Managers.GameUI.OffPopup();
                    }

                    isFix = true;
                    if (_mask == null)
                    {
                        _mask = Managers.GameUI.Mask;
                        _arrow = _mask.transform.GetChild(0).GetComponent<Image>();
                    }
                    _mask.enabled = true;
                    _arrow.gameObject.SetActive(true);

                    if (_tutorialLevel == 0) _arrow.gameObject.SetActive(false);

                    _mask.rectTransform.anchoredPosition = _pos[_tutorialLevel];
                    _mask.GetComponent<RectTransform>().sizeDelta = _size[_tutorialLevel];
                    _arrow.rectTransform.anchoredPosition = _arrowpos[_tutorialLevel];

                }
            }
        }
        else if (Managers.Game._stageManager.isCinema == true && _tutorialLevel >= _pos.Length - 1) // Cinema Tutorial
        {

            int _num = _tutorialLevel - (_pos.Length - 1);
            //Debug.Log(_num);
            if (_num < _3dArrowPos.Length)
            {
                _3dArrow.gameObject.SetActive(true);
                _3dArrow.position = _3dArrowPos[_num].position;
                //Debug.Log(_num);
                //DOTween.Sequence().AppendCallback(() => _virCams[_tutorialLevel - 8].SetActive(true))
                //    .AppendInterval(_camTime).
                //    AppendCallback(() =>
                //    {
                //        _virCams[_tutorialLevel - 8].SetActive(false);
                //        Tutorial_Comple();
                //    });
            }

        }
    }

    public void Tutorial_Comple()
    {
        if (_tutorialLevel <= _pos.Length)
        {
            isFix = false;
            if (_mask == null)
            {
                _mask = Managers.GameUI.Mask;
                _arrow = _mask.transform.GetChild(0).GetComponent<Image>();
            }
            _mask.enabled = false;
            _arrow.gameObject.SetActive(false);
        }

        _3dArrow.gameObject.SetActive(false);


        _tutorialLevel++;
        ES3.Save<int>("_tutorialLevel", _tutorialLevel);




    }








}
