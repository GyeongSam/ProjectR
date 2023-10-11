using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageB_BonusItem : MonoBehaviour
{
  public StageBManager stageManager;
    // Start is called before the first frame update
    void Start()
    {
      stageManager = GameObject.Find("StageManager").GetComponent<StageBManager>();

    }
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.CompareTag("Player")&&collision.gameObject.GetComponent<StageB_PlayerController>().photonView.IsMine)
    {
      stageManager.scoreUp();
      Destroy(gameObject);
    }
    else if (collision.collider.CompareTag("Fail"))
    {
      Destroy(gameObject);
    }
  }
}
