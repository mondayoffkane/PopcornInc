using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class MoneyZone : MonoBehaviour
{

    [TitleGroup("Money")][SerializeField] GameObject _moneyPref;

    [TitleGroup("Money")]
    [SerializeField] Vector3 _stackInterval = Vector3.zero;
    [TitleGroup("Money")] public Stack<Transform> _moneyStack;
    [TitleGroup("Money")] public int _width = 3, _height = 3;


    public double _moneyPrice = 1d;

    private void OnEnable()
    {
        if (_moneyPref == null) _moneyPref = Resources.Load<GameObject>("Money_Pref");

        _stackInterval = _moneyPref.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        _moneyStack = new Stack<Transform>();

        if (transform.GetComponent<Collider>() == null)
            transform.gameObject.AddComponent<BoxCollider>().isTrigger = true;

        transform.GetComponent<BoxCollider>().size = new Vector3(5.5f, 0f, 3f);

        transform.GetComponent<Renderer>().enabled = false;
    }

    public void SetInit(Vector3 _colSize, int _Width, int _Height)
    {
        transform.GetComponent<BoxCollider>().size = _colSize;
        _width = _Width;
        _height = _Height;
    }

    [Button]
    public void PopMoney(Transform _spawnTrans, double _price = 1, int _count = 1)
    {

        _moneyPrice = _price;
        for (int i = 0; i < _count; i++)
        {
            Transform _money = Managers.Pool.Pop(_moneyPref, transform).GetComponent<Transform>();
            _money.position = _spawnTrans.position;
            _money.transform.rotation = Quaternion.Euler(Vector3.zero);
            _moneyStack.Push(_money);
            _money.DOJump(transform.position
                + new Vector3(
                    (((_moneyStack.Count - 1) % _width) - (_width / 2)) * _stackInterval.x
                , (((_moneyStack.Count - 1) / (_width * _height)) * _stackInterval.y)
                , ((((_moneyStack.Count - 1) % (_width * _height)) / _width - (_height / 2)) * _stackInterval.z)
                ), 5f, 1, 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Managers.Game.Vibe();
            //MMVibrationManager.Haptic(HapticTypes.LightImpact);
            //StartCoroutine(Cor_GetMoney());
            if (_moneyStack.Count > 0)
                Managers.Sound.Play("Money");

            while (_moneyStack.Count > 0)
            {
                Transform _money = _moneyStack.Pop().transform;
                _money.SetParent(other.transform);
                _money.DOLocalJump(Vector3.zero, 8f, 1, 0.5f + 0.5f / (_moneyStack.Count + 1)).SetEase(Ease.InCubic)
                    .OnComplete(() => Managers.Pool.Push(_money.GetComponent<Poolable>()));

                Managers.Game.CalcMoney(_moneyPrice, 1);
            }

        }
        //IEnumerator Cor_GetMoney()
        //{
        //    //int _count = _moneyStack.Count;

        //   while(_moneyStack.Count>0)
        //    {
        //        Transform _money = _moneyStack.Pop().transform;
        //        _money.SetParent(other.transform);
        //        _money.DOLocalJump(Vector3.zero, 3f, 1, 0.2f /*- (i / _count)*/).SetEase(Ease.Linear)
        //            .OnComplete(() => Managers.Pool.Push(_money.GetComponent<Poolable>()));
        //        yield return new WaitForSeconds(0.2f * (1.1f - (i / _count)));
        //    }
        //}
    }




}
