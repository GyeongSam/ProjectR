using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class StageEManager : IStageManager, IInstanceManager
{
    public GameObject UI;
  public GameObject initialSpwanPoint;

  public PhysicMaterial[] physicsMaterial;
  public GameObject playerInstance;
  public GameObject colorCirclePrefab;
  public Material[] materials;

  // Start is called before the first frame update
  void Start()
  {
    if (PlayerControllerE.LocalPlayerInstance == null)
    {
      Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
      // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

      Debug.Log("Before : " + CameraController._player);
      //CameraController._player = playerInstance = IInstanceManager.InstantiatePlayerCharacter((int)Random.Range(1, 100), new Vector3(-104, 9, -86), Quaternion.identity);
      PlayerControllerE.LocalPlayerInstance = CameraController._player = playerInstance = IInstanceManager.InstantiatePlayerCharacter(PlayerPrefs.GetInt("nowEggNum"), initialSpwanPoint.transform.position + new Vector3(0,0, -1.6f * PhotonNetwork.LocalPlayer.ActorNumber), Quaternion.identity);   

      SetPlayerInstanceAttributes(playerInstance, PhotonNetwork.LocalPlayer.ActorNumber-1);

      photonView.RPC("SetPlayerInstanceAttributesRPC", RpcTarget.AllViaServer, playerInstance.GetPhotonView().ViewID, PhotonNetwork.LocalPlayer.ActorNumber - 1);

      Camera.main.GetComponent<CameraController>().CamSet();
      Debug.Log("After : " + CameraController._player);
    }
    else
    {
      Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
    }
    Define.doors = GameObject.FindGameObjectWithTag("Door");
    Define.Mirror = GameObject.FindGameObjectsWithTag("Mirror");

    /*if (PhotonNetwork.IsMasterClient)
    {
      IInstanceManager.Instantiate(IInstanceManager.StagePath.StageE, "Key(green)", new Vector3(2, -4, -8), Quaternion.identity);
    }*/
  }

  private void SetPlayerInstanceAttributes(GameObject playerInstance, int materialIdx)
  {
    GameObject foot = AttachEmptyObject("Foot", playerInstance);
    GameObject foot_GroundCheck = AttachEmptyObject("Foot(GroundCheck)", playerInstance);
    GameObject sideCollider = AttachEmptyObject("SideCollider", playerInstance);
    GameObject head = AttachEmptyObject("Head", playerInstance);
    GameObject colorCircle = Instantiate(colorCirclePrefab);

    // remove rotation
    Rigidbody rigidbody = playerInstance.GetComponent<Rigidbody>();
    rigidbody.constraints |= RigidbodyConstraints.FreezeRotation;

    // add root collider
    /*SphereCollider playerSphereCollider = playerInstance.AddComponent<SphereCollider>();
    playerSphereCollider.radius = 0.3f;
    playerSphereCollider.isTrigger = true;*/

    // add player controller
    PlayerControllerE playerController = playerInstance.AddComponent<PlayerControllerE>();
    playerController.materials = materials;
    playerController.PlayerColor = (Define.PlayerColor)materialIdx;

    // add foot object
    foot.transform.localPosition = new Vector3(0, 0, 0);
    //foot.AddComponent<GroundCheckController>();
    //foot.AddComponent<TrampolineCheckController>();
    BoxCollider foot_boxCollider = foot.AddComponent<BoxCollider>();
    foot_boxCollider.center = new Vector3(0, 0.05f, 0);
    foot_boxCollider.size = new Vector3(0.5f, 0.1f, 0.5f);
    foot_boxCollider.material = physicsMaterial[0];
    // todo: add material
    // foot_boxCollider.material = 

    // add ground check object
    foot_GroundCheck.transform.localPosition = new Vector3(0, 0, 0);
    BoxCollider foot_GroundCheck_boxCollider = foot_GroundCheck.AddComponent<BoxCollider>();
    foot_GroundCheck_boxCollider.center = new Vector3(0, -0.01f, 0);
    foot_GroundCheck_boxCollider.isTrigger = true;
    foot_GroundCheck_boxCollider.size = new Vector3(0.5f, 0.02f, 0.5f);
    foot_GroundCheck.AddComponent<GroundCheckController>();

    // add side collider
    sideCollider.layer = LayerMask.NameToLayer("Player");
    sideCollider.transform.localPosition = new Vector3(0, 0, 0);
    SphereCollider sphereCollider = sideCollider.AddComponent<SphereCollider>();
    sphereCollider.material = physicsMaterial[1];
    sphereCollider.center = new Vector3(0, 0.36f, 0);
    sphereCollider.radius = 0.35f;
    sphereCollider.tag = "Player";
    // todo: add material
    // sphereCollider.material = 

    // add head collider
    head.transform.localPosition = new Vector3(0, 0, 0);
    BoxCollider head_boxCollider = head.AddComponent<BoxCollider>();
    head_boxCollider.center = new Vector3(0, 0.675f, 0);
    head_boxCollider.size = new Vector3(0.5f, 0.1f, 0.5f);

    // add color circle
    colorCircle.transform.parent = playerInstance.transform;
    colorCircle.transform.localPosition = new Vector3(0, 0.025f, 0);
    colorCircle.transform.rotation = Quaternion.Euler(-90, 0, 0);
    colorCircle.transform.localScale = new Vector3(25, 25, 25);
    
    MeshRenderer mr = colorCircle.GetComponent<MeshRenderer>();
    mr.materials = new Material[1] { materials[materialIdx] };

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
  void SetPlayerInstanceAttributesRPC(int viewID, int actorNo)
  {
    GameObject[] goList = GameObject.FindGameObjectsWithTag("Player");

    foreach (GameObject go in goList)
    {
        if (go.GetPhotonView()?.ViewID == viewID && !go.GetPhotonView().IsMine)
        {
            SetPlayerInstanceAttributes(go, actorNo);
        }
    }
  }

    
    public void UpdateRanking()
    {
        UI_Clear uic = UI.transform.GetChild(2).GetComponent<UI_Clear>();
        uic.getEgg('E');
        uic.setUi_EggGet();
        int playTime = UI.transform.GetChild(0).GetComponent<UI_Static>().m * 60 + UI.transform.GetChild(0).GetComponent<UI_Static>().s;
        if (PhotonNetwork.IsMasterClient)
            uic.RecordUpdateForMastClient(PlayerPrefs.GetString("team_guid"), 'E', playTime);
    }
}
