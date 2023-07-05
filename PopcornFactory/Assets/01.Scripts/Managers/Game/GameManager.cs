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
        _stageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManager>();
        _FloatingText = Resources.Load<GameObject>("Floating");
        //CalcMoney(0);
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

    public StageManager _stageManager;

    public double Money = 0f;
    public int Gem = 0;
    public GameObject _FloatingText;

    public void CalcMoney(double _value)
    {
        Money += _value > 0 ? _value * (_stageManager._income_ugrade_level + 1) : _value;
        //Money += _value * (_stageManager._income_ugrade_level + 1);
        Managers.GameUI.Money_Text.text = $"{Managers.ToCurrencyString(Money)}";
        //Managers.
        ES3.Save<double>("Money", Managers.Game.Money);
        _stageManager.CheckButton();


    }

    public void CalcGem(int _value)
    {
        Gem += _value;
        Managers.GameUI.Gem_Text.text = $"{Gem.ToString()}";
        ES3.Save<int>("Gem", Managers.Game.Gem);
    }


    public void PopText(string _str, Transform _trans)
    {
        Transform _floatingText = Managers.Pool.Pop(_FloatingText, _trans).GetComponent<Transform>();
        _floatingText.localPosition = new Vector3(0f, 3f, 0f);
        _floatingText.GetComponentInChildren<Text>().text = $"{_str}";
        _floatingText.DOLocalMoveY(4f, 1f).SetEase(Ease.Linear)
            .OnComplete(() => Managers.Pool.Push(_floatingText.GetComponent<Poolable>()));
    }


}
