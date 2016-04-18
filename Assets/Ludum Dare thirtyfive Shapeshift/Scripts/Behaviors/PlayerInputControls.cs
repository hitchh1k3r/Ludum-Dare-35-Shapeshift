using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class PlayerInputControls : MonoBehaviour
{

  // Input joystick 1 and 2, or keyboard
  // public KeyCode splitJoin, triangleUp, triangleLeft, triangleRight, triangleDown, squareUp, squareLeft, squareRight, squareDown;

  public MozaicManager currentWorld;

  public List<PlayerController> characterControllers;
  public static List<Transform> extraFocus = new List<Transform>();
  public static float lockScale;
  public static float levelStep = 0;

  // private int characterControlled;
  new private Camera camera;
  private Coroutine shaker;
  private Vector3 cameraWorldOffset;

  void Awake()
  {
    camera = GetComponent<Camera>();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (RefManager.instance.circle.gameObject.activeSelf)
      {
        RefManager.instance.circle.gameObject.SetActive(false);
        RefManager.instance.square.gameObject.SetActive(true);
        RefManager.instance.triangle.gameObject.SetActive(true);
      }
      else
      {
        RefManager.instance.circle.gameObject.SetActive(true);
        RefManager.instance.square.gameObject.SetActive(false);
        RefManager.instance.triangle.gameObject.SetActive(false);
      }
    }
    if (Input.GetButtonDown("Shapeshift"))
    {
      // characterControlled = (characterControlled + 1) % characterControllers.Count;
    }
    else
    {
      // characterControllers[characterControlled].ControlUpdate();
    }
    Vector3 targetPos = Vector3.zero;
    float targetSize = 6;

    float minX = float.PositiveInfinity;
    float maxX = float.NegativeInfinity;
    float minY = float.PositiveInfinity;
    float maxY = float.NegativeInfinity;
    foreach (PlayerController character in characterControllers)
    {
      if (character.gameObject.activeInHierarchy)
      {
        if (character.transform.position.x > maxX)
        {
          maxX = character.transform.position.x;
        }
        if (character.transform.position.x < minX)
        {
          minX = character.transform.position.x;
        }
        if (character.transform.position.y > maxY)
        {
          maxY = character.transform.position.y;
        }
        if (character.transform.position.y < minY)
        {
          minY = character.transform.position.y;
        }
      }
    }

    foreach (Transform extraTransform in extraFocus)
    {
      if (extraTransform.position.x > maxX)
      {
        maxX = extraTransform.position.x;
      }
      if (extraTransform.position.x < minX)
      {
        minX = extraTransform.position.x;
      }
      if (extraTransform.position.y > maxY)
      {
        maxY = extraTransform.position.y;
      }
      if (extraTransform.position.y < minY)
      {
        minY = extraTransform.position.y;
      }
    }

    targetPos = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);

    // FIXME (hitch) make this somehow link to current world offset!
    if (levelStep > 0)
    {
      targetPos.y = Mathf.Round((targetPos.y + 2.5f) / levelStep) * levelStep;
    }

    if (lockScale > 0)
    {
      targetSize = lockScale;
    }
    else
    {
      maxX = 0;
      maxY = 0;
      foreach (PlayerController character in characterControllers)
      {
        if (character.gameObject.activeInHierarchy)
        {
          float xDist = Mathf.Abs(targetPos.x - character.transform.position.x) + 1.5f;
          float yDist = Mathf.Abs(targetPos.y - character.transform.position.y) + 1.5f;
          if (xDist > maxX)
          {
            maxX = xDist;
          }
          if (yDist > maxY)
          {
            maxY = yDist;
          }
        }
      }
      foreach (Transform extraTransform in extraFocus)
      {
        float xDist = Mathf.Abs(targetPos.x - extraTransform.position.x) + 1.5f;
        float yDist = Mathf.Abs(targetPos.y - extraTransform.position.y) + 1.5f;
        if (xDist > maxX)
        {
          maxX = xDist;
        }
        if (yDist > maxY)
        {
          maxY = yDist;
        }
      }

      float aspect = Screen.height / (Screen.width * 0.625f);
      targetSize = Mathf.Max(6, maxY, maxX * aspect);
    }

    float fact = Time.deltaTime * ((camera.transform.position - targetPos).sqrMagnitude / 30);
    Vector3 pos = Vector2.Lerp(camera.transform.position, targetPos, fact);

    camera.transform.position = new Vector3(pos.x, pos.y, -10);
    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, Time.deltaTime * 10);
    cameraWorldOffset = camera.transform.position - currentWorld.transform.position;
  }

  public void ScreenShake(float amount)
  {
    amount = Mathf.Clamp01(amount);
    if (amount > 0)
    {
      if (shaker != null)
      {
        StopCoroutine(shaker);
      }
      shaker = StartCoroutine(DoShake(amount));
    }
  }

  private IEnumerator DoShake(float amount)
  {
    while (amount > 0)
    {
      amount -= Time.deltaTime * 1.5f;
      camera.transform.position += amount * (Vector3)Random.insideUnitCircle;
      yield return null;
    }
  }

  public void ResetCameraOffset()
  {
    camera.transform.position = currentWorld.transform.position + cameraWorldOffset;
  }

}
