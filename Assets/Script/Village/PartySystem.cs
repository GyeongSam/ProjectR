using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartySystem : MonoBehaviour
{
  enum StageType
  {
    B,
    C,
    D,
    E
  }

  public GameObject ct;
  public string gameType;
  public Transform gameTypeTap;
  public GameObject detailPopUp;
  public GameObject partyListContent;
  public GameObject partyListEntityPrefab;
  public GameObject partyInfoPrefab;
  GameObject partyInfo;

  public PartyManager partyManager;
  private List<PartyManager.PartyEntity> list;
  private List<GameObject> instanceList = new List<GameObject>();

  public void close()
  {
    gameObject.SetActive(false);
    GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
  }

  public void InstantiatePartyListEntity(PartyManager.PartyEntity entity)
  {
    GameObject entityInstance = Instantiate(partyListEntityPrefab, partyListContent.transform);
    RoomList roomList = entityInstance.GetComponent<RoomList>();
    roomList.SetInfo(entity, partyInfo, entityInstance);

    instanceList.Add(entityInstance);
  }

  void createRoomList(StageType stageType)
  {
    if(instanceList != null)
    {
      foreach(GameObject go in instanceList)
        Destroy(go);

      instanceList.Clear();
    }

    GameObject[] entityList = GameObject.FindGameObjectsWithTag("RoomListEntity");
    foreach (GameObject entity in entityList)
    {
      Destroy(entity);
    }

    StartCoroutine(partyManager.GetPartyList((o) =>
    {
      list = o as List<PartyManager.PartyEntity>;

      foreach (var e in list.Select((value, index) => new { value, index }))
      {
        if (e.value.type.Equals((int)stageType))
          InstantiatePartyListEntity(e.value);
      }
    }
    ));
  }
  // Start is called before the first frame update
  void Start()
  {
    ct = GameObject.Find("UI").transform.GetChild(4).gameObject;
    partyInfo = Instantiate(partyInfoPrefab, transform);
    partyManager = gameObject.AddComponent<PartyManager>();

    createRoomList(StageType.B);
  }

  private void OnEnable()
  {
    for (int i = 4; i < 8; ++i)
    {
      gameTypeTap.GetChild(i).gameObject.SetActive(false);
    }
    if (gameType == "B")
    {
      gameTypeTap.GetChild(4).gameObject.SetActive(true);
    }
    else if (gameType == "C")
    {
      gameTypeTap.GetChild(5).gameObject.SetActive(true);
    }
    else if (gameType == "D")
    {
      gameTypeTap.GetChild(6).gameObject.SetActive(true);
    }
    else if (gameType == "E")
    {
      gameTypeTap.GetChild(7).gameObject.SetActive(true);
    }

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Escape)&&!ct.activeSelf)
    {
      close();
    }
  }

  public void RefreshPartyList()
  {
    if (gameType == "B")
    {
      createRoomList(StageType.B);
    }
    else if (gameType == "C")
    {
      createRoomList(StageType.C);
    }
    else if (gameType == "D")
    {
      createRoomList(StageType.D);
    }
    else if (gameType == "E")
    {
      createRoomList(StageType.E);
    }

  }
  public void CreatRoomPopUpOn()
  {
    GameObject.Find("UI").transform.GetChild(4).gameObject.SetActive(true);
  }
}
