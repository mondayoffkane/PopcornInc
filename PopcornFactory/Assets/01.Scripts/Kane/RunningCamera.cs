using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningCamera : MonoBehaviour
{

    public Transform Player;
    public Vector3 Offset;
    public float Limit_x = 4f;
    public float Follow_Speed = 5f;
    [SerializeField] Vector3 temp_pos;
    [SerializeField] Vector3 Rot;



    // Start is called before the first frame update



    void Start()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Rot = transform.rotation.eulerAngles;

        Offset = transform.position - Player.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        temp_pos = Vector3.Lerp(transform.position, Player.position + Offset, Time.deltaTime * Follow_Speed);
        transform.position = new Vector3(temp_pos.x, temp_pos.y, Player.position.z + Offset.z);
        transform.rotation = Quaternion.Euler(Rot);

        if (transform.position.x < -Limit_x)
        {
            transform.position = new Vector3(-Limit_x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > Limit_x)
        {
            transform.position = new Vector3(Limit_x, transform.position.y, transform.position.z);
        }
    }
}
