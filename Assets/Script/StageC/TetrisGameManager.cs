using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TetrisGameManager : MonoBehaviour
{
  public int getScore = 0;
  int cube = 2;
  public float fallTime = 1.0f;
  public Boolean cancerMode = false;
  public int height = 46;
  public int width = 48;
  public Transform[,] _grid;
  public string output;
  public GameObject ui;

  // Start is called before the first frame update
  void Start()
  {
    _grid = new Transform[width, height];
    SetCountText();
  }

  // Update is called once per frame  
  void Update()
  {
  }

  public void AddToGrid(Transform _transform)
  {
    foreach (Transform children in _transform)
    {
      int roundX = Mathf.RoundToInt(children.transform.position.x);
      int roundY = Mathf.RoundToInt(children.transform.position.y);

      if (roundY % 2 == 1)
        roundY--;

      _grid[roundX, roundY] = children;

      CheckForLines();
    }
  }

  void CheckForLines()
  {
    Boolean isButtom = false;
    int cnt = 0;

    for (int i = 0; i < height; i++)
    {
      if (HasLine(i))
      {
        if (i == 0)
          isButtom = true;

        DeleteLine(i);
        RowDown(i);
        i--;

        if (isButtom)
          cnt++;
      }
    }

    getScore += cnt;
    SetCountText();
    if (getScore == 10)
    {
      //ui.GetChild(2).gameObject.SetActive(true);
      Time.timeScale = 0f;
    }
  }

  bool HasLine(int i)
  {
    for (int j = 2; j < width; j += 2)
    {
      if (_grid[j, i] == null)
        return false;
    }

    return true;
  }

  void DeleteLine(int i)
  {
    for (int j = 2; j < width; j += 2)
    {
      Destroy(_grid[j, i].gameObject);
      _grid[j, i] = null;
    }
  }

  void RowDown(int i)
  {
    for (int y = i; y < height; y++)
    {
      for (int j = 0; j < width; j++)
      {
        if (_grid[j, y] != null)
        {
          _grid[j, y - 2] = _grid[j, y];
          _grid[j, y] = null;
          _grid[j, y - 2].transform.position -= new Vector3(0, cube, 0);
        }
      }
    }
  }

  public void DestroyTetrominoItem()
  {
    for (int x = 0; x < width; x++)
    {
      int cnt = 0;
      for (int y = height - 1; y >= 0; y--)
      {
        if (cnt == 2)
          break;


        if (_grid[x, y] != null)
        {
          Destroy(_grid[x, y].gameObject);
          _grid[x, y] = null;
          cnt++;
        }
      }
    }
  }

  void SetCountText()
  {
    GameObject.Find("TetrisScore").GetComponent<TextMeshProUGUI>().text = "[Score] " + getScore.ToString();
  }
}
