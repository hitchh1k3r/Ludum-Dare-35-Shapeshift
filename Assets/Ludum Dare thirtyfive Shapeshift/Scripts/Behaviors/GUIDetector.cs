using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIDetector : MonoBehaviour
{

  public TrackType type;

  private bool lastState;

  void Awake()
  {
    foreach (Graphic drawable in GetComponentsInChildren<Graphic>())
    {
      drawable.color = new Color(1, 1, 1, 0);
    }
  }

  void Update()
  {
    bool newState = lastState;
    if (type == TrackType.CIRCLE)
    {
      newState = RefManager.instance.circle.gameObject.activeInHierarchy && !RefManager.instance.circle.inNode;
    }
    else if (type == TrackType.TRIANGLE)
    {
      newState = RefManager.instance.triangle.gameObject.activeInHierarchy && !RefManager.instance.triangle.inNode;
    }
    else if (type == TrackType.SQUARE)
    {
      newState = RefManager.instance.square.gameObject.activeInHierarchy && !RefManager.instance.square.inNode;
    }
    else if (type == TrackType.TRIANGLE_NODE)
    {
      newState = RefManager.instance.triangle.inNode || RefManager.instance.circle.inNode;
    }
    else if (type == TrackType.SQUARE_NODE)
    {
      newState = RefManager.instance.square.inNode || RefManager.instance.circle.inNode;
    }

    if (lastState != newState)
    {
      lastState = newState;
      foreach (Graphic drawable in GetComponentsInChildren<Graphic>())
      {
        StartCoroutine(Tween.EaseCoroutine(drawable, Tween.UIPROP_COLOR, new Color(1, 1, 1, (newState ? 1 : 0)), 0.5f, Tween.EASE_QUAD, Tween.EASE_QUAD));
      }
    }
  }

  public enum TrackType
  {
    CIRCLE,
    SQUARE,
    TRIANGLE,
    SQUARE_NODE,
    TRIANGLE_NODE
  }

}
