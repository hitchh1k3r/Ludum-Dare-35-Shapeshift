using UnityEngine;

public class Button : MonoBehaviour
{

  public ButtonCode buttonFrequency;
  public Transform pushPart;

  private bool pushed;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (!pushed)
    {
      if (other.gameObject.GetComponentInParent<PlayerController>() != null)
      {
        SoundPlayer.PlaySound(SoundPlayer.Sound.INTERACT, 0.5f);
        pushed = true;
        StartCoroutine(Tween.EaseCoroutine(pushPart.transform, Tween.TRANSPROP_POSITION, pushPart.transform.localPosition + new Vector3(0, -0.75f, 0), 0.5f, Tween.EASE_QUAD, Tween.EASE_QUAD));
        Door.OpenDoors(this, buttonFrequency);
      }
    }
  }

  public enum ButtonCode
  {
    RED,
    BLUE,
    GREEN,
    ALPHA,
    BETA,
    GAMMA,
    DELTA,
  }

}
