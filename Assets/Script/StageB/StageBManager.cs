using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.UI;

public class StageBManager : MonoBehaviourPun, IInstanceManager
{
    public GameObject UI;
  public GameObject playerPrefab;

  public GameObject bonusObjectPrefab;

  public GameObject speedUpObjectPrefab;
  public GameObject speedDownObjectPrefab;
  public GameObject scaleObjectPrefab;
  public GameObject stunObjectPrefab;

  public GameObject MspeedUpObjectPrefab;
  public GameObject MspeedDownObjectPrefab;
  public GameObject MscaleObjectPrefab;
  public GameObject MstunObjectPrefab;

  public GameObject gameFieldObject;

  private Vector3 centerPos;
  private Smooth.SmoothSyncPUN2 smoothSync;

  public float gameFieldDiameter;
  public int gameScore = 0;

  public GameObject Ui;

  private bool isFail=true;
  private int[,] gameLevelDesign = {
    {1,1,0,0,0,0,0,0},
    {1,2,0,0,0,0,0,0},
    {2,3,0,0,0,0,0,0},

    {1,1,1,0,0,0,0,0},
    {1,1,2,0,0,0,0,0},
    {2,1,3,0,0,0,0,0},

    {1,1,1,1,0,0,0,0},
    {1,1,2,2,0,0,0,0},
    {2,1,2,3,0,0,0,0},

    {0,0,1,1,1,1,0,0},
    {0,0,2,2,2,2,0,0},
    {0,0,2,3,3,3,0,0},

    {0,0,0,1,1,1,1,0},
    {0,0,0,2,2,1,2,0},
    {0,0,0,3,2,2,3,0},

    {0,0,0,0,1,1,1,1},
    {0,0,0,0,2,2,2,2},
    {0,0,0,0,2,2,3,3},
  };
  private int stepNow = 0;

  // Start is called before the first frame update
  void Start()
  {
    Ui = GameObject.Find("UI");
    centerPos = gameFieldObject.transform.position;

    if (playerPrefab == null)
    {
      Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
    }
    else
    {
      if (PlayerController.LocalPlayerInstance == null)
      {
        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
        int currentPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        playerPrefab = IInstanceManager.InstantiatePlayerCharacter(PlayerPrefs.GetInt("nowEggNum"), GenerateRandomPositionInCircle(10, 30, -45, -25), Quaternion.identity);
        //string path = "Characters/Egg" + (int)Random.Range(1, 4);
        //playerPrefab = PhotonNetwork.Instantiate(path, GenerateRandomPositionInCircle(10, 30, -45, -25), Quaternion.identity);
        SetPlayerInstanceAttributes(playerPrefab);
        photonView.RPC("SetPlayerInstanceAttributesRPC", RpcTarget.AllViaServer, playerPrefab.GetPhotonView().ViewID);

        // attach smoothsync
        //playerPrefab.AddComponent<Smooth.SmoothSyncPUN2>();
      }
      else
      {
        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
      }
    }

    if(PhotonNetwork.LocalPlayer.IsMasterClient)
    {

      StartCoroutine(CoinGenerator());
      StartCoroutine(GimmickGenerator());
    }
  }
  public void scoreUp()
  {
    gameScore++;
    Ui.transform.GetChild(0).GetComponent<UI_Static>().setCoin(gameScore);
  }
  IEnumerator GimmickGenerator()
  {
    yield return new WaitForSeconds(10f);
    if (gameLevelDesign[stepNow, 0]>0) InstantiateObjects(speedUpObjectPrefab.name, gameLevelDesign[stepNow,0]);
    if (gameLevelDesign[stepNow, 1] > 0) InstantiateObjects(speedDownObjectPrefab.name, gameLevelDesign[stepNow, 1]);
    if (gameLevelDesign[stepNow, 2] > 0) InstantiateObjects(scaleObjectPrefab.name, gameLevelDesign[stepNow, 2]);
    if (gameLevelDesign[stepNow, 3] > 0) InstantiateObjects(stunObjectPrefab.name, gameLevelDesign[stepNow, 3]);

    if (gameLevelDesign[stepNow, 4] > 0) InstantiateObjects(MspeedUpObjectPrefab.name, gameLevelDesign[stepNow, 4]);
    if (gameLevelDesign[stepNow, 5] > 0) InstantiateObjects(MspeedDownObjectPrefab.name, gameLevelDesign[stepNow, 5]);
    if (gameLevelDesign[stepNow, 6] > 0) InstantiateObjects(MscaleObjectPrefab.name, gameLevelDesign[stepNow, 6]);
    if (gameLevelDesign[stepNow, 7] > 0) InstantiateObjects(MstunObjectPrefab.name, gameLevelDesign[stepNow, 7]);

    if (stepNow++ < 15)
    {
      StartCoroutine(GimmickGenerator());
    }
    else
    {
      isFail = false;
      StartCoroutine(RankGimmickGenerator(4.5f));
    }
  }
  IEnumerator RankGimmickGenerator(float time)
  {
    yield return new WaitForSeconds(time);
    InstantiateObjects(MspeedUpObjectPrefab.name, 3);
    InstantiateObjects(MspeedDownObjectPrefab.name, 3);
    InstantiateObjects(MscaleObjectPrefab.name, 3);
    InstantiateObjects(MstunObjectPrefab.name, 3);
    StartCoroutine(RankGimmickGenerator(time - Time.deltaTime));
  }

