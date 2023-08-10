using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class RunningStageSpawner : MonoBehaviour
{
    public GameObject _roadPref;
    public int _roadCount = 10;
    float _roadInterval;

    public GameObject _gatePref;
    public int _gateCount = 10;
    public float _gateInterval = 10f;



    Transform _roadGroup;
    Transform _gateGroup;




    [Button]
    public void SpawnMap()
    {

        //_roadInterval = _roadPref.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

        //if (_roadGroup == null)
        //{
        //    _roadGroup = new GameObject("_roadGroup").transform;
        //    _roadGroup.SetParent(transform);
        //}
        //else
        //{
        //    DestroyImmediate(_roadGroup.gameObject);
        //    _roadGroup = new GameObject("_roadGroup").transform;
        //    _roadGroup.SetParent(transform);
        //    //for (int i = 0; i < _roadGroup.childCount; i++)
        //    //{
        //    //    DestroyImmediate(_roadGroup.GetChild(0).gameObject);
        //    //}

        //}

        //if (_gateGroup == null)
        //{
        //    _gateGroup = new GameObject("_gateGroup").transform;
        //    _gateGroup.SetParent(transform);
        //}
        //else
        //{
        //    DestroyImmediate(_gateGroup.gameObject);
        //    _gateGroup = new GameObject("_gateGroup").transform;
        //    _gateGroup.SetParent(transform);
        //    //for (int i = 0; i < _gateGroup.childCount; i++)
        //    //{
        //    //    DestroyImmediate(_gateGroup.GetChild(0).gameObject);
        //    //}

        //}



        //for (int i = 0; i < _roadCount; i++)
        //{

        //    GameObject _road = PrefabUtility.InstantiatePrefab(_roadPref) as GameObject;
        //    _road.transform.SetParent(_roadGroup);
        //    _road.transform.localPosition = new Vector3(0f, 0f, _roadInterval * i);
        //}

        //for (int i = 0; i < _gateCount; i++)
        //{
        //    GameObject _gate = PrefabUtility.InstantiatePrefab(_gatePref) as GameObject;
        //    _gate.transform.SetParent(_gateGroup);
        //    _gate.transform.localPosition = new Vector3(Random.Range(-4.5f, 4.5f), 0f, _gateInterval * (i + 1));
        //}



    }




}
