using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
  public static GameObject LocalPlayerInstance;
  public float movespeed = 5.0f;
  public float jumpspeed = 250f;
  public int playerDir = 0 ;
  public bool is_floor = false;
  public bool is_jump = true;
  public bool canUseUpgradeItem = false;

  public StageDManager.MarkFloorCallback markCallback;

  private GameObject itemButton;
  private Vector3[] dirList = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
  private Rigidbody rigidBody;
  Smooth.SmoothSyncPUN2 smpun2;
  // Start is called before the first frame update
  void Start()
  {
    rigidBody = GetComponent<Rigidbody>();
    smpun2 = GetComponent<Smooth.SmoothSyncPUN2>();
    if (!photonView.IsMine) return;
    itemButton = GameObject.Find("ItemButton");
    GameObject.Find("me").GetComponent<MyChar>().obj = transform;
  }


  public void getHitByBomb(Vector3 v3)
  {
    is_jump = true;
    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(v3.x, v3.z)) < 0.1) {
      Debug.Log("wow");
      rigidBody.AddForce(new Vector3(1.5f, 10, -1.5f).normalized * 2500f);
    }
    else rigidBody.AddForce((transform.position - v3 + new Vector3(0, 3, 0)).normalized * 2500f);
  }
  // Update is called once per frame
  private void FixedUpdate()
  {
    if (!photonView.IsMine) return;
    smpun2.teleportOwnedObjectFromOwner();
  }

  void Update()
  {

    if (!photonView.IsMine) return;
    //smpun2.teleportOwnedObjectFromOwner();
    if (is_floor && !is_jump)
    {

      if (Input.GetKeyDown(KeyCode.Space))
      {
        is_jump = true;
        rigidBody.AddForce((dirList[playerDir] + new Vector3(0, 3, 0)) * jumpspeed);
      }
      else if (Input.GetKey(KeyCode.W))
      {
        playerDir = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidBody.velocity = dirList[playerDir] * movespeed;
      }
      else if (Input.GetKey(KeyCode.D))
      {
        playerDir = 1;
        transform.rotation = Quaternion.Euler(0, 90, 0);
        rigidBody.velocity = dirList[playerDir] * movespeed;
      }
      else if (Input.GetKey(KeyCode.S))
      {
        playerDir = 2;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        rigidBody.velocity = dirList[playerDir] * movespeed;
      }
      else if (Input.GetKey(KeyCode.A))
      {
        playerDir = 3;
        transform.rotation = Quaternion.Euler(0, 270, 0);
        rigidBody.velocity = dirList[playerDir] * movespeed;
      }
      if (canUseUpgradeItem && is_floor && Input.GetKeyDown(KeyCode.F))
      {
        Debug.Log("Upgrade!!!!!!!!!!!!!");
        Collider[] colliders = Physics.OverlapBox(transform.position, GetComponent<SphereCollider>().center, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
          if (collider.CompareTag("Floor"))
          {
            collider.gameObject.GetComponent<FloorScript>().dN(1);
            itemButton.transform.GetChild(0).gameObject.SetActive(false);
            itemButton.transform.GetChild(1).gameObject.SetActive(true);
            canUseUpgradeItem = false;
            //photonView.RPC("MarkFloorUpgradedRPC", RpcTarget.MasterClient, collider.gameObject.transform.position);
            break;
          }
        }
      }

    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Player") || collision.collider.CompareTag("Bomb"))
    {
      is_jump = false;
    }
    if (!photonView.IsMine) return;
    if (collision.collider.CompareTag("Finish"))
    {
        Debug.Log("Finish");
        GameObject.Find("StageManager").GetComponent<StageDManager>().uiControll();        
    }
  }
  
  [PunRPC]
  void MarkFloorUpgradedRPC(Vector3 position)
  {
    markCallback(position, StageDManager.MarkType.Upgrade);
  }
}