  IEnumerator CoinGenerator()
  {
    yield return new WaitForSeconds(5f); 
    InstantiateObjects(bonusObjectPrefab.name, 3);
    StartCoroutine(CoinGenerator());
  }



  void InstantiateObjects(string name, int count)
  {
    for (int i = 0; i < count; ++i)
    {
      Vector3 pos = GenerateRandomPositionInCircle(10, 30, -45, -25);
      IInstanceManager.Instantiate(IInstanceManager.StagePath.StageB, name, pos, Quaternion.identity);
    }
  }

  [PunRPC]
  public void UiControllRPC(bool isFail)
  {
    Debug.Log("fail RPC called");
    Ui.transform.GetChild(isFail?1:2).gameObject.SetActive(true);

    if (!isFail)
    {
        UI_Clear uic = UI.transform.GetChild(2).GetComponent<UI_Clear>();
        uic.getEgg('B');
        uic.setUi_EggGet();
        int playTime = UI.transform.GetChild(0).GetComponent<UI_Static>().m * 60 + UI.transform.GetChild(0).GetComponent<UI_Static>().s;
        if (PhotonNetwork.IsMasterClient)
            uic.RecordUpdateForMastClient(PlayerPrefs.GetString("team_guid"), 'B', playTime);

        uic.RecordUpdate(PlayerPrefs.GetString("team_guid"), PlayerPrefs.GetString("userName"), gameScore, 'D', playTime);
    }

  }

  public void UiControll()
  {
    Debug.Log("master client get");
    
    photonView.RPC("UiControllRPC", RpcTarget.AllViaServer, isFail);
  }


  Vector3 GenerateRandomPositionInCircle(int minX, int maxX, int minZ, int maxZ)
  {
    Vector3 generatedPos = new Vector3(0, gameFieldObject.transform.position.y, 0);

    int i = 0;

    while (i++<50)
    {
      float x = (float)Random.Range(minX, maxX);
      float z = (float)Random.Range(minZ, maxZ);

      generatedPos.x = x;
      generatedPos.z = z;

      if (Vector3.Distance(centerPos, generatedPos) < 15)
        break;
    }

    generatedPos.y = 20;

    return generatedPos;
  }

  private void SetPlayerInstanceAttributes(GameObject playerPrefab)
  {
    playerPrefab.AddComponent<StageB_PlayerController>();

    //set animator and scale
    playerPrefab.GetComponent<Animator>().enabled = false;
    playerPrefab.GetComponent<Transform>().localScale += new Vector3(2, 2, 2);

    // attach rigidbody
    Rigidbody rigidbody = playerPrefab.GetComponent<Rigidbody>();
    rigidbody.mass = 1;
    rigidbody.drag = 0;
    rigidbody.angularDrag = 0.05f;
    rigidbody.useGravity = true;

    // attach collider
    SphereCollider collider = playerPrefab.AddComponent<SphereCollider>();
    collider.center = new Vector3(0, 0.3f, 0);
    collider.radius = 0.3f;
  }

  [PunRPC]
  void SetPlayerInstanceAttributesRPC(int viewID)
  {
    GameObject[] goList = GameObject.FindGameObjectsWithTag("Player");

    foreach(GameObject go in goList)
    {
      if (go.GetPhotonView().ViewID == viewID && !go.GetPhotonView().IsMine)
        SetPlayerInstanceAttributes(go);
    }
  }
}
