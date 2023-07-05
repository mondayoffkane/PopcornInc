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
        Corn,
        PopCorn_base,
        Popcorn_1,
        Popcorn_2,
        Popcorn_3


    }
    public ProductType _productType;


    MeshFilter _meshFilter;
    Renderer _renderer;



    // =================================
    public void SetType(ProductType _type, double _pricevalue)
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        _productType = _type;
        _price = _pricevalue;
        _meshFilter.sharedMesh = _productMeshes[(int)_productType];

        if (_renderer == null) _renderer = GetComponent<Renderer>();
        _renderer.material = Mats[(int)_productType];



    }

}
