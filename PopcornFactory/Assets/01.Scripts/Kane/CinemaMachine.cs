using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CinemaMachine : MonoBehaviour
{

    [TitleGroup("Product")] public GameObject _cinemaProduct;
    [TitleGroup("Product")] public CinemaProduct.CinemaProductType _productType;
    [TitleGroup("Product")] public float _spawnInterval = 1f;
    [TitleGroup("Product")] public float _jumpPower = 10f;
    [TitleGroup("Product")] public float _moveSpeed = 1f;
    [TitleGroup("Product")] public Transform _stackPos;
    [TitleGroup("Product")] public int _maxCount = 5;

    [SerializeField] float _stackY;


    public Stack<CinemaProduct> _productStack;

    void Start()
    {

        _productStack = new Stack<CinemaProduct>();
        _stackY = _cinemaProduct.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

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
            _product.transform.DOJump(_stackPos.position + new Vector3(0f, _productStack.Count * _stackY, 0f), _jumpPower, 1, _moveSpeed).SetEase(Ease.Linear);

        }
    }


}
