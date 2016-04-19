using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.GetComponentInParent<PlayerController>() != null)
    {
      RefManager.instance.respawnLocation = transform;
    }
  }

}
