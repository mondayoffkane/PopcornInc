using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Product : MonoBehaviour
{

    public double _price = 3d;

    public Mesh[] _productMeshes;
    public Material[] Mats; // 0 : stage , 1 : base popcorn, 2~ other popcorns
    public enum ProductType
    {
        BaseCorn,
        KernelCorn,
        MixCorn,
        PopCorn_base,

        Corn,
        Cacao

    }
    public ProductType _productType;
    public int _productNum = 0;


    MeshFilter _meshFilter;
    Renderer _renderer;



    // =================================
    public void SetType(ProductType _type/*, int _num,*/, double _pricevalue)
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        _productType = _type;
        //_productNum = _num;
        _price = _pricevalue;

        //Mesh _mesh = 

        _meshFilter.sharedMesh = _productMeshes[(int)_productType];

        if (_renderer == null) _renderer = GetComponent<Renderer>();
        _renderer.material = Mats[(int)_productType];



    }

}
