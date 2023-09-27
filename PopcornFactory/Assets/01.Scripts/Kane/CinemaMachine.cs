using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CinemaMachine : MonoBehaviour
{

    public GameObject _lockImg;


    [TitleGroup("Product")] public GameObject _cinemaProduct;
    [TitleGroup("Product")] public CinemaProduct.CinemaProductType _productType;
    [TitleGroup("Product")] public float _spawnInterval = 1f;
    [TitleGroup("Product")] public float _jumpPower = 10f;
    [TitleGroup("Product")] public float _moveSpeed = 1f;
    [TitleGroup("Product")] public Transform _stackPos;
    [TitleGroup("Product")] public int _maxCount = 5;

    public Transform _checkZone;
    [SerializeField] float _stackTerm;


    public Stack<CinemaProduct> _productStack;

    StageManager _stageManager;

    private void OnEnable()
    {
        if (_lockImg != null)
            _lockImg.SetActive(false);
    }


    /// <summary>
    ///  ================================================
    /// </summary>
    ///

    void Start()
    {

        _productStack = new Stack<CinemaProduct>();
        _stackTerm = _cinemaProduct.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

        if (_stackPos == null) _stackPos = transform.Find("StackPos");
        _stackPos.GetComponent<Renderer>().enabled = false;
        StartCoroutine(Cor_Update());


    }



    IEnumerator Cor_Update()
    {
        while (true)
        {
            SpawnProduct();


            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    public void SpawnProduct()
    {
        if (_productStack.Count < _maxCount)
        {
            CinemaProduct _product = Managers.Pool.Pop(_cinemaProduct, transform).GetComponent<CinemaProduct>();

            _product.Init(_productType);
            _productStack.Push(_product);
            _product.transform.position = transform.position;
            _product.transform.SetParent(_stackPos);
            //_product.transform.DOJump(_stackPos.position + new Vector3(0f, _productStack.Count * _stackTerm, 0f), _jumpPower, 1, _moveSpeed).SetEase(Ease.Linear);
            _product.transform.DOJump(_stackPos.position + new Vector3(0f, 0f, -(_productStack.Count - 1) * _stackTerm - 0.3f), _jumpPower, 1, _moveSpeed).SetEase(Ease.Linear);

            _product.transform.DOLocalJump(Vector3.right * (_productStack.Count - 1) * _stackTerm, _jumpPower, 1, _moveSpeed).SetEase(Ease.Linear);


        }
    }


}
