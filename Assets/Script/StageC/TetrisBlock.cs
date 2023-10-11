using System;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class TetrisBlock : MonoBehaviourPun
{
  private float previousTime;
  public Vector3 rotationPoint;
  public float fallTime;
  public static int height;
  public static int width;
  public static float cube = 2.0000f;
  private static Transform[,] _grid;
  public SpawnerTetris spawner;

  private void Start()
  {
    if (photonView.IsMine)
      this.name = "myTetromino";
    else
      this.name = "notMyTetromino";
    height = GameObject.Find("GameManager").GetComponent<TetrisGameManager>().height;
    fallTime = GameObject.Find("GameManager").GetComponent<TetrisGameManager>().fallTime;

    height = GameObject.Find("GameManager").GetComponent<TetrisGameManager>().height;
    width = GameObject.Find("GameManager").GetComponent<TetrisGameManager>().width;
  }

  // Update is called once per frame
  void Update()
  {
    int[,] tetrominoCodi;

    _grid = GameObject.Find("GameManager").GetComponent<TetrisGameManager>()._grid;

    tetrominoCodi = CheckPosition();

    if (photonView.IsMine)
    {
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {

        for (int i = 0; i < tetrominoCodi.GetLength(0); i++)
        {
          tetrominoCodi[i, 0] -= 2;
        }

        if (ValidMoveWithCodi(tetrominoCodi) && ValidMoveOtherBlockWithCodi(tetrominoCodi))
        {
          transform.position += new Vector3(-cube, 0, 0);
        }
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {

        for (int i = 0; i < tetrominoCodi.GetLength(0); i++)
        {
          tetrominoCodi[i, 0] += 2;
        }

        if (ValidMoveWithCodi(tetrominoCodi) && ValidMoveOtherBlockWithCodi(tetrominoCodi))
        {
          transform.position += new Vector3(cube, 0, 0);
        }
      }
      else if (Input.GetKeyDown(KeyCode.UpArrow))
      {

        TurnTetromino(tetrominoCodi, transform.position.x, transform.position.y);

        if (ValidMoveWithCodi(tetrominoCodi) && ValidMoveOtherBlockWithCodi(tetrominoCodi))
        {
          transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 2), 90);
        }
      }

      tetrominoCodi = CheckPosition();
    }

    _grid = GameObject.Find("GameManager").GetComponent<TetrisGameManager>()._grid;

    if (Time.time - previousTime > (Input.GetKeyDown(KeyCode.DownArrow) && photonView.IsMine ? fallTime / 10 : fallTime))
    {

      for (int i = 0; i < tetrominoCodi.GetLength(0); i++)
      {
        tetrominoCodi[i, 1] -= 2;
      }

      if (ValidMoveWithCodi(tetrominoCodi) && ValidMoveOtherBlockWithCodi(tetrominoCodi))
      {
        transform.position += new Vector3(0, -cube, 0);
      }
      else if (!ValidMoveWithCodi(tetrominoCodi))
      {
        GameObject.Find("GameManager").GetComponent<TetrisGameManager>().AddToGrid(transform);
        this.name = "fallenTetromino";
        this.enabled = false;

        if (photonView.IsMine)
          spawner.NewTetromino();
      }

      previousTime = Time.time;
    }
  }

  static bool ValidMoveWithCodi(int[,] tetrominoCodi)
  {
    for (int i = 0; i < tetrominoCodi.GetLength(0); i++)
    {
      int roundX = Mathf.RoundToInt(tetrominoCodi[i, 0]);
      int roundY = Mathf.RoundToInt(tetrominoCodi[i, 1]);

      if (roundX < 0 || roundX >= width || roundY < 0 || roundY >= height)
      {
        return false;
      }

      if (_grid[roundX, roundY] != null)
      {
        return false;
      }
    }

    return true;
  }

  static bool ValidMoveOtherBlockWithCodi(int[,] tetrominoCodi)
  {
    for (int i = 0; i < tetrominoCodi.GetLength(0); i++)
    {
      int roundX = Mathf.RoundToInt(tetrominoCodi[i, 0]);
      int roundY = Mathf.RoundToInt(tetrominoCodi[i, 1]);

      if (roundX < 1 || roundX >= width || roundY < 0 || roundY >= height)
      {
        return false;
      }

      if (GameObject.Find("notMyTetromino") != null)
      {
        foreach (Transform children in GameObject.Find("notMyTetromino").transform)
        {
          int roundX_other = Mathf.RoundToInt(children.transform.position.x);
          int roundY_other = Mathf.RoundToInt(children.transform.position.y);

          if (roundX == roundX_other && roundY == roundY_other)
          {
            return false;
          }
        }
      }
    }

    return true;
  }

  void TurnTetromino(int[,] tetrominoCodi, float x, float y)
  {
    float[,] temp = new float[tetrominoCodi.Length / 2, 2];

    for (int i = 0; i < tetrominoCodi.Length / 2; i++)
    {
      temp[i, 0] = tetrominoCodi[i, 0] - x;
      temp[i, 1] = tetrominoCodi[i, 1] - y;
    }

    for (int i = 0; i < tetrominoCodi.Length / 2; i++)
    {
      float tempX = temp[i, 0];

      if (x < 0)
      {
        temp[i, 0] = temp[i, 1];
        temp[i, 1] = -tempX;
      }
      else
      {
        temp[i, 0] = -temp[i, 1];
        temp[i, 1] = tempX;
      }
    }

    for (int i = 0; i < tetrominoCodi.Length / 2; i++)
    {
      tetrominoCodi[i, 0] = Mathf.RoundToInt(temp[i, 0] + x);
      tetrominoCodi[i, 1] = Mathf.RoundToInt(temp[i, 1] + y);
    }
  }

  void OnTriggerEnter(Collider other)

  {
    if (other.gameObject.CompareTag("Player") && this.enabled)
    {
      transform.position -= new Vector3(0, -cube, 0);
      GameObject.Find("GameManager").GetComponent<TetrisGameManager>().AddToGrid(transform);
      this.name = "fallenTetromino";
      this.enabled = false;
      spawner.NewTetromino();
    }
  }

  private int[,] CheckPosition()
  {
    int[,] tetrominoCodi;
    int index = 0;

    if (transform.childCount == 4)
    {
      tetrominoCodi = new int[4, 2];

      foreach (Transform children in transform)
      {
        int roundX = Mathf.RoundToInt(children.transform.position.x);
        int roundY = Mathf.RoundToInt(children.transform.position.y);
        tetrominoCodi[index, 0] = roundX;
        tetrominoCodi[index++, 1] = roundY;
      }
    }
    else
    {
      tetrominoCodi = new int[5, 2];

      foreach (Transform children in transform)
      {
        int roundX = Mathf.RoundToInt(children.transform.position.x);
        int roundY = Mathf.RoundToInt(children.transform.position.y);
        tetrominoCodi[index, 0] = roundX;
        tetrominoCodi[index++, 1] = roundY;
      }
    }

    return tetrominoCodi;
  }
}