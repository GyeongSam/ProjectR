using System.Collections.Generic;
using UnityEngine;

public class LaserBeam
{
    Vector3 pos, dir;

    GameObject laserObj;
    LineRenderer laser;

    Material material;

    Vector3 endPoint;

    Define.PlayerColor color;

    Dictionary<Define.PlayerColor, Color> findColor = new Dictionary<Define.PlayerColor, Color>()
    {
        { Define.PlayerColor.None, Color.white },
        { Define.PlayerColor.Red, Color.red },
        { Define.PlayerColor.Orange, new Color(203f/255f, 61f/255f, 0/255f) },
        { Define.PlayerColor.Yellow, Color.yellow },
        { Define.PlayerColor.Green, Color.green },
        { Define.PlayerColor.Blue, Color.blue },
        { Define.PlayerColor.Purple, new Color(145f/255f, 0/255f, 145f/255f) }
    };

    public LaserBeam(Vector3 pos, Vector3 dir, Material material, GameObject parent, Define.PlayerColor color)
    {
        laser = new LineRenderer();
        laserObj = new GameObject();
        laserObj.transform.parent = parent.transform;
        laserObj.name = "Laser Beam";

        this.pos = pos;
        this.dir = dir;
        this.material = material;
        this.color = color;

        laser = laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        laser.material = material;

        laser.startColor = findColor[color];
        laser.endColor = findColor[color];

        CastRay(pos, dir, laser);
    }

    void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 300))
        {
            Vector3 reflectDir = Vector3.Reflect(dir, hit.normal);
            endPoint = hit.point;
            MakeLaser();

            if (hit.collider.gameObject.tag == "Mirror" && hit.transform.gameObject.GetComponent<PlayerControllerE>().PlayerColor == color)
                new LaserBeam(endPoint, reflectDir, material, laserObj, (color + 1));

            if (color == Define.PlayerColor.None && hit.collider.gameObject.name == "Target")
            {
                LaserTarget lt = hit.collider.gameObject.GetComponent<LaserTarget>();
                if (lt.Count == 0)
                    lt.StartCheck();
                lt.Count += 1;
            }   
        }

        else
        {
            endPoint = ray.GetPoint(300);
            MakeLaser();
        }
    }

    void MakeLaser()
    {
        laser.SetPosition(0, pos);
        laser.SetPosition(1, endPoint);
    }
}
