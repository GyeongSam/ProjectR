using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
  [SerializeField]
  /*Vector3 RespawnPos = new Vector3(107.88f, -119.5f, 346.34f);*/

  public float[] RespawnXPos = new float[] { 107.88f, 1005.808f, 174.2501f };
  public float[] RespawnYPos = new float[] { -119.5f, -156.9288f, -147.6431f };
  public float[] RespawnZPos = new float[] { 346.34f, 542.2339f, -468.1222f };

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      Rigidbody rb = other.transform.parent.GetComponent<Rigidbody>();
      rb.AddForce(new Vector3(0, 0, 0));

      int rand = UnityEngine.Random.Range(0, 3);
      Vector3 RespawnPos = new Vector3(RespawnXPos[rand], RespawnYPos[rand], RespawnZPos[rand]);

      //GameObject.FindGameObjectWithTag("Player").transform.position = RespawnPos;
      other.transform.parent.transform.position = RespawnPos;

    }
  }
}
