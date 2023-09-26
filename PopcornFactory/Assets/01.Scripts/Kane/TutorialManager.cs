using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool isNone = false;

    public static TutorialManager _instance;


    public int _tutorialLevel;


    public Vector3[] _pos;
    public Vector2[] _size;

    public Vector3[] _arrowpos;
    public bool isFix = false;

    public Image _mask;
    public Image _arrow;

    public Transform _3dArrow;


    // ===================================
    private void OnEnable()
    {
        _instance = this;
    }


    private void Start()
    {

        _mask = Managers.GameUI.Mask;
        _arrow = _mask.transform.GetChild(0).GetComponent<Image>();
        _tutorialLevel = ES3.Load<int>("_tutorialLevel", 0);



        //Tutorial();

    }


    public void Tutorial(bool isOff = true)
    {
        if (!isNone)
        {

            if (isFix == false)
            {
                if (Managers.Game._stageManager._targetMachine_Trans != null)
                    Managers.Game._stageManager._targetMachine_Trans.GetComponent<Machine>().isPress = false;

                Debug.Log("tutorial Leve : " + _tutorialLevel);
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

    public void Tutorial_Comple()
    {
        isFix = false;
        if (_mask == null)
        {
            _mask = Managers.GameUI.Mask;
            _arrow = _mask.transform.GetChild(0).GetComponent<Image>();
        }
        _mask.enabled = false;
        _arrow.gameObject.SetActive(false);

        _tutorialLevel++;
        ES3.Save<int>("_tutorialLevel", _tutorialLevel);

    }






}
