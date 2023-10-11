using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChar : MonoBehaviour
{
  public Transform obj;

    // Update is called once per frame
    void Update()
    {
      if (obj != null)
      {
        gameObject.transform.position = obj.position + new Vector3(0, 1.5f, 0);
      }
    }
}
