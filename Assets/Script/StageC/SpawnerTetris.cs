using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnerTetris : MonoBehaviourPun, IInstanceManager
{
  public GameObject[] Tetromino;
  public GameObject[] TetrominoCancer;
  public GameObject[] buttons;
  private bool cancerMode = false;

  // Start is called before the first frame update
  void Start()
  {
  }

  public void NewTetromino()
  {
    GameObject block;
    cancerMode = GameObject.Find("GameManager").GetComponent<TetrisGameManager>().cancerMode;

    if (cancerMode)
      block = IInstanceManager.Instantiate(IInstanceManager.StagePath.StageC, TetrominoCancer[UnityEngine.Random.Range(0, TetrominoCancer.Length)].name, transform.position, Quaternion.identity);
    else
      block = IInstanceManager.Instantiate(IInstanceManager.StagePath.StageC, Tetromino[UnityEngine.Random.Range(0, TetrominoCancer.Length)].name, transform.position, Quaternion.identity);

    block.GetComponent<TetrisBlock>().spawner = this;
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void InitalizeSpawner()
  {
    Debug.LogError("InitalizeSpawner");
    NewTetromino();
  }
}
