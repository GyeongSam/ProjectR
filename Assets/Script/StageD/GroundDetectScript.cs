using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDetectScript : MonoBehaviour
{
    public GameObject Player;
    PlayerController PlayerScript;

    void Start()
    {
        PlayerScript = Player.GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor")|| other.CompareTag("Player")|| other.CompareTag("Bomb"))
        {
            PlayerScript.is_floor = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor")|| other.CompareTag("Player")|| other.CompareTag("Bomb"))
        {
            PlayerScript.is_floor = false;
        }
    }

}

