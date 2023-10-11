using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersVillage : MonoBehaviour
{
  static ManagersVillage s_Instance;
  static ManagersVillage Instance { get { Init(); return s_Instance; } }

  InputManagerVillage _inputManagerVillage = new InputManagerVillage();
  public static InputManagerVillage Input { get { return Instance._inputManagerVillage; } }

  static void Init()
  {
    if (s_Instance == null)
    {
      GameObject go = GameObject.Find("@Manager");

      if (go == null)
      {
        go = new GameObject { name = "@Manager" };
        go.AddComponent<ManagersVillage>();

      }
      
      s_Instance = go.GetComponent<ManagersVillage>();
    }
  }
  void Start()
  {
    Init();
  }

  void Update()
  {
    Input.ForUpdate();
  }

  void FixedUpdate()
  {
    Input.ForFixedUpdate();
  }
}
