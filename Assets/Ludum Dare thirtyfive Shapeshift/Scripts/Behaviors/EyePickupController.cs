using UnityEngine;

public class EyePickupController : MonoBehaviour
{

  [System.NonSerialized]
  public bool noCamera;

  void Update()
  {
    Vector3 minDir = 7 * Vector3.up;
    foreach (PlayerController character in RefManager.instance.allCharacters)
    {
      if (character.gameObject.activeInHierarchy)
      {
        Vector3 dir = (character.transform.position - transform.position);
        dir.z = 0;
        if (dir.sqrMagnitude < minDir.sqrMagnitude)
        {
          minDir = dir;
        }
      }
    }
    if (minDir.sqrMagnitude < 49)
    {
      transform.position += (((49 - minDir.sqrMagnitude) / 49) * Time.deltaTime) * minDir.normalized;
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.GetComponent<PlayerController>() != null)
    {
      gameObject.SetActive(false);
      RefManager.instance.circle.transform.position = collision.gameObject.transform.position;
      RefManager.instance.square.gameObject.SetActive(false);
      RefManager.instance.triangle.gameObject.SetActive(false);
      RefManager.instance.circle.gameObject.SetActive(true);
      RefManager.instance.circle.transform.localScale = 0.25f * Vector3.one;
      RefManager.instance.circle.StartCoroutine(Tween.EaseCoroutine(RefManager.instance.circle.transform, Tween.TRANSPROP_SCALE, Vector3.one, 0.75f, Tween.EASE_NONE, Tween.EASE_ELASTIC));
      SoundPlayer.PlaySound(SoundPlayer.Sound.COMBINE, 1);
    }
  }

}
