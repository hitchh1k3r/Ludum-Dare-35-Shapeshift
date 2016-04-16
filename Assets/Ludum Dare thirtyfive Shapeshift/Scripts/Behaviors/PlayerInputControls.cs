using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class PlayerInputControls : MonoBehaviour
{

  public List<CharacterController> characterControllers;

  private int characterControlled;
  new private Camera camera;

  void Awake()
  {
    camera = GetComponent<Camera>();
  }

  void Update()
  {
    if (Input.GetButtonDown("Shapeshift"))
    {
      characterControlled = (characterControlled + 1) % characterControllers.Count;
    }
    else
    {
      characterControllers[characterControlled].ControlUpdate();
    }
    Vector3 pos = Vector2.Lerp(camera.transform.position, characterControllers[characterControlled].transform.position, Time.deltaTime * 30);
    camera.transform.position = new Vector3(pos.x, pos.y, -10);
  }

}
