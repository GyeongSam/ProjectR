using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using static WebManager;

public class SignInPanel : MonoBehaviourPunCallbacks
{
  public GameObject mainPanel;
  public GameObject signupPanel;

  TMP_InputField id;
  TMP_InputField password;

  private int next_focus = 2;

  MyNetworkManager networkManager;

  private void Start()
  {
    //networkManager = transform.parent.parent.GetComponent<MyNetworkManager>();
    networkManager = GameObject.Find("NetworkManager").gameObject.GetComponent<MyNetworkManager>();
  }

  private void OnEnable()
  {
    transform.GetChild(1).transform.GetChild(4).gameObject.SetActive(false);

    id = transform.GetChild(1).GetChild(2).GetComponent<TMP_InputField>();
    password = transform.GetChild(1).GetChild(3).GetComponent<TMP_InputField>();

    nextFocus();
  }
  public void nextFocus()
  {
    transform.GetChild(1).transform.GetChild(next_focus).GetComponent<Selectable>().Select();
  }
  public void editNextIdx(int next_idx)
  {
    next_focus = next_idx;
  }


  public void API_Signin()
  {
    StartCoroutine(SignIn(id.text, password.text));
  }
  public void back()
  {
    mainPanel.SetActive(true);
    gameObject.SetActive(false);
  }
  public void Signup()
  {
    signupPanel.SetActive(true);
    gameObject.SetActive(false);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Return))
    {
      API_Signin();
    }
    else if (Input.GetKeyDown(KeyCode.Escape))
    {
      back();
    }
    else if (Input.GetKeyDown(KeyCode.Tab))
    {
      nextFocus();
    }
  }

  IEnumerator SignIn(string id, string password)
  {
    WebManager.MemberReqDto memberReqDto = new WebManager.MemberReqDto();
    memberReqDto.id = id;
    memberReqDto.password = password;

    yield return StartCoroutine(WebManager.Instance().WebRequest("member/login", "GET", memberReqDto));

    WebManager.LoginRespDto loginRespDto = JsonConvert.DeserializeObject<WebManager.LoginRespDto>(WebManager.Instance().WebRequestResult);

    if (loginRespDto.successFailure)
    {
            transform.GetChild(1).transform.GetChild(4).gameObject.SetActive(false);
            PlayerPrefs.SetString("userName", loginRespDto.id);
      PlayerPrefs.SetInt("nowEggNum", loginRespDto.nowEgg);
      StartCoroutine(MyEggInfo());
    }

    else
    {
      transform.GetChild(1).transform.GetChild(4).gameObject.SetActive(true);
    }
  }
  IEnumerator MyEggInfo()
  {
    yield return StartCoroutine(WebManager.Instance().WebRequest($"member/egg-possession-info/{PlayerPrefs.GetString("userName")}", "GET", null));

    string jsonArrayStr = "{\"eggPossessionInfo\" :" + WebManager.Instance().WebRequestResult + "}";
    WebManager.EggPossessionRespDto eggPossessionRespDto = JsonConvert.DeserializeObject<EggPossessionRespDto>(jsonArrayStr);//JsonConvert.DeserializeObject<WebManager.EggPossessionRespDto>(jsonArrayStr);
    char[] temp = new char[100];
    int temp2 = 0;
    for (int i = 0; i < 100; ++i)
    {
      temp[i] = '0';
    }
    foreach (int i in eggPossessionRespDto.eggPossessionInfo)
    {
      temp[i - 1] = '1';
      temp2++;
    }
    string a = new string(temp);
    PlayerPrefs.SetString("MyEggList",a);
    PlayerPrefs.SetInt("level", temp2);

    MyNetworkManager.Instance().SetLocalPlayerNickname(PlayerPrefs.GetString("userName"));
    MyNetworkManager.Instance().JoinRoom("Village");

  }



  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    Debug.Log("OJR");
    SceneManager.LoadScene("Scenes/Village/Village");
  }

  public override void OnJoinRoomFailed(short returnCode, string message)
  {
    Debug.Log($"OJRF {message}");
  }
}
