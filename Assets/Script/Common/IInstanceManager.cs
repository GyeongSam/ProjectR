using System.Collections;
using UnityEngine;
using Photon.Pun;

public interface IInstanceManager
{
  public enum StagePath
  {
    StageB,
    StageC,
    StageD,
    StageE
  }

  public static GameObject Instantiate(StagePath path, string prefabName, Vector3 pos, Quaternion rot)
  {
    return PhotonNetwork.Instantiate(path.ToString() + "/" + prefabName, pos, rot);
  }

  public static GameObject InstantiatePlayerCharacter(int number, Vector3 pos, Quaternion rot)
  {
    string path = "Characters/Egg" + number.ToString();
    return PhotonNetwork.Instantiate(path, pos, rot);
  }

  public static IEnumerator Destroy(GameObject targetGo, float duration = 0)
  {
    yield return new WaitForSeconds(duration);
    PhotonNetwork.Destroy(targetGo);
  }
}