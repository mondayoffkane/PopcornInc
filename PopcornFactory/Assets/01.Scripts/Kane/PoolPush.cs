using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolPush : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lab_Product"))
        {
            Managers.Pool.Push(other.GetComponent<Poolable>());
        }
    }



    [Button]
    public void TestCsvPrint()
    {

        //int _exp = 0;
        //List<Dictionary<string, object>> _list = CSVReader.Read("Testfile");
        List<Dictionary<string, object>> data = CSVReader.Read("Testfile");



        for (var i = 0; i < data.Count; i++)
        {
            Debug.Log("index " + (i).ToString() + " : " + data[i]["num"] + " " + data[i]["value"]);
        }

        //_exp = (int)data[0]["attack"];
        //Debug.Log(_exp);
        //Debug.Log(data);
    }
}
