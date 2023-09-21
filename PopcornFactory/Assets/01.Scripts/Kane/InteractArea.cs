using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class InteractArea : MonoBehaviour
{
    public enum OpenType
    {
        Unlock,
        CounterStaff,
        CleanerStaff
    }
    public OpenType _openType;

    public Transform _target;

    public double _unlockPrice;
    public double _currentPrice;
    public GameObject _money_Pref;


    public bool isPlayerIn = false;

    public float _currentTerm;


    [Title("UI")]
    public Text _priceText;
    public Image _fillImg;

    [SerializeField] Player _player;

    // ==============================================================

    private void Start()
    {

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GetComponent<Renderer>().enabled = false;

        // add load data

        _currentPrice = _unlockPrice;


        if (_priceText == null) _priceText = transform.Find("Canvas").Find("PriceText").GetComponent<Text>();
        if (_fillImg == null) _fillImg = transform.Find("Canvas").Find("Fill").GetComponent<Image>();


        _priceText.text = $"{Managers.ToCurrencyString(_currentPrice)}";
        _fillImg.fillAmount = (float)((_unlockPrice - _currentPrice) / _unlockPrice);

        StartCoroutine(Cor_Update());
    }




    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return Time.deltaTime;
            if (isPlayerIn)
            {
                if (Managers.Game.Money >= _unlockPrice * Time.deltaTime)
                {
                    Managers.Game.CalcMoney(-_unlockPrice * Time.deltaTime);
                    _currentPrice -= _unlockPrice * Time.deltaTime;

                    Transform _momey = Managers.Pool.Pop(_money_Pref).transform;
                    _priceText.text = $"{Managers.ToCurrencyString(_currentPrice)}";
                    _fillImg.fillAmount = (float)((_unlockPrice - _currentPrice) / _unlockPrice);


                    _momey.SetParent(_player.transform);
                    _momey.transform.localPosition = Vector3.zero;
                    _momey.DOJump(transform.position, 8f, 1, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        _momey.gameObject.SetActive(false);
                        Managers.Pool.Push(_momey.GetComponent<Poolable>());
                    });
                    if (_currentPrice <= 0)
                    {
                        _priceText.text = $"0";
                        _fillImg.fillAmount = 1f;


                        _currentPrice = 0;
                        isPlayerIn = false;


                        switch (_openType)
                        {
                            case OpenType.Unlock:
                                _target.GetComponent<EventObject>().CallUnlock();

                                break;
                            case OpenType.CounterStaff:
                                Managers.Game._cinemaManager.AddCounterStaff();
                                break;

                            case OpenType.CleanerStaff:
                                Managers.Game._cinemaManager.AddCleanerStaff();
                                break;
                        }

                        gameObject.SetActive(false);

                    }
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        isPlayerIn = true;
    }


    private void OnTriggerExit(Collider other)
    {
        isPlayerIn = false;
    }





}
