using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeController : MonoBehaviour
{

  public EyeType type = EyeType.NONE;

  public Transform rescaleInverse;
  public Transform pupil;

  private Vector3 pupilTarget;

  void Update()
  {
    if (rescaleInverse != null)
    {
      transform.localScale = new Vector3(1 / rescaleInverse.localScale.x, 1 / rescaleInverse.localScale.y, 1 / rescaleInverse.localScale.z);
    }

    bool skipLook = false;
    if (type != EyeType.NONE)
    {
      if (type == EyeType.SQUARE && (Mathf.Abs(Input.GetAxisRaw("Square_Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Square_Vertical")) > 0.1f))
      {
        pupilTarget = new Vector3(0.2f * Input.GetAxisRaw("Square_Horizontal"), 0.2f * Input.GetAxisRaw("Square_Vertical"), -2);
        skipLook = true;
      }
      if (type == EyeType.TRIANGLE && (Mathf.Abs(Input.GetAxisRaw("Triangle_Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Triangle_Vertical")) > 0.1f))
      {
        pupilTarget = new Vector3(0.2f * Input.GetAxisRaw("Triangle_Horizontal"), 0.2f * Input.GetAxisRaw("Triangle_Vertical"), -2);
        skipLook = true;
      }
    }

    if (!skipLook)
    {
      Vector3 minDir = 10 * Vector3.up;
      foreach (PlayerController character in RefManager.instance.allCharacters)
      {
        if (character.gameObject.activeInHierarchy)
        {
          Vector3 dir = (character.transform.position - transform.position);
          dir.z = 0;
          if (dir.sqrMagnitude > 1 && dir.sqrMagnitude < minDir.sqrMagnitude)
          {
            minDir = dir;
          }
        }
      }
      if (minDir.sqrMagnitude < 100)
      {
        Vector3 newPos = (0.05f * (minDir.sqrMagnitude / 100) + 0.15f) * minDir.normalized;
        newPos.z = -2;
        pupilTarget = newPos;
      }
      else
      {
        pupilTarget = new Vector3(0, 0, -2);
      }
    }

    pupil.localPosition = Vector3.Lerp(pupil.localPosition, pupilTarget, Time.deltaTime * 10);
  }

  public enum EyeType
  {
    NONE,
    TRIANGLE,
    SQUARE
  }

}
