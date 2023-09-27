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
        public bool isFirst = true;
        //public int Stage_Level;
        //public int Staff_Upgrade_Level;
        //public int Income_Upgrade_Level;
        public int Parts_Upgrade_Level;

        //public int Speed_Upgrade_Level;
        public int PlayTime;
        public int IslandTime;
        public int CinemaTime;


    }
    public StageData _stageData;

    public StageData GetStageData(int _stageLevel = 0)
    {

        Managers.Game.Money = ES3.Load<double>("Money", 0);
        _stageData = ES3.Load<StageData>("StageData" + _stageLevel.ToString(), new StageData());
        Managers.Game.Gem = ES3.Load<int>("Gem", 0);
        Managers.Game.CinemaMoney = ES3.Load<double>("CinemaMoney", 0);


        return _stageData;

    }
    public void SetStageData(StageData _stagedata, int _stageLevel)
    {
        ES3.Save<StageData>("StageData" + _stageLevel.ToString(), _stagedata);
        ES3.Save<double>("Money", Managers.Game.Money);
        ES3.Save<int>("Gem", Managers.Game.Gem);
        ES3.Save<int>("Stage_" + _stageLevel.ToString(), _stagedata.PlayTime);
        ES3.Save<int>("IslandTime", _stagedata.IslandTime);
        ES3.Save<int>("CinemaTime", _stagedata.CinemaTime);
        ES3.Save<double>("CinemaMoney", Managers.Game.CinemaMoney);

    }
    // =====================================
    public class MachineData
    {
        public int Machin_Level;
        public int PriceScope_Level;
        public int Spawn_Level;
    }
    public MachineData _machineData;

    public MachineData GetMachineData(int _stageLevel, string _machineName)
    {
        MachineData loadMachineData = ES3.Load<MachineData>("Stage_" + _stageLevel.ToString() + "_" + _machineName, new MachineData());


        return loadMachineData;
    }

    public void SetMachineData(int _stageLevel, string _machineName, MachineData _data)
    {
        ES3.Save<MachineData>("Stage_" + _stageLevel.ToString() + "_" + _machineName, _data);
    }





}