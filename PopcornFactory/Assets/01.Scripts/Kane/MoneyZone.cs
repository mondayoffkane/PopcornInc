using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoneyZone : MonoBehaviour
{

    [TitleGroup("Money")][SerializeField] GameObject _moneyPref;

    [TitleGroup("Money")]
    [SerializeField] Vector3 _stackInterval = Vector3.zero;
    [TitleGroup("Money")] public Stack<Transform> _moneyStack;
    [TitleGroup("Money")] int _width = 3, _height = 4;



    private void OnEnable()
    {
        if (_moneyPref == null) _moneyPref = Resources.Load<GameObject>("Money_Pref");

        _stackInterval = _moneyPref.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        _moneyStack = new Stack<Transform>();

        if (transform.GetComponent<Collider>() == null)
            transform.gameObject.AddComponent<BoxCollider>().isTrigger = true;
    }

    [Button]
    public void PopMoney(int _count = 3)
    {


        for (int i = 0; i < _count; i++)
        {
            Transform _money = Managers.Pool.Pop(_moneyPref, transform).GetComponent<Transform>();
            _money.position = transform.position;
            _money.transform.rotation = Quaternion.Euler(Vector3.zero);
            _moneyStack.Push(_money);
            _money.DOJump(transform.position
                + new Vector3(
                    (((_moneyStack.Count - 1) % _width) - 1) * _stackInterval.x
                , (((_moneyStack.Count - 1) / (_width * _height)) * _stackInterval.y)
                , ((((_moneyStack.Count - 1) % (_width * _height)) / _width - 1) * _stackInterval.z)
                ), 5f, 1, 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //StartCoroutine(Cor_GetMoney());
            while (_moneyStack.Count > 0)
            {
                Transform _money = _moneyStack.Pop().transform;
                _money.SetParent(other.transform);
                _money.DOLocalJump(Vector3.zero, 8f, 1, 0.5f + 0.5f / (_moneyStack.Count + 1)).SetEase(Ease.InCubic)
                    .OnComplete(() => Managers.Pool.Push(_money.GetComponent<Poolable>()));

                Managers.Game.CalcMoney(1d, 1);
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
