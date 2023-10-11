using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameEndController : MonoBehaviourPun
{
    bool isReceived;

    [SerializeField]
    GameObject GameClearUI;

    public GameObject GameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            photonView.RPC("GameEnd", RpcTarget.AllBufferedViaServer, null);
        }
    }

    [PunRPC]
    void GameEnd()
    {
        if (!isReceived)
        {
            Managers.Input.IsActive = false;
            GameClearUI.SetActive(true);
            isReceived = true;
            GameManager.GetComponent<StageEManager>().UpdateRanking();
        }
    }
}
