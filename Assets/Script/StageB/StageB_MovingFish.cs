using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageB_MovingFish : MonoBehaviour
{
  Vector3 v3;
    // Start is called before the first frame update
    void Start()
    {
        v3 = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
    transform.localPosition = new Vector3(v3.x + Mathf.Sin(Time.time), v3.y, v3.z+ Mathf.Cos(Time.time));
    }
}
