using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ResetPos : MonoBehaviour
{

    public int _count;

    public Transform _group;

    public Transform[] _objs;
    public Vector3[] _pos;
    public Vector3[] _rot;
    // Start is called before the first frame update
    void Start()
    {
        //Set_Pos();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Q))
        {
            Reset_Pos();
        }

    }

    [Button]
    public void Set_Pos()
    {
        //Debug.Log("reset");
        _count = _group.transform.childCount;

        _objs = new Transform[_count];
        _pos = new Vector3[_count];
        _rot = new Vector3[_count];


        for (int i = 0; i < _count; i++)
        {
            _objs[i] = _group.transform.GetChild(i);
            _pos[i] = _objs[i].localPosition;
            _rot[i] = _objs[i].eulerAngles;
        }
    }

    public void Reset_Pos()
    {

        for (int i = 0; i < _count; i++)
        {
            _objs[i].transform.SetParent(_group);
            _objs[i].localPosition = _pos[i];
            _objs[i].eulerAngles = _rot[i];
            _objs[i].GetComponent<Rigidbody>().isKinematic = true;
            _objs[i].transform.localScale = Vector3.one;
        }
    }
}
