using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class VillageManager : IStageManager, IInstanceManager
{
  public GameObject playerInstance;
  public GameObject mainCamera;

  Vector3 initialSpwanPoint = new Vector3(107.88f, -119.5f, 346.34f);

  void Start()
  {
    playerInstance = IInstanceManager.InstantiatePlayerCharacter(PlayerPrefs.GetInt("nowEggNum"), initialSpwanPoint, Quaternion.identity); ;
    SetPlayerInstanceAttributes(playerInstance);
    mainCamera.GetComponent<CameraControllerVillage>().SetupCam(playerInstance);
    photonView.RPC("SetPlayerInstanceAttributesRPC", RpcTarget.AllBufferedViaServer, playerInstance.GetPhotonView().ViewID);
  }

   


  private void SetPlayerInstanceAttributes(GameObject playerInstance)
  {
    GameObject foot = AttachEmptyObject("Foot", playerInstance);
    GameObject foot_GroundCheck = AttachEmptyObject("Foot(GroundCheck)", playerInstance);
    GameObject sideCollider = AttachEmptyObject("SideCollider", playerInstance);
    GameObject head = AttachEmptyObject("Head", playerInstance);

    playerInstance.transform.localScale = new Vector3(3, 3, 3);

    // remove rotation
    Rigidbody rigidbody = playerInstance.GetComponent<Rigidbody>();
    rigidbody.constraints |= RigidbodyConstraints.FreezeRotation;

    // add root collider
    /*SphereCollider playerSphereCollider = playerInstance.AddComponent<SphereCollider>();
    playerSphereCollider.radius = 0.3f;
    playerSphereCollider.isTrigger = true;*/

    // add player controller
    PlayerControllerVillage playerController = playerInstance.AddComponent<PlayerControllerVillage>();

    // add foot object
    foot.transform.localPosition = new Vector3(0, 0, 0);
    //foot.AddComponent<GroundCheckController>();
    //foot.AddComponent<TrampolineCheckController>();
    BoxCollider foot_boxCollider = foot.AddComponent<BoxCollider>();
    foot_boxCollider.center = new Vector3(0, 0.05f, 0);
    foot_boxCollider.size = new Vector3(0.5f, 0.1f, 0.5f);
    // todo: add material
    // foot_boxCollider.material = 

    // add ground check object
    foot_GroundCheck.transform.localPosition = new Vector3(0, 0, 0);
    foot_GroundCheck.tag = "Player";
    BoxCollider foot_GroundCheck_boxCollider = foot_GroundCheck.AddComponent<BoxCollider>();
    foot_GroundCheck_boxCollider.center = new Vector3(0, -0.01f, 0);
    foot_GroundCheck_boxCollider.isTrigger = true;
    foot_GroundCheck_boxCollider.size = new Vector3(0.5f, 0.02f, 0.5f);
    foot_GroundCheck.AddComponent<GroundCheckControllerVillage>();

    // add side collider
    sideCollider.tag = "Player";
    sideCollider.transform.localPosition = new Vector3(0, 0, 0);
    SphereCollider sphereCollider = sideCollider.AddComponent<SphereCollider>();
    sphereCollider.center = new Vector3(0, 0.35f, 0);
    sphereCollider.radius = 0.35f;
    // todo: add material
    // sphereCollider.material = 

    // add head collider
    head.transform.localPosition = new Vector3(0, 0, 0);
    BoxCollider head_boxCollider = head.AddComponent<BoxCollider>();
    head_boxCollider.center = new Vector3(0, 0.675f, 0);
    head_boxCollider.size = new Vector3(0.5f, 0.1f, 0.5f);

    // set SmoothSync options
    Smooth.SmoothSyncPUN2 smpun2 = playerInstance.GetComponent<Smooth.SmoothSyncPUN2>();
    smpun2.whenToUpdateTransform = Smooth.SmoothSyncPUN2.WhenToUpdateTransform.FixedUpdate;
    smpun2.setVelocityInsteadOfPositionOnNonOwners = false;
    smpun2.extrapolationMode = Smooth.SmoothSyncPUN2.ExtrapolationMode.Unlimited;
  }

  private GameObject AttachEmptyObject(string name, GameObject target)
  {
    GameObject go = new GameObject(name);
    go.transform.parent = target.transform;

    return go;
  }

  [PunRPC]
  void SetPlayerInstanceAttributesRPC(int viewID)
  {
    GameObject[] goList = GameObject.FindGameObjectsWithTag("Player");

    foreach (GameObject go in goList)
    {
      if (go.GetPhotonView()?.ViewID == viewID && !go.GetPhotonView().IsMine)
        SetPlayerInstanceAttributes(go);
    }
  }
}
