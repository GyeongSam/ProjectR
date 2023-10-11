using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
  static Managers s_Instance;
  static Managers Instance { get { Init(); return s_Instance; } }

  InputManager _inputManager = new InputManager();
  public static InputManager Input { get { return Instance._inputManager; } }

  static void Init()
  {
    if (s_Instance == null)
    {
      GameObject go = GameObject.Find("@Manager");

      if (go == null)
      {
        go = new GameObject { name = "@Manager" };
        go.AddComponent<Managers>();

      }
      s_Instance = go.GetComponent<Managers>();
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
