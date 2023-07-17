using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{

    public float value;
    public Material[] _gateMat;

    Renderer _renderer;

    Text _valueText;

    public enum GateType
    {
        Damage,
        Range,
        FireRate
    }
    public GateType _gateType;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (GetComponent<Rigidbody>() == null)
        {
            transform.gameObject.AddComponent<Rigidbody>();
        }
        GetComponent<Rigidbody>().useGravity = false;
        if (_valueText == null) _valueText = transform.Find("Canvas").Find("valueText").GetComponent<Text>();

        AddValue(0);
    }

    public void AddValue(float _val)
    {
        value += _val;
        if (value >= 0)
        {
            _renderer.sharedMaterial = _gateMat[0];
            _valueText.text = $"{_gateType.ToString()}\n+{value:0}";
        }
        else
        {
            _renderer.sharedMaterial = _gateMat[1];
            _valueText.text = $"{_gateType.ToString()}\n{value:0}";
        }


    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            AddValue(other.GetComponent<Bullet>()._damage);
        }
    }

}
