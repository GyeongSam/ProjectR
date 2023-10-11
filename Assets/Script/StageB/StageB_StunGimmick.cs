using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageB_StunGimmick : MonoBehaviour
{
  public StageBManager stageManager;
  public bool isStatic = false;

  private void Start()
  {
    if (isStatic)
    {
      Destroy(gameObject, 5f);
    }
  }
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.CompareTag("Player"))
    {
      collision.gameObject.GetComponent<StageB_PlayerController>().GetStuned();
      Destroy(gameObject);
    }
    else if (collision.collider.CompareTag("Fail"))
    {
      Destroy(gameObject);
    }
  }
}
