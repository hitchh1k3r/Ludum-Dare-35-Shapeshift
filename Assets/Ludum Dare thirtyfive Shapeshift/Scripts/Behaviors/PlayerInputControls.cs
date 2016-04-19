using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class PlayerInputControls : MonoBehaviour
{

  public MozaicManager currentWorld;
  public Transform universe;

  public List<Transform> objectsLinkedToWorld = new List<Transform>();

  public List<PlayerController> characterControllers;
  public static List<Transform> extraFocus = new List<Transform>();
  public static float lockScale;
  public static float levelStep = 0;
  public static Vector3 lockPlace = Vector3.zero;
  public static float moveSpeed = 1;

  new private Camera camera;
  private Coroutine shaker;
  private bool respawning;

  void Awake()
  {
    camera = GetComponent<Camera>();
  }

  void LateUpdate()
  {
    if (currentWorld.transform.rotation != Quaternion.identity)
    {
      Quaternion rot = currentWorld.transform.rotation;
      Quaternion reverse = Quaternion.Inverse(rot);
      Quaternion[] rotations = new Quaternion[objectsLinkedToWorld.Count];
      int i = 0;
      foreach (Transform o in objectsLinkedToWorld)
      {
        rotations[i] = o.rotation;
        o.parent = currentWorld.transform;
        ++i;
      }
      Vector2 camOffset = reverse * ((Vector2)camera.transform.position - currentWorld.phyics.centerOfMass);
      universe.RotateAround(currentWorld.phyics.centerOfMass, Vector3.back, rot.eulerAngles.z);
      currentWorld.transform.rotation = Quaternion.identity;
      currentWorld.phyics.velocity = reverse * currentWorld.phyics.velocity;
      camera.transform.position = (Vector3)(currentWorld.phyics.centerOfMass + camOffset) + new Vector3(0, 0, -10);
      i = 0;
      foreach (Transform o in objectsLinkedToWorld)
      {
        o.parent = null;
        o.rotation = rotations[i];
        ++i;
      }
    }
  }

  void Update()
  {
    bool alive = false;
    foreach (PlayerController character in characterControllers)
    {
      if (character.gameObject.activeInHierarchy)
      {
        alive = true;
        break;
      }
    }
    if (!alive && !respawning)
    {
      respawning = true;
      StartCoroutine(Respawn());
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
    if (RefManager.instance.squareEye.gameObject.activeInHierarchy && !RefManager.instance.squareEye.noCamera)
    {
      if (RefManager.instance.squareEye.transform.position.x > maxX)
      {
        maxX = RefManager.instance.squareEye.transform.position.x;
      }
      if (RefManager.instance.squareEye.transform.position.x < minX)
      {
        minX = RefManager.instance.squareEye.transform.position.x;
      }
      if (RefManager.instance.squareEye.transform.position.y > maxY)
      {
        maxY = RefManager.instance.squareEye.transform.position.y;
      }
      if (RefManager.instance.squareEye.transform.position.y < minY)
      {
        minY = RefManager.instance.squareEye.transform.position.y;
      }
    }
    if (RefManager.instance.triangleEye.gameObject.activeInHierarchy && !RefManager.instance.triangleEye.noCamera)
    {
      if (RefManager.instance.triangleEye.transform.position.x > maxX)
      {
        maxX = RefManager.instance.triangleEye.transform.position.x;
      }
      if (RefManager.instance.triangleEye.transform.position.x < minX)
      {
        minX = RefManager.instance.triangleEye.transform.position.x;
      }
      if (RefManager.instance.triangleEye.transform.position.y > maxY)
      {
        maxY = RefManager.instance.triangleEye.transform.position.y;
      }
      if (RefManager.instance.triangleEye.transform.position.y < minY)
      {
        minY = RefManager.instance.triangleEye.transform.position.y;
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
    // for now it'll just have arbitrary height jumps... whoops no time to fix ^_^
    if (levelStep > 0)
    {
      targetPos.y = Mathf.Round((targetPos.y + 2.5f) / levelStep) * levelStep;
    }

    if (lockPlace != Vector3.zero)
    {
      targetPos = lockPlace;
    }

    float fact = Time.deltaTime * ((camera.transform.position - targetPos).sqrMagnitude / 30);
    Vector3 pos = Vector2.Lerp(camera.transform.position, targetPos, moveSpeed * moveSpeed * fact);
    if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
    {
      pos = RefManager.instance.respawnLocation.position;
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
          float xDist = Mathf.Abs(pos.x - character.transform.position.x) + 1.5f;
          float yDist = Mathf.Abs(pos.y - character.transform.position.y) + 1.5f;
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
      if (RefManager.instance.squareEye.gameObject.activeInHierarchy && !RefManager.instance.squareEye.noCamera)
      {
        float xDist = Mathf.Abs(pos.x - RefManager.instance.squareEye.transform.position.x) + 1.5f;
        float yDist = Mathf.Abs(pos.y - RefManager.instance.squareEye.transform.position.y) + 1.5f;
        if (xDist > maxX)
        {
          maxX = xDist;
        }
        if (yDist > maxY)
        {
          maxY = yDist;
        }
      }
      if (RefManager.instance.triangleEye.gameObject.activeInHierarchy && !RefManager.instance.triangleEye.noCamera)
      {
        float xDist = Mathf.Abs(pos.x - RefManager.instance.triangleEye.transform.position.x) + 1.5f;
        float yDist = Mathf.Abs(pos.y - RefManager.instance.triangleEye.transform.position.y) + 1.5f;
        if (xDist > maxX)
        {
          maxX = xDist;
        }
        if (yDist > maxY)
        {
          maxY = yDist;
        }
      }
      foreach (Transform extraTransform in extraFocus)
      {
        float xDist = Mathf.Abs(pos.x - extraTransform.position.x) + 1.5f;
        float yDist = Mathf.Abs(pos.y - extraTransform.position.y) + 1.5f;
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

    camera.transform.position = new Vector3(pos.x, pos.y, -10);
    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, moveSpeed * Time.deltaTime * 10);
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

  private IEnumerator Respawn()
  {
    RefManager.instance.squareEye.noCamera = false;
    RefManager.instance.triangleEye.noCamera = false;
    Vector3 pos = RefManager.instance.squareEye.transform.position;
    pos.z = -5;
    RefManager.instance.squareEye.transform.position = pos;
    pos = RefManager.instance.triangleEye.transform.position;
    pos.z = -5;
    RefManager.instance.triangleEye.transform.position = pos;
    Vector3 resp = RefManager.instance.respawnLocation.position;
    resp.z = -5;
    StartCoroutine(Tween.EaseCoroutine(RefManager.instance.squareEye.transform, Tween.TRANSPROP_ROTATION, Quaternion.identity, 3, Tween.EASE_QUAD, Tween.EASE_QUAD));
    StartCoroutine(Tween.EaseCoroutine(RefManager.instance.triangleEye.transform, Tween.TRANSPROP_ROTATION, Quaternion.identity, 3, Tween.EASE_QUAD, Tween.EASE_QUAD));
    StartCoroutine(Tween.EaseCoroutine(RefManager.instance.squareEye.transform, Tween.TRANSPROP_POSITION, resp + 0.4f * Vector3.right, 3, Tween.EASE_QUAD, Tween.EASE_QUAD));
    StartCoroutine(Tween.EaseCoroutine(RefManager.instance.triangleEye.transform, Tween.TRANSPROP_POSITION, resp + 0.4f * Vector3.left + 0.1f * Vector3.down, 3, Tween.EASE_QUAD, Tween.EASE_QUAD));
    yield return new WaitForSeconds(3);
    Vector3 spawn = RefManager.instance.respawnLocation.position;
    spawn.z = 0.15f;
    RefManager.instance.circle.transform.position = spawn;
    RefManager.instance.squareEye.gameObject.SetActive(false);
    RefManager.instance.triangleEye.gameObject.SetActive(false);
    RefManager.instance.circle.transform.localScale = 0.25f * Vector3.one;
    RefManager.instance.circle.gameObject.SetActive(true);
    RefManager.instance.circle.StartCoroutine(Tween.EaseCoroutine(RefManager.instance.circle.transform, Tween.TRANSPROP_SCALE, Vector3.one, 0.75f, Tween.EASE_NONE, Tween.EASE_ELASTIC));
    respawning = false;
    SoundPlayer.PlaySound(SoundPlayer.Sound.COMBINE, 1);
    yield break;
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

}
