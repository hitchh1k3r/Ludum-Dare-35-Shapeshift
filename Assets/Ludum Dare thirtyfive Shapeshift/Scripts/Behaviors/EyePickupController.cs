using UnityEngine;

public class EyePickupController : MonoBehaviour
{

  void Update()
  {
    Vector3 minDir = 7 * Vector3.up;
    foreach (PlayerController character in RefManager.instance.allCharacters)
    {
      Vector3 dir = (character.transform.position - transform.position);
      dir.z = 0;
      if (dir.sqrMagnitude < minDir.sqrMagnitude)
      {
        minDir = dir;
      }
    }
    if (minDir.sqrMagnitude < 49)
    {
      transform.position += (((49 - minDir.sqrMagnitude) / 49) * Time.deltaTime) * minDir.normalized;
    }
  }

}
