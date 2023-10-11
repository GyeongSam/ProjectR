using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StageCManager : IStageManager, IInstanceManager
{
  public GameObject playerPrefab;
  public GameObject[] spawner;
  private bool isTetrisPlayer { get; set; } = false;
  private bool isInsidePlayer { get; set; } = false;
  public int currentControllerNo;
  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("Current Player is " + PhotonNetwork.LocalPlayer.ActorNumber);

    if (PhotonNetwork.IsMasterClient)
    {
      int[] generatedRole = SelectRandomUser(PhotonNetwork.CurrentRoom.PlayerCount, 2);
      Debug.Log("generatedrole " + System.String.Join(", ", System.Array.ConvertAll(generatedRole, x => x.ToString())));

      for(int i=1; i<= generatedRole.Length; ++i)
      {
        if (generatedRole[i-1] == 0)
          photonView.RPC("SetInsidePlayer", RpcTarget.AllViaServer, i);
        else
          photonView.RPC("SetTetrisPlayer", RpcTarget.AllViaServer, i, generatedRole[i-1]);
      }
    }
  }

  int[] SelectRandomUser(int userCount, int targetCount)
  {
    int currentCount = 1;
    int[] res = new int[userCount];

    System.Random rand = new System.Random();

    for (; ; )
    {
      if (currentCount > targetCount)
        break;

      int no = rand.Next(1, userCount);
      if (res[no] == 0)
        res[no] = currentCount++;
    }

    return res;
  }

  [PunRPC]
  void SetTetrisPlayer(int actorNo, int controllerNo)
  {
    if (PhotonNetwork.LocalPlayer.ActorNumber == actorNo)
    {
      isTetrisPlayer = true;
      GameObject targetSpawner = spawner[controllerNo - 1];
      targetSpawner.GetComponent<SpawnerTetris>().InitalizeSpawner();
    }
  }

  [PunRPC]
  void SetInsidePlayer(int actorNo)
  {
    if (PhotonNetwork.LocalPlayer.ActorNumber == actorNo)
    {
      isInsidePlayer = true;
      Vector3 playerPos = new Vector3(Random.Range(3f, 42f), -1f, -1f);
      IInstanceManager.Instantiate(IInstanceManager.StagePath.StageC, this.playerPrefab.name, playerPos, Quaternion.identity);
    }
    
  }
}
