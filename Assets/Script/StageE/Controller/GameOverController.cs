using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameOverController : MonoBehaviourPun
{
    bool isReceived;

    [SerializeField]
    GameObject GameFailUI;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Camera.main.transform.position = new Vector3(-180, 180, 180);
            Camera.main.transform.rotation = Quaternion.Euler(-90, 0, 0);
            photonView.RPC("GameOver", RpcTarget.AllBufferedViaServer, null);
        }
    }

    [PunRPC]
    void GameOver()
    {
        if (!isReceived)
        {
            Managers.Input.IsActive = false;
            GameFailUI.SetActive(true);
            isReceived = true;
        }
    }
}
