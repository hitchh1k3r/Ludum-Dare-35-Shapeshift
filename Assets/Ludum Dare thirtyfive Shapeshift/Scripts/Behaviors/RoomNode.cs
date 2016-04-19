using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class RoomNode : MonoBehaviour
{

  public MozaicManager mozaic;
  public Transform room;

  public GameObject visualCircle, visualBox, visualTriangle;

  new private Collider2D collider;
  private PlayerController pilot;
  private bool inProgress;

  private bool leaveFast;

  void Awake()
  {
    collider = GetComponent<Collider2D>();
  }

  void LateUpdate()
  {
    if (pilot != null)
    {
      pilot.transform.position = new Vector3(transform.position.x, transform.position.y, 0.15f);
    }
  }

  void Update()
  {
    if (pilot != null)
    {
      if (Input.GetButtonUp("Square_Shift") || Input.GetButtonUp("Triangle_Shift"))
      {
        if (pilot.shape == PlayerController.CharacterShape.CIRCLE)
        {
          leaveFast = true;
        }
        else if (pilot.shape == PlayerController.CharacterShape.SQUARE && Input.GetButtonUp("Square_Shift"))
        {
          leaveFast = true;
        }
        else if (pilot.shape == PlayerController.CharacterShape.TRIANGLE && Input.GetButtonUp("Triangle_Shift"))
        {
          leaveFast = true;
        }
      }
      if (leaveFast && !inProgress)
      {
        leaveFast = false;
        pilot.inNode = false;
        pilot = null;
        SoundPlayer.PlaySound(SoundPlayer.Sound.INTERACT, 0.5f);
        visualCircle.SetActive(false);
        visualBox.SetActive(false);
        visualTriangle.SetActive(false);
        PlayerInputControls.lockScale = 0;
        PlayerInputControls.extraFocus.Remove(room);
      }
      else if (!inProgress)
      {
        if ((pilot.shape == PlayerController.CharacterShape.CIRCLE || pilot.shape == PlayerController.CharacterShape.TRIANGLE) && (Input.GetAxisRaw("Triangle_Horizontal") < -0.1f || Input.GetAxisRaw("Triangle_Horizontal") > 0.1f))
        {
          StartCoroutine(DoTransform(0, 0, ((Input.GetAxisRaw("Triangle_Horizontal") < 0) ? 90 : -90)));
        }
        else if ((pilot.shape == PlayerController.CharacterShape.CIRCLE || pilot.shape == PlayerController.CharacterShape.SQUARE) && (Input.GetAxisRaw("Square_Horizontal") < -0.1f || Input.GetAxisRaw("Square_Horizontal") > 0.1f))
        {
          if (mozaic.MoveCell(room, (Input.GetAxisRaw("Square_Horizontal") < 0) ? MozaicManager.MoveDirection.LEFT : MozaicManager.MoveDirection.RIGHT))
          {
            StartCoroutine(DoTransform(((Input.GetAxisRaw("Square_Horizontal") < 0) ? -10 : 10), 0, 0));
          }
        }
        else if ((pilot.shape == PlayerController.CharacterShape.CIRCLE || pilot.shape == PlayerController.CharacterShape.SQUARE) && (Input.GetAxisRaw("Square_Vertical") < -0.1f || Input.GetAxisRaw("Square_Vertical") > 0.1f))
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
      if (Input.GetButtonUp("Square_Shift") || Input.GetButtonUp("Triangle_Shift"))
      {
        if (Physics2D.IsTouching(RefManager.instance.circle.collider, collider))
        {
          visualCircle.SetActive(true);
          pilot = RefManager.instance.circle;
        }
        else if (Input.GetButtonUp("Square_Shift") && Physics2D.IsTouching(RefManager.instance.square.collider, collider))
        {
          visualBox.SetActive(true);
          pilot = RefManager.instance.square;
        }
        else if (Input.GetButtonUp("Triangle_Shift") && Physics2D.IsTouching(RefManager.instance.triangle.collider, collider))
        {
          visualTriangle.SetActive(true);
          pilot = RefManager.instance.triangle;
        }
        if (pilot != null)
        {
          SoundPlayer.PlaySound(SoundPlayer.Sound.INTERACT, 0.5f);
          pilot.inNode = true;
          PlayerInputControls.lockScale = 12;
          PlayerInputControls.extraFocus.Add(room);
        }
      }
    }
  }

  IEnumerator DoTransform(float deltaX, float deltaY, float deltaTheta)
  {
    inProgress = true;
    if (deltaX != 0 || deltaY != 0)
    {
      SoundPlayer.PlaySound(SoundPlayer.Sound.BLOCK_SLIDE, 0.5f);
      yield return Tween.EaseCoroutine(room, Tween.TRANSPROP_POSITION, room.localPosition + new Vector3(deltaX, deltaY, 0), 0.75f, Tween.EASE_QUAD, Tween.EASE_QUAD);
    }
    if (deltaTheta != 0)
    {
      SoundPlayer.PlaySound(SoundPlayer.Sound.BLOCK_ROTATE, 0.5f);
      yield return Tween.EaseCoroutine(room, Tween.TRANSPROP_ROTATION, Quaternion.Euler(0, 0, deltaTheta) * room.localRotation, 0.75f, Tween.EASE_QUAD, Tween.EASE_QUAD);
    }
    inProgress = false;
    SoundPlayer.PlaySound(SoundPlayer.Sound.BLOCK_STOP, 0.25f);
    yield break;
  }

}
