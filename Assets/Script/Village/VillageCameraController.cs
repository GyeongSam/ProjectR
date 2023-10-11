using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageCameraController : MonoBehaviour
{
    public GameObject player;
    public float xmove = 0;
    public float ymove = 0;
    public float distance = 5;
    // Update is called once per frame


    void LateUpdate()
    {
        /*if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X");
            ymove -= Input.GetAxis("Mouse Y");
        }
        transform.rotation = Quaternion.Euler(ymove, xmove, 0);*/
        Vector3 reserveDistance = new Vector3(0.0f, -distance, distance+2);

        transform.position = player.transform.position - transform.rotation * reserveDistance;
    }
}
