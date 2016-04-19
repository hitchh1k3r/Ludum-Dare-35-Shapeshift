using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour
{

  public Button.ButtonCode buttonFrequency;

  public static Dictionary<Button.ButtonCode, List<Door>> doors = new Dictionary<Button.ButtonCode, List<Door>>();

  private bool opened;

  void Awake()
  {
    if (!doors.ContainsKey(buttonFrequency))
    {
      doors.Add(buttonFrequency, new List<Door>());
    }
    doors[buttonFrequency].Add(this);
  }

  public static void OpenDoors(Button button, Button.ButtonCode frequency)
  {
    button.StartCoroutine(OpenOneAtATime(frequency));
  }

  private static IEnumerator OpenOneAtATime(Button.ButtonCode frequency)
  {
    if (doors.ContainsKey(frequency))
    {
      foreach (Door door in doors[frequency])
      {
        yield return door.Open();
      }
    }
  }

  private IEnumerator Open()
  {
    if (!opened)
    {
      opened = true;
      PlayerInputControls.extraFocus.Add(transform);
      yield return Tween.EaseCoroutine(transform, Tween.TRANSPROP_POSITION, transform.localPosition + 3 * Vector3.up, 2, Tween.EASE_QUAD, Tween.EASE_QUAD);
      yield return new WaitForSeconds(0.5f);
      PlayerInputControls.extraFocus.Remove(transform);
    }
  }

}
