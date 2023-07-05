using UnityEngine;

[System.Serializable]
public class DataManager
{
    ///<summary>Manager생산할때 만들어짐</summary>
    public void Init()
    {
        _useHaptic = ES3.Load<bool>("Haptic", true);
        _useSound = ES3.Load<bool>("Sound", true);
    }


    public bool UseHaptic
    {
        get => _useHaptic;
        set
        {
            _useHaptic = value;
            SaveData("Haptic", value);
        }
    }
    [SerializeField]
    private bool _useHaptic;

    public bool UseSound
    {
        get => _useSound;
        set
        {
            _useSound = value;
            SaveData("Sound", value);
            Managers.Sound.BgmOnOff(value);
        }
    }
    [SerializeField]
    private bool _useSound;

    public void SaveData<T>(string key, T data)
    {
        ES3.Save(key, data);
    }

    public T GetData<T>(string key, T _default = default(T))
    {
        return ES3.Load<T>(key, _default);
    }


    public class StageData
    {
        public int Stage_Level;
        public int Staff_Upgrade_Level;
        public int Income_Upgrade_Level;
        public int Parts_Upgrade_Level;

        public int Speed_Upgrade_Level;
        public int Cutting_Income_Level;
        public int Cutting_Speed_Level;
        public int Popcorn_Income_Level;
        public int Popcorn_Speed_Level;
        public int Seasoning_Income_Level;
        public int Seasoning_Speed_Level;

    }
    public StageData _stageData;

    public StageData GetStageData(int _stageLevel = 0)
    {

        Managers.Game.Money = ES3.Load<double>("Money", 0);
        _stageData = ES3.Load<StageData>("StageData" + _stageLevel.ToString(), new StageData());
        Managers.Game.Gem = ES3.Load<int>("Gem", 0);

        return _stageData;

    }
    public void SetStageData(StageData _stagedata)
    {
        ES3.Save<StageData>("StageData" + _stagedata.Stage_Level.ToString(), _stagedata);
        ES3.Save<double>("Money", Managers.Game.Money);
        ES3.Save<int>("Gem", Managers.Game.Gem);
    }


}