using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ending : MonoBehaviour
{

  public GameObject enableThis;
  public GameObject dis1, dis2;

  bool done;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (!done)
    {
      if (other.GetComponentInParent<PlayerController>() != null)
      {
        done = true;
        dis1.SetActive(false);
        dis2.SetActive(false);
        enableThis.SetActive(true);
        PlayerInputControls.moveSpeed = 0.01f;
        PlayerInputControls.lockPlace = new Vector3(-66, -229, -10);
        PlayerInputControls.lockScale = 400;
      }
    }
  }

}
