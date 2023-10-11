using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static MyNetworkManager;
using static WebManager;

public class PartyRPCHandler : MonoBehaviourPunCallbacks
{
    private static PartyRPCHandler _instance;
    private static PhotonView _photonView;
    private static PartyManager.PartyEntity _currentEntity;

    object _lock;
    void Start()
    {
        if (_instance == null)
        {
            //_instance = new PartyRPCHandler();
            _instance = this;
        }

        if (_photonView == null)
        {
            _photonView = photonView;
        }
    }

    public static PartyRPCHandler Instance()
    {
        return _instance;
    }

    public void SetCurrentEntity(PartyManager.PartyEntity entity)
    {
        _currentEntity = entity;
    }

    public void JoinOrCreateRoom(PartyManager.PartyEntity entity)
    {
        object _entity = entity;
        _photonView.RPC("JoinOrCreateRoomRPC", RpcTarget.AllViaServer, entity.guid);
    }

    [PunRPC]
    void JoinOrCreateRoomRPC(string guid)
    {
        if (_currentEntity.guid == guid)
        {
            MyNetworkManager.Instance().LeaveRoom();

            StartCoroutine(LobbyWaiter());
        }
    }

    public void JoinVillage()
    {
        MyNetworkManager.Instance().LeaveRoom();

        //StartCoroutine(VillageWaiter());
    }

    IEnumerator LobbyWaiter()
    {
        while (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
            MyNetworkManager.Instance().JoinLobby();

            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator VillageWaiter()
    {
        while (!PhotonNetwork.InLobby) { yield return new WaitForSeconds(0.1f); }
        MyNetworkManager.Instance().JoinRoom("Village");
    }

    public override void OnLeftRoom()
    {
        MyNetworkManager.Instance().JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        if (_currentEntity != null)
            MyNetworkManager.Instance().JoinRoom(_currentEntity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.Name.Equals("Village"))
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 6 && PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            AddTeam(_currentEntity.title, _currentEntity.guid, _currentEntity.users);
            PlayerPrefs.SetString("team_guid", _currentEntity.guid);

            switch ((int)PhotonNetwork.CurrentRoom.CustomProperties["StageType"])
            {
            case (int)StageType.StageB:
                PhotonNetwork.LoadLevel(1);
                break;
            case (int)StageType.StageC:
                PhotonNetwork.LoadLevel(4);
                break;
            case (int)StageType.StageD:
                PhotonNetwork.LoadLevel(2);
                break;
            case (int)StageType.StageE:
                PhotonNetwork.LoadLevel(3);
                break;
            }
        }
    }

    public void AddTeam(string teamName, string teamId, List<string> memberIdList)
    {
        WebManager.TeamInfo teamInfo = new TeamInfo();
        teamInfo.teamId = teamId;
        teamInfo.teamName = teamName;
        teamInfo.memberIdList = memberIdList;

        StartCoroutine(WebManager.Instance().WebRequest("team/add-team", "POST", teamInfo));
    }
}
