using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShootLaser : MonoBehaviour
{
    public Material material;
    LaserBeam beam;

    void Update()
    {
        if (transform.childCount != 1)
            Destroy(transform.GetChild(1).gameObject);

        beam = new LaserBeam(gameObject.transform.position, gameObject.transform.forward, material, gameObject, Define.PlayerColor.Red);
    }
}

