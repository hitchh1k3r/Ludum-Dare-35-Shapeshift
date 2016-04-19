using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class Fullscreener : UnityEngine.UI.Button
{

  new private UnityEngine.UI.Image image;
  private UnityEngine.UI.Text text;
  private bool lastState;

  new void Awake()
  {
    image = GetComponent<UnityEngine.UI.Image>();
    text = GetComponentInChildren<UnityEngine.UI.Text>();
    base.Awake();
  }

  void Update()
  {
    if (lastState != Screen.fullScreen)
    {
      lastState = Screen.fullScreen;
      if (lastState)
      {
        text.enabled = false;
        image.enabled = false;
      }
      else
      {
        text.enabled = true;
        image.enabled = true;
      }
    }
  }

  public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
  {
    Screen.fullScreen = true;
    base.OnPointerDown(eventData);
  }

}
