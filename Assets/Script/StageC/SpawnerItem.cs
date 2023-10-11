using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpawnerItem : MonoBehaviour
{
    public GameObject[] Items;
    private float previousTime;
    private float spwanTime = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - previousTime > spwanTime)
        {
            float posX = Random.Range(1.0f, 47.0f);
            float posY = 37.5f;
            float posZ = -1.0f;

            Vector3 spawnPos = new Vector3(posX, posY, posZ);

            GameObject temp = Instantiate(Items[UnityEngine.Random.Range(0, Items.Length)], spawnPos, Quaternion.identity);

            previousTime = Time.time;
        }
    }
}
