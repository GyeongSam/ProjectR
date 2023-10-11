using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class MyNetworkManager : MonoBehaviourPunCallbacks
{
  public enum StageType
  {
    StageB,
    StageC,
    StageD,
    StageE,
    Village
  }

  string gameVersion = "1";
  string roomName;
  string nickname = "";

  private static MyNetworkManager _instance;
  private static object _lock = new object();
  public static MyNetworkManager Instance()
  {
    if (_instance == null)
    {
      lock (_lock)
      {
        if (_instance == null) 
          _instance = new MyNetworkManager();
      }
    }

    return _instance;
  }

  private void Awake()
  {
  }

  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("start");
    Connect();
    _instance = this;
  }

  private RoomOptions CreateRoomOptions(byte maxPlayers = 6)
  {
    return new RoomOptions
    {
      CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(),
      MaxPlayers = maxPlayers
    };
  }

  public void Connect()
  {
    if (PhotonNetwork.IsConnected)
    {
      Debug.Log("Already Connected");
    }
    else
    {
      PhotonNetwork.GameVersion = gameVersion;
      PhotonNetwork.ConnectUsingSettings();
      PhotonNetwork.AutomaticallySyncScene = false;
    }
  }

  public void Disconnect()
  {
    PhotonNetwork.Disconnect();
  }

  public void CreateRoom(StageType stageType)
  {
    roomName = System.Guid.NewGuid().ToString();
    CreateRoom(roomName, stageType);
  }

  public void CreateRoom(PartyManager.PartyEntity entity)
  {
    RoomOptions roomOptions = CreateRoomOptions();

    roomOptions.CustomRoomProperties.Add("Title", entity.title);
    roomOptions.CustomRoomProperties.Add("StageType", entity.type);
    roomOptions.CustomRoomProperties.Add("IsRandomRoom", false);

    roomName = entity.guid;
    PhotonNetwork.CreateRoom(roomName, roomOptions);
  }

  public void CreateRoom(string title, StageType stageType)
  {
    RoomOptions roomOptions = CreateRoomOptions();

    roomOptions.CustomRoomProperties.Add("Title", title);
    roomOptions.CustomRoomProperties.Add("StageType", stageType);
    roomOptions.CustomRoomProperties.Add("IsRandomRoom", false);

    roomName = System.Guid.NewGuid().ToString();
    PhotonNetwork.CreateRoom(roomName, roomOptions);
  }

  public void JoinRoom()
  {
    string roomName = EventSystem.current.currentSelectedGameObject.name;
    Debug.Log(roomName + " has been clicked!");

    JoinRoom(roomName);
  }

  public void JoinRoom(string roomName, RoomOptions roomOptions = null)
  {
    Debug.Log("5");


    if (PhotonNetwork.InRoom)
      PhotonNetwork.LeaveRoom();

    /* temporary */
    if (roomOptions == null)
    {
      roomOptions = CreateRoomOptions();

      StageType stageType;
      if (roomName.Equals("Stage B"))
        stageType = StageType.StageB;
      else if (roomName.Equals("Stage C"))
        stageType = StageType.StageC;
      else if (roomName.Equals("Stage D"))
        stageType = StageType.StageD;
      else if (roomName.Equals("Stage E"))
        stageType = StageType.StageE;
      else
      {
        roomOptions.MaxPlayers = 0;
        stageType = StageType.Village;
      }


      roomOptions.CustomRoomProperties.Add("StageType", stageType);
      roomOptions.CustomRoomProperties.Add("IsRandomRoom", false);
    }


    PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
  }

  public void JoinRoom(PartyManager.PartyEntity entity)
  {
    RoomOptions roomOptions = CreateRoomOptions();
    roomOptions.CustomRoomProperties.Add("StageType", entity.type);
    roomOptions.CustomRoomProperties.Add("IsRandomRoom", false);
    roomOptions.CustomRoomProperties.Add("Title", entity.title);

    JoinRoom(entity.guid, roomOptions);
  }

  public void JoinExistingRoom(string guid)
  {
    PhotonNetwork.JoinRoom(guid);
  }

  public void JoinExistingRoom(PartyManager.PartyEntity entity)
  {
    JoinExistingRoom(entity.guid);
  }

  public bool JoinRandomRoom(StageType stageType)
  {
    ExitGames.Client.Photon.Hashtable customOptionsTable = new ExitGames.Client.Photon.Hashtable();
    customOptionsTable.Add("StageType", stageType);
    customOptionsTable.Add("IsRandomRoom", true);

    return PhotonNetwork.JoinRandomOrCreateRoom(customOptionsTable, 6);
  }

  public void SetLocalPlayerNickname(string nickName)
  {
    PhotonNetwork.LocalPlayer.NickName = nickName;
  }

  public void LeaveRoom()
  {

    Debug.Log("3");


    if (PhotonNetwork.CurrentRoom.Name.Equals("Village"))
    {
      PhotonNetwork.AutomaticallySyncScene = true;
      Debug.LogError("Leave Room");
      while (PhotonNetwork.InRoom)
      {
        PhotonNetwork.LeaveRoom();
      }
      Debug.LogError("Leave Room Comp");
      PhotonNetwork.JoinLobby();
    }
      
    else
    {
      PhotonNetwork.AutomaticallySyncScene = false;
      if (GameObject.Find("UI").transform.GetChild(1).gameObject.activeSelf) {
        GameObject.Find("UI").transform.GetChild(1).gameObject.GetComponent<UI_Fail>().villageWait();
        while (PhotonNetwork.InRoom)
        {
          PhotonNetwork.LeaveRoom();
        }
        GameObject.Find("UI").transform.GetChild(1).GetComponent<UI_Fail>().lobbyWait();
      } 
      else if (GameObject.Find("UI").transform.GetChild(2).gameObject.activeSelf)
      {
        GameObject.Find("UI").transform.GetChild(2).gameObject.GetComponent<UI_Clear>().villageWait();
        while (PhotonNetwork.InRoom)
        {
          PhotonNetwork.LeaveRoom();
        }
        GameObject.Find("UI").transform.GetChild(2).GetComponent<UI_Clear>().lobbyWait();
      }
    }

  }

  IEnumerator VillageWaiter()
  {
    Debug.Log("44");

    while (!PhotonNetwork.InLobby) { yield return new WaitForSeconds(0.1f); }
    JoinRoom("Village");
  }

  public void JoinLobby()
  {
    PhotonNetwork.JoinLobby();
  }

  public override void OnConnected()
  {
  }

  public override void OnConnectedToMaster()
  {
    Debug.Log("OCTM");
    PhotonNetwork.JoinLobby();
  }

  public override void OnDisconnected(DisconnectCause cause)
  {
  }

  public override void OnJoinedLobby()
  {
    Debug.LogError("OJL");
  }

  public override void OnCreatedRoom()
  {
    if (!PhotonNetwork.CurrentRoom.Name.Equals("Village"))
      PhotonNetwork.AutomaticallySyncScene = true;
  }

  public override void OnCreateRoomFailed(short returnCode, string message)
  {
  }

  public override void OnJoinRandomFailed(short returnCode, string message)
  {
  }

  public override void OnJoinedRoom()
  {
    if (PhotonNetwork.CurrentRoom.Name.Equals("Village"))
    {
      PhotonNetwork.AutomaticallySyncScene = false;
      PhotonNetwork.LoadLevel("Scenes/Village/Village");
    }
    else if (PhotonNetwork.CurrentRoom.Name.Equals("LoginTest"))
    {
      PhotonNetwork.AutomaticallySyncScene = true;
    }
    else
      PhotonNetwork.AutomaticallySyncScene = true;
  }

  public override void OnLeftRoom()
  {
    Debug.Log("OLR");
    PhotonNetwork.JoinLobby();
    Debug.Log("OLR c");
  }

  public override void OnPlayerEnteredRoom(Player newPlayer)
  {
    if (PhotonNetwork.CurrentRoom.Name.Equals("Village") || PhotonNetwork.CurrentRoom.Name.Equals("LoginTest"))
      return;

    Debug.LogError($"OPER {PhotonNetwork.CurrentRoom.PlayerCount}");

    if (PhotonNetwork.CurrentRoom.PlayerCount >= 6 && PhotonNetwork.LocalPlayer.IsMasterClient)
    {
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
}
