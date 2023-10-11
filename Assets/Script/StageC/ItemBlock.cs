using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock : MonoBehaviour
{
  public int type;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter(Collider other)

  {
    if (other.gameObject.CompareTag("PlayerBody"))
    {
      ItemEffects();
      Destroy(this.gameObject);
    }
  }

  void ItemEffects()
  {
    switch (type)
    {
      // fast
      case 0:
        GameObject.Find("GameManager").GetComponent<TetrisGameManager>().fallTime /= 2;
        break;
      // slow
      case 1:
        GameObject.Find("GameManager").GetComponent<TetrisGameManager>().fallTime *= 2;
        break;
      // 이상한 테트리스
      case 2:
        GameObject.Find("GameManager").GetComponent<TetrisGameManager>().cancerMode = true;
        break;
      // 테트리스
      case 3:
        GameObject.Find("GameManager").GetComponent<TetrisGameManager>().cancerMode = false;
        break;
      case 4:
        if (GameObject.FindWithTag("Tetromino"))
        {
          GameObject.Find("GameManager").GetComponent<TetrisGameManager>().DestroyTetrominoItem();
        }
        break;

    }
  }
}
