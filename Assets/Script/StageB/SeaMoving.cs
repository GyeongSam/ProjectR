using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeaMoving : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
      transform.localPosition = new Vector3(transform.localPosition.x, 9f + Mathf.Sin(Time.time), transform.localPosition.z);
    }


}
