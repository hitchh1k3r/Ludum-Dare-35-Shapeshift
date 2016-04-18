using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class RoomNode : MonoBehaviour
{

  public MozaicManager mozaic;
  public Transform room;
  public PlayerController circle;

  new private Collider2D collider;
  private bool active;
  private bool inProgress;

  void Awake()
  {
    collider = GetComponent<Collider2D>();
  }

  void Update()
  {
    if (active)
    {
      if (!inProgress)
      {
        if (Input.GetButtonDown("Shapeshift"))
        {
          active = false;
          PlayerInputControls.lockScale = 0;
          PlayerInputControls.extraFocus.Remove(room);
          circle.transform.position = transform.position;
          circle.gameObject.SetActive(true);
        }
        else if (Input.GetAxisRaw("Triangle_Horizontal") < -0.1f || Input.GetAxisRaw("Triangle_Horizontal") > 0.1f)
        {
          StartCoroutine(DoTransform(0, 0, ((Input.GetAxisRaw("Triangle_Horizontal") < 0) ? 90 : -90)));
        }
        else if (Input.GetAxisRaw("Square_Horizontal") < -0.1f || Input.GetAxisRaw("Square_Horizontal") > 0.1f)
        {
          if (mozaic.MoveCell(room, (Input.GetAxisRaw("Square_Horizontal") < 0) ? MozaicManager.MoveDirection.LEFT : MozaicManager.MoveDirection.RIGHT))
          {
            StartCoroutine(DoTransform(((Input.GetAxisRaw("Square_Horizontal") < 0) ? -10 : 10), 0, 0));
          }
        }
        else if (Input.GetAxisRaw("Square_Vertical") < -0.1f || Input.GetAxisRaw("Square_Vertical") > 0.1f)
        {
          if (mozaic.MoveCell(room, (Input.GetAxisRaw("Square_Vertical") < 0) ? MozaicManager.MoveDirection.DOWN : MozaicManager.MoveDirection.UP))
          {
            StartCoroutine(DoTransform(0, ((Input.GetAxisRaw("Square_Vertical") < 0) ? -10 : 10), 0));
          }
        }
      }
    }
    else
    {
      if (Input.GetButtonDown("Shapeshift"))
      {
        if (Physics2D.IsTouching(circle.collider, collider))
        {
          active = true;
          PlayerInputControls.lockScale = 12;
          PlayerInputControls.extraFocus.Add(room);
          circle.gameObject.SetActive(false);
        }
      }
    }
  }

  IEnumerator DoTransform(float deltaX, float deltaY, float deltaTheta)
  {
    inProgress = true;
    if (deltaX != 0 || deltaY != 0)
    {
      yield return Tween.EaseCoroutine(room, Tween.TRANSPROP_POSITION, room.localPosition + new Vector3(deltaX, deltaY, 0), 0.5f, Tween.EASE_QUAD, Tween.EASE_QUAD);
    }
    if (deltaTheta != 0)
    {
      yield return Tween.EaseCoroutine(room, Tween.TRANSPROP_ROTATION, Quaternion.Euler(0, 0, deltaTheta) * room.rotation, 0.5f, Tween.EASE_QUAD, Tween.EASE_QUAD);
    }
    inProgress = false;
    yield break;
  }

}
