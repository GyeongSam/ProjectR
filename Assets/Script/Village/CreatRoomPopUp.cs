using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CreatRoomPopUp : MonoBehaviour
{
  public string gameType;
  public GameObject failmessage;
  public TMP_Text input;
  public TMP_Text gameTypeBox;
  //public GameObject pm;

  public void Start()
  {
    //pm = GameObject.Find("UI").transform.GetChild(3).gameObject;
  }

  public void OnEnable()
  {
    gameType = GameObject.Find("UI").transform.GetChild(3).gameObject.GetComponent<PartySystem>().gameType;
    gameTypeBox.text = "Stage " + gameType;
  }

  public void Create()
  {
    Debug.Log("Create");
    int temp=0;
    if (gameType == "B") temp = 0;
    else if (gameType == "C") temp = 1;
    else if (gameType == "D") temp = 2;
    else if (gameType == "E") temp= 3;
    GameObject.Find("UI").transform.GetChild(3).transform.GetChild(2).gameObject.GetComponent<TeamInfoPopup>().CreateRoom(input.text, PlayerPrefs.GetString("userName"), temp);
  }
  public void Quit()
  {
    Debug.Log("Quit");
    gameObject.SetActive(false);
  }
 

}
