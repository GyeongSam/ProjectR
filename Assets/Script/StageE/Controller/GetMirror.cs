using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GetMirror : MonoBehaviourPun
{
    [SerializeField]
    Define.PlayerColor _color;

    bool _isCalled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.transform.parent.gameObject;

            Transform mirror = transform.parent;

            if (player?.GetComponent<PlayerControllerE>().PlayerColor == _color && 
            PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().PlayerColor == _color)
            {
                if (!_isCalled)
                {
                    photonView.RPC("GetMirrorRPC", RpcTarget.AllViaServer, (int)_color);
                    _isCalled = true;
                }
            }
        }
    }

    [PunRPC]
    void GetMirrorRPC(int color)
    {
        Transform mirror = null;
        foreach (GameObject go in Define.Mirror) 
        {
            if (go.transform.parent.childCount == 2 && go.transform.parent.GetChild(1).GetComponent<GetMirror>()._color == (Define.PlayerColor)color)
            {
                mirror = go.transform.parent;
            }
        }

        Debug.Log(mirror.name);

        GameObject player = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerControllerE>()?.PlayerColor == (Define.PlayerColor)color)
                player = p;
        }

        Debug.Log(player.name);

        if (mirror != null && player != null)
        {
            mirror.parent.GetComponent<LastRoomStartCheck>().MirrorCount += 1;
            mirror.position = player.transform.position + player.transform.up / 2;
            mirror.position += player.transform.forward / 3;
            mirror.parent = player.transform;
            mirror.rotation = Quaternion.LookRotation(player.transform.forward);

            mirror.GetChild(0).GetComponent<BoxCollider>().enabled = true;
            mirror.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject);
        }
    }
}
