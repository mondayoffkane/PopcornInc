using System.Collections.Generic;
using DG.Tweening;
using MondayOFF;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class GameManager : MonoBehaviour
{

    [HideInInspector]
    public JoyStickController JoyStickController;
    public void SetDownAction(System.Action action)
    {
        JoyStickController?.AddDownEvent(action);
    }
    public void SetUpAction(System.Action action)
    {
        JoyStickController?.AddUpEvent(action);
    }
    public void SetMoveAction(System.Action<Vector2> action)
    {
        JoyStickController?.AddMoveEvent(action);
    }


    public void Init()
    {
        //_stageManager = SpawnStage();
        _stageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManager>();
        _FloatingText = Resources.Load<GameObject>("Floating");

        //_labotoryManager = GameObject.FindGameObjectWithTag("LabotoryManager").GetComponent<LabotoryManager>();
        _cinemaManager = GameObject.FindGameObjectWithTag("CinemaManager").GetComponent<CinemaManager>();



        MondayOFF.IAPManager.RegisterProduct("popcorninc_starterpack", BuyStarterPack);
        MondayOFF.IAPManager.RegisterProduct("popcorninc_cleanpack", BuyCleanPack);
        MondayOFF.IAPManager.RegisterProduct("popcorninc_gempack_1", BuyGemPack1);
        MondayOFF.IAPManager.RegisterProduct("popcorninc_gempack_2", BuyGemPack2);
        MondayOFF.IAPManager.RegisterProduct("popcorninc_gempack_3", BuyGemPack3);
        MondayOFF.IAPManager.RegisterProduct("popcorninc_upgradepack", BuyUpgradePack);

        MondayOFF.IAPManager.OnAfterPurchase += (isSuccess) => Debug.Log("구매 완료!");

    }
    private void BuyStarterPack()
    {
        _cinemaManager._player._cleaner_iap = true;
        _cinemaManager._player._isBuyCleanPack = true;
        _cinemaManager._player._cleanerObj.SetActive(true);
        _cinemaManager._player.isCleaner = true;
        _StageManager._noAds = 1;

        AdsManager.DisableAdType(AdType.Banner | AdType.Interstitial);
        PlayerPrefs.SetInt("NoAds", 1);

        if (Managers.Game._stageManager._noAds == 0)
        {
            Managers.GameUI.CleanPack.SetActive(false);
            Managers.GameUI.StarterPack.SetActive(true);
        }

        if (Managers.Game._cinemaManager._player._isBuyCleanPack)
        {
            Managers.GameUI.CleanPack.SetActive(false);
            Managers.GameUI.StarterPack.SetActive(false);
        }

        _cinemaManager._player.SaveData();




        // Disable RV, PlayOn, Adverty too?
        MondayOFF.NoAds.OnNoAds?.Invoke();

        PlayerPrefs.SetInt(MondayOFF.NoAds.NoAdsProductKey, 1);
        PlayerPrefs.Save();


        CalcGem(40);

        //Debug.Log("Starter Pack");
        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "StarterPack" } });
    }

    private void BuyCleanPack()
    {
        _cinemaManager._player._cleaner_iap = true;
        _cinemaManager._player._isBuyCleanPack = true;
        _cinemaManager._player._cleanerObj.SetActive(true);
        _cinemaManager._player.isCleaner = true;
        _cinemaManager._player.SaveData();
        CalcMoney(500, 1);
        CalcGem(40);
        //Debug.Log("Clean Pack");

        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "CleanPack" } });

    }

    void BuyUpgradePack()
    {
        _cinemaManager._player._speed_iap = true;
        _cinemaManager._player._capacity_iap = true;
        _cinemaManager._player._maxCount = 4;
        _cinemaManager._player.SaveData();

        _cinemaManager._player._speed = 12;
        _cinemaManager._joystick.Speed = 12;

        //Debug.Log("Upgrade Pack ");
        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "UpgradePack" } });
    }

    void BuyGemPack1()
    {
        CalcGem(20);
        //Debug.Log("Gem Pack 1 _ 20");
        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "Gem_1" } });
    }
    void BuyGemPack2()
    {
        CalcGem(50);
        //Debug.Log("Gem Pack 2 _ 50");
        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "Gem_2" } });
    }
    void BuyGemPack3()
    {
        CalcGem(100);
        //Debug.Log("Gem Pack 3 _ 100");
        EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "Product", "Gem_3" } });
    }



    public void Clear()
    {
        if (JoyStickController != null)
        {
            JoyStickController.DownAction = null;
            JoyStickController.UpAction = null;
            JoyStickController.JoystickMoveAction = null;
        }
    }

    /////////// ==============================


    [SerializeField] StageManager _StageManager;
    public StageManager _stageManager
    {
        get
        {
            if (_StageManager == null)
            {
                //_StageManager = SpawnStage();
                _StageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManager>();
            }
            return _StageManager;
        }
        set
        {

            _StageManager = value;
        }
    }

    public CinemaManager _cinemaManager;
    //public LabotoryManager _labotoryManager;




    //public void NextStage()
    //{

    //    Destroy(_stageManager.gameObject);

    //    _lastStageLevel++;
    //    Money = 0;

    //    ES3.Save<int>("LastStageLevel", _lastStageLevel);
    //    ES3.Save<double>("Money", Money);

    //    //_stageManager = SpawnStage();
    //}

    public int _lastStageLevel;

    public double Money = 0f;
    public int Gem = 0;

    public double CinemaMoney = 0f;


    public GameObject _FloatingText;

    public void AddMoney(double _value)
    {
        Money += _value;
        Managers.GameUI.Money_Text.text = $"{Managers.ToCurrencyString(Money, 2)}";

        ES3.Save<double>("Money", Money);
        _stageManager.CheckButton();
    }

    public void AddCinemaMoney(double _value)
    {
        CinemaMoney += _value;
        Managers.GameUI.CinemaMoney_Text.text = $"{Managers.ToCurrencyString(CinemaMoney, 2)}";

        ES3.Save<double>("CinemaMoney", CinemaMoney);
        //_stageManager.CheckButton();
    }



    public void CalcMoney(double _value, int _moneyType = 0)
    {
        switch (_moneyType)
        {
            case 0:

                if (_value > 0)
                {
                    double _double = _stageManager.isRvDouble ? 2d : 1d;
                    Money += _value * _double;
                }
                else
                {
                    Money += _value;
                }

                Managers.GameUI.Money_Text.text = $"{Managers.ToCurrencyString(Money, 2)}";
                ES3.Save<double>("Money", Money);
                _stageManager.CheckButton();
                break;

            case 1:
                if (_value > 0)
                {
                    double _double = _cinemaManager.isRvDouble ? 2d : 1d;
                    CinemaMoney += _value * _double;
                }
                else
                {
                    CinemaMoney += _value;
                }

                Managers.GameUI.CinemaMoney_Text.text = $"{Managers.ToCurrencyString(CinemaMoney, 2)}";
                ES3.Save<double>("CinemaMoney", CinemaMoney);


                break;

        }



    }

    public void CalcGem(int _value)
    {
        Gem += _value;
        Managers.GameUI.Gem_Text.text = $"{Gem.ToString()}";
        ES3.Save<int>("Gem", Managers.Game.Gem);
    }


    public void PopText(double _value, Transform _trans)
    {
        double _double = _stageManager.isRvDouble ? 2d : 1d;
        _value *= _double;

        Transform _floatingText = Managers.Pool.Pop(_FloatingText, _trans).GetComponent<Transform>();
        _floatingText.rotation = Quaternion.Euler(45f, 0f, 0f);
        _floatingText.localPosition = new Vector3(0f, 4f, 0f);
        _floatingText.GetComponentInChildren<Text>().text = $"{Managers.ToCurrencyString(_value)}";
        _floatingText.DOLocalMoveY(7f, 1f).SetEase(Ease.OutCirc)
            .OnComplete(() => Managers.Pool.Push(_floatingText.GetComponent<Poolable>()));
    }

    public bool CanUseGem(int _count)
    {
        if (Gem > _count)
        {
            CalcGem(-_count);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Vibe(int _num = 4)
    {
        if (Managers.Data.UseHaptic)
            MMVibrationManager.Haptic((HapticTypes)_num);
    }


}
