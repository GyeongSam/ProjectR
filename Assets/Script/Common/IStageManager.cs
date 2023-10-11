using System.Collections;
using UnityEngine;
using Photon.Pun;

public abstract class IStageManager : MonoBehaviourPunCallbacks
{  
  private void StageClearedWorker()
  {
    // todo: Scene change
  }

  private void StageFailedWorker()
  {
    // todo: Stage reset
  }

  public void StageCleared()
  {
    photonView.RPC("StageCLearedRPC", RpcTarget.AllViaServer);
  }

  public void StageFailed()
  {
    photonView.RPC("StageFailedRPC", RpcTarget.AllViaServer);
  }

  [PunRPC]
  void StageClearedRPC()
  {
    StageClearedWorker();
  }

  [PunRPC]
  void StageFailedRPC()
  {
    StageFailedWorker();
  }
}