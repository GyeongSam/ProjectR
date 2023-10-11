using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageB_SpeedGimmick : MonoBehaviour
{
  public StageBManager stageManager;
  public float c = 1f;
  public bool isStatic = false;

  private void Start()
  {
    if (isStatic)
    {
      Destroy(gameObject,5f);
    }
  }
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.CompareTag("Player"))
    {
      collision.gameObject.GetComponent<StageB_PlayerController>().SpeedChange(c);
      Destroy(gameObject);
    }
    else if (collision.collider.CompareTag("Sea"))
    {
      Destroy(gameObject);
    }
  }
}
