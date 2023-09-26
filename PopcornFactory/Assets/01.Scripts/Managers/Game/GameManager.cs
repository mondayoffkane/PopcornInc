using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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
        _floatingText.localPosition = new Vector3(0f, 4f, 0f);
        _floatingText.GetComponentInChildren<Text>().text = $"{Managers.ToCurrencyString(_value)}";
        _floatingText.DOLocalMoveY(7f, 1f).SetEase(Ease.OutCirc)
            .OnComplete(() => Managers.Pool.Push(_floatingText.GetComponent<Poolable>()));
    }


}
