using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaProduct : MonoBehaviour
{

    public enum CinemaProductType
    {
        Popcorn,
        Choco,
        Berry
    }
    public CinemaProductType _cinemaProductType;

    public Mesh[] _productMeshes = new Mesh[3];
    public Material[] _Mats = new Material[3];

    Material[] _tmpmaterials;

    MeshFilter _meshFilter;
    MeshRenderer _renderer;
    // ===================================



    public void Init(CinemaProductType _type)
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        if (_renderer == null) _renderer = GetComponent<MeshRenderer>();

        _cinemaProductType = _type;

        _tmpmaterials = _renderer.materials;

        _meshFilter.sharedMesh = _productMeshes[(int)_cinemaProductType];

        _tmpmaterials[1] = _Mats[(int)_cinemaProductType];
        _renderer.sharedMaterials = _tmpmaterials;
    }
}
