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

    MeshFilter _meshFilter;

    // ===================================

    public void OnEnable()
    {

        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
    }

    public void Init(CinemaProductType _type)
    {
        _cinemaProductType = _type;


        _meshFilter.sharedMesh = _productMeshes[(int)_cinemaProductType];

    }
}
