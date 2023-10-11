using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class StageDManager : IStageManager, IInstanceManager
{
  public enum MarkType
  {
    Destroy,
    Upgrade
  };

  //public GameObject stageManager;
  //public GameObject bombPrefab;
  //public GameObject giantBombPrefab;
  //public GameObject upgradeItemPrefab;
  //public GameObject bonusItemPrefab;
  //public GameObject playerInstance;
  public GameObject UI;
  public GameObject playerInstance;

  public GameObject bomb;
  public GameObject giantBomb;
  public GameObject upgradeItem;
  public Transform field;

  public int gameScore = 0;
  //public int upgradeItemCount = 3;
  //public int bonusItemCount = 3;
  //public int remainGiantBombCount = 5;

  public int bombPos = -1;
  public int giantBombPos = -1;

  public delegate void MarkFloorWithGOCallback(GameObject targetGo, MarkType markType);
  public delegate void MarkFloorCallback(Vector3 position, MarkType markType);

  public float[,] pos = { { -5, 3, 5, }, { 0, 3, 5 }, { 5, 3, 5 }, { -5, 3, 0 }, { 0, 3, 0, }, { 5, 3, 0 }, { -5, 3, -5 }, { 0, 3, -5 }, { 5, 3, -5 } };
  //bool[] isFloorDestroyed = new bool[9];
  //bool[] isFloorUpgraded = new bool[9];


  private bool isFail = true;
  System.Random random = new System.Random();

  // Start is called before the first frame update
  void Start()
  {
    //bomb.transform.SetParent(field.GetChild(Random.Range(0, 8)).transform, false);
    //giantBomb.transform.position = field.GetChild(Random.Range(0, 8)).transform.position + new Vector3(0,3,0);

    //upgradeItem.transform.SetParent(field.GetChild(Random.Range(0, 8)).transform, false);


    if (PlayerController.LocalPlayerInstance == null)
    {
      Debug.Log($"Actor Number {PhotonNetwork.LocalPlayer.ActorNumber}");
      int currentPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
      // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
      playerInstance = IInstanceManager.InstantiatePlayerCharacter(PlayerPrefs.GetInt("nowEggNum"), new Vector3(pos[currentPlayerId, 0], pos[currentPlayerId, 1], pos[currentPlayerId, 2]), Quaternion.identity);
      photonView.RPC("SetPlayerInstanceAttributesRPC", RpcTarget.AllViaServer, playerInstance.GetPhotonView().ViewID);

      SetPlayerInstanceAttributes(playerInstance);
    }
    else
    {
      Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
    }


    if (PhotonNetwork.IsMasterClient)
    {
      StartCoroutine(BombCoroutine());
      StartCoroutine(GimmickCoroutine());
      StartCoroutine(CoinCoroutine());
    }
  }
  public void scoreUp()
  {
    gameScore++;
    UI.transform.GetChild(0).GetComponent<UI_Static>().setCoin(gameScore);
  }
  public void bombExp(bool isGiant)
  {
    if (!PhotonNetwork.IsMasterClient) return;
    if (isGiant)
    {
      giantBombPos = -1;
    }
    else bombPos = -1;
  }
  //IEnumerator bombCoroutine()
  //{
  //  for (int i = 0; i < 100; i++)
  //  {
  //    yield return new WaitForSeconds(8f);

  //    int idx;
  //    do
  //    {
  //      idx = random.Next(0, pos.GetLength(0));
  //    } while (isFloorDestroyed[idx]);

  //    Debug.Log($"Undestroyed {idx} found!");

  //    Vector3 newBombPosition = new Vector3(pos[idx, 0], pos[idx, 1], pos[idx, 2]);
  //    float timeToExplode = Random.Range(0, 3.0f);
  //    Debug.Log("RPC created with time " + timeToExplode);
  //    bool isGiantBomb = remainGiantBombCount > 0 && Random.Range(1, 1) > 0.5;

  //    if (isGiantBomb)
  //      remainGiantBombCount--;

  //    photonView.RPC("CreateBombRPC", RpcTarget.AllViaServer, newBombPosition, timeToExplode, isGiantBomb);
  //  }
  //}

  //IEnumerator ItemsCoroutine(int count, string prefabName)
  //{
  //  for (int i = 0; i < count; i++)
  //  {
  //    yield return new WaitForSeconds(20f);

  //    int idx;
  //    do
  //    {
  //      idx = random.Next(0, pos.GetLength(0));
  //    } while (isFloorDestroyed[idx]);

  //    Vector3 itemPos = new Vector3(pos[idx, 0], pos[idx, 1]-3.0f, pos[idx, 2]);

  //    GameObject go = IInstanceManager.Instantiate(IInstanceManager.StagePath.StageD, prefabName, itemPos, Quaternion.identity);
  //    go.SetActive(true);
  //  }
  //}

  Vector3 getRandomPos()
  {
    return new Vector3(Random.Range(-1.3f, 1.3f), 0, Random.Range(-1.3f, 1.3f));
  }

  IEnumerator BombCoroutine()
  {
    while (true)
    {
      yield return new WaitForSeconds(8f);


      int temp = Random.Range(0, 9);
      while (temp == giantBombPos || !field.GetChild(temp).gameObject.activeSelf)
      {
        temp = Random.Range(0, 9);
      }
      photonView.RPC("bombRPC", RpcTarget.AllViaServer, field.GetChild(temp).transform.position + new Vector3(0, 3, 0), Random.Range(2f, 4f));
      bombPos = temp;

    }
  }

  IEnumerator GimmickCoroutine()
  {
    yield return new WaitForSeconds(20f);
    int step = 0;
    while (step++ < 6)
    {
      yield return new WaitForSeconds(15f - (float)step);

      int temp = Random.Range(0, 9);
      while (!field.GetChild(temp).gameObject.activeSelf)
      {
        temp = Random.Range(0, 9);
      }
      photonView.RPC("upgradeItemRPC", RpcTarget.AllViaServer, field.GetChild(temp).transform.position + getRandomPos());

      yield return new WaitForSeconds(5f);

      temp = Random.Range(0, 9);
      while (temp == bombPos || !field.GetChild(temp).gameObject.activeSelf)
      {
        temp = Random.Range(0, 9);
      }

      photonView.RPC("giantBombRPC", RpcTarget.AllViaServer, field.GetChild(temp).transform.position + new Vector3(0, 3, 0));
      giantBombPos = temp;

    }

    isFail = false;

    float gimmickTime1 = 10f;
    float gimmickTime2 = 5f;
    while (true)
    {
      yield return new WaitForSeconds(gimmickTime1);
      int temp = Random.Range(0, 9);
      while (!field.GetChild(temp).gameObject.activeSelf)
      {
        temp = Random.Range(0, 9);
      }
      photonView.RPC("upgradeItemRPC", RpcTarget.AllViaServer, field.GetChild(temp).transform.position + getRandomPos());

      yield return new WaitForSeconds(gimmickTime2);
      temp = Random.Range(0, 9);
      while (temp == bombPos || !field.GetChild(temp).gameObject.activeSelf)
      {
        temp = Random.Range(0, 9);
      }

      photonView.RPC("giantBombRPC", RpcTarget.AllViaServer, field.GetChild(temp).transform.position + new Vector3(0, 3, 0));
      giantBombPos = temp;
      if (gimmickTime2 > 3f) gimmickTime2 -= Time.deltaTime;
    }

  }


  IEnumerator CoinCoroutine()
  {
    while (true)
    {
      yield return new WaitForSeconds(7f);
      Vector3[] temp = { getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos(), getRandomPos() };
      photonView.RPC("coinRPC", RpcTarget.AllViaServer, temp);
    }
  }
  [PunRPC]
  public void bombRPC(Vector3 pos, float time)
  {
    bomb.SetActive(true);
    bomb.GetComponent<Rigidbody>().velocity = Vector3.zero;
    bomb.transform.position = pos;
    bomb.GetComponent<BombController>().IgniteGranade(time);
  }

  [PunRPC]
  public void upgradeItemRPC(Vector3 pos)
  {
    upgradeItem.SetActive(true);
    upgradeItem.transform.position = pos;
  }
  [PunRPC]
  public void giantBombRPC(Vector3 pos)
  {
    giantBomb.SetActive(true);
    giantBomb.transform.position = pos;
    giantBomb.GetComponent<BombController>().IgniteGranade(5f);
  }

  [PunRPC]
  public void coinRPC(Vector3[] pos)
  {
    for (int i = 0; i < 9; ++i)
    {
      field.GetChild(i).transform.GetChild(2).gameObject.SetActive(true);
      field.GetChild(i).transform.GetChild(2).transform.localPosition = pos[i] + new Vector3(0, 0.5f, 0);
    }
  }



  [PunRPC]
  public void uiControllRPC(bool isFail)
  {
    UI.transform.GetChild(isFail ? 1 : 2).gameObject.SetActive(true);
    if (!isFail) 
    {
        UI_Clear uic = UI.transform.GetChild(2).GetComponent<UI_Clear>();
        uic.getEgg('D');
        uic.setUi_EggGet();
        int playTime = UI.transform.GetChild(0).GetComponent<UI_Static>().m * 60 + UI.transform.GetChild(0).GetComponent<UI_Static>().s;
        if (PhotonNetwork.IsMasterClient)
            uic.RecordUpdateForMastClient(PlayerPrefs.GetString("team_guid"), 'D', playTime);

        uic.RecordUpdate(PlayerPrefs.GetString("team_guid"), PlayerPrefs.GetString("userName"), gameScore, 'D', playTime);
    }
  }

  [PunRPC]
  public void isSucessRPC()
  {
    photonView.RPC("uiControllRPC", RpcTarget.AllViaServer, isFail);
  }

  public void uiControll()
  {
    photonView.RPC("isSucessRPC", RpcTarget.MasterClient);
  }


  public override void OnLeftRoom()
  {
    base.OnLeftRoom();
    SceneManager.LoadScene(0);
  }

  public void LeaveRoom()
  {
    PhotonNetwork.LeaveRoom();
  }

  //public void MarkFloor(Vector3 position, MarkType type)
  //{
  //  for (int i = 0; i < pos.GetLength(0); ++i)
  //  {
  //    if (position.x == pos[i, 0] && position.z == pos[i, 2])
  //    {
  //      if (type == MarkType.Destroy)
  //        isFloorDestroyed[i] = true;
  //      else
  //        isFloorUpgraded[i] = true;

  //      break;
  //    }
  //  }
  //}

  //public void MarkFloorWithGo(GameObject targetGo, MarkType type)
  //{
  //  for (int i = 0; i < pos.GetLength(0); ++i)
  //  {
  //    if (targetGo.transform.position.x == pos[i, 0] && targetGo.transform.position.z == pos[i, 2])
  //    {
  //      if (type == MarkType.Destroy && !isFloorUpgraded[i])
  //      {
  //        isFloorDestroyed[i] = true;
  //        targetGo.SetActive(false);
  //      }
  //      else if(type == MarkType.Upgrade)
  //        isFloorUpgraded[i] = true;

  //      break;
  //    }
  //  }
  //}

  private void SetPlayerInstanceAttributes(GameObject playerInstance)
  {
    Smooth.SmoothSyncPUN2 smpun2 = playerInstance.GetComponent<Smooth.SmoothSyncPUN2>();
    smpun2.whenToUpdateTransform = Smooth.SmoothSyncPUN2.WhenToUpdateTransform.FixedUpdate;

    playerInstance.AddComponent<PlayerController>();
    //playerInstance.GetComponent<PlayerController>().markCallback = MarkFloor;

    Rigidbody rigidbody = playerInstance.GetComponent<Rigidbody>();
    rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    // attach ConstantForce
    ConstantForce constantForce = playerInstance.AddComponent<ConstantForce>();
    constantForce.force = new Vector3(0, -50, 0);

    // attach SphereCollider
    SphereCollider collider = playerInstance.AddComponent<SphereCollider>();
    collider.center = new Vector3(0, 0.27f, 0);
    collider.radius = 0.3f;

    // create empty object and attach to player
    GameObject colliderWrapper = new GameObject("ColliderWrapper");
    colliderWrapper.transform.parent = playerInstance.transform;
    colliderWrapper.transform.localPosition = new Vector3(0, 0, 0);

    // attach ground detect script
    GroundDetectScript gds = colliderWrapper.AddComponent<GroundDetectScript>();
    gds.Player = playerInstance;

    // attach BoxCollider
    BoxCollider boxCollider = colliderWrapper.AddComponent<BoxCollider>();
    boxCollider.size = new Vector3(.5f, .1f, .5f);
    boxCollider.center = new Vector3(0, 0, 0);
    boxCollider.isTrigger = true;
  }

  [PunRPC]
  void SetPlayerInstanceAttributesRPC(int viewID)
  {
    GameObject[] goList = GameObject.FindGameObjectsWithTag("Player");

    foreach (GameObject go in goList)
    {
      if (go.GetPhotonView().ViewID == viewID && !go.GetPhotonView().IsMine)
        SetPlayerInstanceAttributes(go);
    }
  }

  //[PunRPC]
  //void CreateBombRPC(Vector3 bombPosition, float timeToExplode, bool isGiantBomb)
  //{
  //  Debug.Log("RPC called");
  //  GameObject go =
  //    isGiantBomb? 
  //      GameObject.Instantiate(this.giantBombPrefab, bombPosition, Quaternion.identity)
  //    : GameObject.Instantiate(this.bombPrefab, bombPosition, Quaternion.identity);

  //  go.GetComponent<BombController>().markCallback = MarkFloorWithGo;
  //  go.GetComponent<BombController>().isGiantBomb = isGiantBomb;
  //  go.GetComponent<BombController>().IgniteGranade(timeToExplode);
  //}
}
