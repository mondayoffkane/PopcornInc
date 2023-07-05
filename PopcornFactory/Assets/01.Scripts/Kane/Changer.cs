using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changer : MonoBehaviour
{

    public Mesh _chagneMesh;
    public Material _changeMat;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CPI_Obj"))
        {
            other.GetComponent<MeshFilter>().sharedMesh = _chagneMesh;
            other.GetComponent<Renderer>().material = _changeMat;
        }
    }

}
