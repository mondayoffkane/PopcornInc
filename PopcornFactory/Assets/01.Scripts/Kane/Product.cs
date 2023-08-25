using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Product : MonoBehaviour
{

    public double _price = 3d;

    public int _productNum = 0;


    MeshFilter _meshFilter;
    Renderer _renderer;



    // =================================
    public void SetType(int _num, Mesh _mesh, Material _mat, double _pricevalue)
    {

        _productNum = _num;

        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = _mesh;



        if (_renderer == null) _renderer = GetComponent<Renderer>();
        _renderer.material = _mat;

        _price = _pricevalue;


    }



}
