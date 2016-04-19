using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

  public CharacterShape shape = CharacterShape.SQUARE;
  new public Collider2D collider;
  public EyeController[] eyes;

  public float extendSpeed = 5;
  public float retractSpeed = 15;

  public float rotateSpeed = 80;

  public float horzExtend = 0;
  public float vertExtend = 0;

  [System.NonSerialized]
  public Rigidbody2D physics;

  [System.NonSerialized]
  public float spaceTimer;

  [System.NonSerialized]
  public bool inNode;

  private int lastRotateDir = 0;
  private bool jumpCooler;
  private bool shaking;
  private bool lastShaking;
  private float shakeTimer;
  private bool noGravity;
  private float jumpChanger;
  private float wallTimer;

  void Awake()
  {
    physics = GetComponent<Rigidbody2D>();
  }

  void FixedUpdate()
  {
    if (!inNode)
    {
      if (!noGravity)
      {
        physics.AddForce(-9.85f * transform.up);
      }
    }
  }
 
  void Update()
  {
    if (!inNode)
    {
      if (transform.position.z != 0.15f)
      {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.15f);
      }
      if (shape == CharacterShape.SQUARE)
      {
        SquareUpdate();
      }
      else if (shape == CharacterShape.TRIANGLE)
      {
        TriangleUpdate();
      }
      else if (shape == CharacterShape.CIRCLE)
      {
        CircleUpdate();
      }
      spaceTimer += Time.deltaTime;
      if (spaceTimer > 1)
      {
        shaking = true;
        shakeTimer -= Time.deltaTime / 2;
        if (shakeTimer > 2f)
        {
          KillShape();
        }
      }
      if (shaking)
      {
        shakeTimer += Time.deltaTime;
        lastShaking = true;
        foreach (EyeController eye in eyes)
        {
          eye.transform.localPosition = (shakeTimer + 0.5f) * 0.15f * Random.insideUnitCircle;
        }
      }
      else if (lastShaking)
      {
        shakeTimer = 0;
        lastShaking = false;
        foreach (EyeController eye in eyes)
        {
          eye.transform.localPosition = Vector3.zero;
        }
      }
    }
    foreach (EyeController eye in eyes)
    {
      eye.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.up);
    }

    foreach (PlayerController p in RefManager.instance.allCharacters)
    {
      p.collider.enabled = false;
    }
    if (Physics2D.OverlapPoint(transform.position, ~(1 << 8)) != null)
    {
      if (wallTimer > 0.25f)
      {
        KillShape();
      }
      wallTimer += Time.deltaTime;
    }
    else
    {
      wallTimer = 0;
    }
    foreach (PlayerController p in RefManager.instance.allCharacters)
    {
      p.collider.enabled = true;
    }
  }

  public void KillShape()
  {
    SoundPlayer.PlaySound(SoundPlayer.Sound.DEATH, 1);
    Vector3 pos = transform.position;
    pos.z = 0.15f;
    if (shape == CharacterShape.CIRCLE || shape == CharacterShape.SQUARE)
    {
      RefManager.instance.squareEye.transform.position = pos;
      RefManager.instance.squareEye.gameObject.SetActive(true);
      horzExtend = 0;
      vertExtend = 0;
      if (spaceTimer > 1)
      {
        RefManager.instance.squareEye.noCamera = true;
      }
      else
      {
        RefManager.instance.squareEye.noCamera = false;
      }
    }
    if (shape == CharacterShape.CIRCLE || shape == CharacterShape.TRIANGLE)
    {
      RefManager.instance.triangleEye.transform.position = pos;
      RefManager.instance.triangleEye.gameObject.SetActive(true);
      collider.transform.rotation = Quaternion.identity;
      lastRotateDir = 0;
      if (spaceTimer > 1)
      {
        RefManager.instance.triangleEye.noCamera = true;
      }
      else
      {
        RefManager.instance.triangleEye.noCamera = false;
      }
    }
    gameObject.SetActive(false);
  }

  private void SquareUpdate()
  {
    float xDist = horzExtend;
    float yDist = vertExtend;
    int xDir = 0;
    int yDir = 0;

    if (Input.GetButton("Square_Shift"))
    {
      shaking = true;
      if (shakeTimer > 1.5f)
      {
        KillShape();
        return;
      }
    }
    else if (shaking)
    {
      shaking = false;
    }

    if (Input.GetAxisRaw("Square_Horizontal") < -0.1 || Input.GetAxisRaw("Square_Horizontal") > 0.1)
    {
      xDir = Input.GetAxisRaw("Square_Horizontal") < -0.1 ? 1 : -1;
      horzExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Horizontal"));
      if (horzExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Square_Horizontal")))
      {
        horzExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Square_Horizontal"));
        xDir = 0;
      }
      if (vertExtend > 0)
      {
        vertExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Horizontal"));
      }
    }
    else if (Input.GetAxisRaw("Square_Vertical") < -0.1)
    {
      horzExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
      if (horzExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Square_Vertical")))
      {
        horzExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
      }
      if (vertExtend > 0)
      {
        vertExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
      }
    }
    else if (Input.GetAxisRaw("Square_Vertical") > 0.1)
    {
      yDir = Input.GetAxisRaw("Square_Vertical") < -0.1 ? 1 : -1;
      vertExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
      if (vertExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Square_Vertical")))
      {
        vertExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
        yDir = 0;
      }
      if (horzExtend > 0)
      {
        horzExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Square_Vertical"));
      }
    }
    else
    {
      if (horzExtend > 0)
      {
        horzExtend -= Time.deltaTime * retractSpeed;
      }
      if (vertExtend > 0)
      {
        vertExtend -= Time.deltaTime * retractSpeed;
      }
      if (horzExtend < 0)
      {
        horzExtend = 0;
      }
      if (vertExtend < 0)
      {
        vertExtend = 0;
      }
    }

    if (horzExtend < 0)
    {
      horzExtend = 0;
    }
    if (vertExtend < 0)
    {
      vertExtend = 0;
    }

    xDist -= horzExtend;
    yDist -= vertExtend;

    collider.enabled = false;

    RaycastHit2D up = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x * 0.85f, 0.001f), 0, transform.up, 1.5f, ~(1 << 8));
    RaycastHit2D down = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x * 0.85f, 0.001f), 0, -transform.up, 1.5f, ~(1 << 8));
    if (up.collider != null && down.collider != null && up.collider.gameObject != down.collider.gameObject)
    {
      float vertSpace = down.distance + up.distance;
      if ((1 + vertExtend) / (1 + horzExtend) > vertSpace)
      {
        yDir = 0;

        xDist += horzExtend;
        yDist += vertExtend;
        float newVert = vertSpace * (1 + horzExtend) - 1;
        float newHorz = horzExtend;
        if (newVert < 0)
        {
          newVert = 0;

          newHorz = (1 / vertSpace) - 1;
          if (newHorz > 2)
          {
            newHorz = 2;
          }
        }

        horzExtend = newHorz;
        vertExtend = newVert;

        xDist -= horzExtend;
        yDist -= vertExtend;
      }
    }
    RaycastHit2D left = Physics2D.BoxCast(transform.position, new Vector2(0.001f, transform.localScale.y * 0.85f), 0, new Vector2(transform.up.y, -transform.up.x), 1.5f, ~(1 << 8));
    RaycastHit2D right = Physics2D.BoxCast(transform.position, new Vector2(0.001f, transform.localScale.y * 0.85f), 0, new Vector2(-transform.up.y, transform.up.x), 1.5f, ~(1 << 8));
    bool wallWedge = false;
    if (left.collider != null && right.collider != null && left.collider.gameObject != right.collider.gameObject)
    {
      float horzSpace = left.distance + right.distance;
      if ((1 + horzExtend) / (1 + vertExtend) > horzSpace)
      {
        xDir = 0;

        wallWedge = true;
        xDist += vertExtend;
        yDist += horzExtend;
        float newHorz = horzSpace * (1 + vertExtend) - 1;
        float newVert = vertExtend;
        if (newHorz < 0)
        {
          newHorz = 0;

          newVert = (1 / horzSpace) - 1;
          if (newVert > 2)
          {
            newVert = 2;
          }
        }

        vertExtend = newVert;
        horzExtend = newHorz;

        xDist -= vertExtend;
        yDist -= horzExtend;
      }
    }

    if (wallWedge)
    {
      noGravity = true;
      physics.drag = 10;
    }
    else
    {
      noGravity = false;
      physics.drag = 0;
    }


    collider.enabled = true;

    transform.position = transform.position + (transform.rotation * new Vector3(0.75f * xDir * xDist, 0.75f * yDir * yDist, 0));
    transform.localScale = new Vector3((1 + horzExtend) / (1 + vertExtend), (1 + vertExtend) / (1 + horzExtend), 1);
  }

  private void TriangleUpdate()
  {
    if (Input.GetButton("Triangle_Shift"))
    {
      shaking = true;
      if (shakeTimer > 1.5f)
      {
        KillShape();
        return;
      }
    }
    else if (shaking)
    {
      shaking = false;
    }

    collider.enabled = false;
    bool groundContact = (Physics2D.Raycast(transform.position, -transform.up, 1).collider != null);
    collider.enabled = true;
    if (Input.GetAxisRaw("Triangle_Horizontal") < -0.1)
    {
      float bonus = 1 + ((collider.transform.rotation.eulerAngles.z % 120) / 120);
      collider.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotateSpeed * bonus * bonus));
      transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
      lastRotateDir = -1;
    }
    else if (Input.GetAxisRaw("Triangle_Horizontal") > 0.1)
    {
      float bonus = 2 - ((collider.transform.rotation.eulerAngles.z % 120) / 120);
      collider.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * rotateSpeed * bonus * bonus));
      transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
      lastRotateDir = 1;
    }
    else
    {
      float zAngle = collider.transform.rotation.eulerAngles.z;
      float deltaAngle = zAngle % 60;
      if (deltaAngle < 0.1 || deltaAngle > 59.9)
      {
        lastRotateDir = 0;
      }
      else if (lastRotateDir > 0)
      {
        float bonus = 2 - ((zAngle % 120) / 120);
        if (deltaAngle < 59)
        {
          transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > deltaAngle)
          {
            amnt = deltaAngle;
          }
          zAngle -= amnt;
        }
        else
        {
          transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > 60 - deltaAngle)
          {
            amnt = 60 - deltaAngle;
          }
          zAngle += amnt;
        }
        collider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zAngle));
      }
      else if (lastRotateDir < 0)
      {
        float bonus = 1 + ((zAngle % 120) / 120);
        if (deltaAngle > 1)
        {
          transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > 60 - deltaAngle)
          {
            amnt = 60 - deltaAngle;
          }
          zAngle += amnt;
        }
        else
        {
          transform.position = transform.position + (transform.rotation * ((groundContact ? 1 : 0.1f) * new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0)));
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > deltaAngle)
          {
            amnt = deltaAngle;
          }
          zAngle -= amnt;
        }
        collider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zAngle));
      }
    }
  }

  private void CircleUpdate()
  {
    float horzAxis = Mathf.Clamp(Input.GetAxisRaw("Triangle_Horizontal") + Input.GetAxisRaw("Square_Horizontal"), -1, 1);
    float vertAxis = Mathf.Clamp(Input.GetAxisRaw("Triangle_Vertical") + Input.GetAxisRaw("Square_Vertical"), -1, 1);
    if (Mathf.Abs(Input.GetAxisRaw("Triangle_Horizontal") - Input.GetAxisRaw("Square_Horizontal")) > 1.75f ||
        Mathf.Abs(Input.GetAxisRaw("Triangle_Vertical") - Input.GetAxisRaw("Square_Vertical")) > 1.75f)
    {
      shaking = true;
      if (shakeTimer > 0.75f)
      {
        SoundPlayer.PlaySound(SoundPlayer.Sound.SPLIT, 1);
        shaking = false;
        RefManager.instance.triangle.transform.position = transform.position;
        RefManager.instance.square.transform.position = transform.position;
        RefManager.instance.triangle.gameObject.SetActive(true);
        RefManager.instance.square.gameObject.SetActive(true);
        gameObject.SetActive(false);
      }
    }
    else
    {
      shaking = false;
      if (horzAxis < -0.1 || horzAxis > 0.1)
      {
        transform.position = transform.position + ((horzAxis * Time.deltaTime * 5) * transform.right);
      }
      if (vertAxis > 0.1)
      {
        if (!jumpCooler)
        {
          collider.enabled = false;
          RaycastHit2D hit = Physics2D.CircleCast(transform.position, (0.8175f * transform.localScale.x) - 0.2f, -transform.up, 0.3f, ~(1 << 8));
          if (!jumpCooler && hit.collider != null && Vector2.Dot(hit.normal, transform.up) > 0.1)
          {
            SoundPlayer.PlaySound(SoundPlayer.Sound.JUMP_SMALL, 1);
            physics.velocity = -5 * -transform.up;
            jumpCooler = true;
          }
          collider.enabled = true;
        }
      }
      if (vertAxis < -0.1)
      {
        if (!jumpCooler)
        {
          collider.enabled = false;
          RaycastHit2D hit = Physics2D.CircleCast(transform.position, (0.8175f * transform.localScale.x) - 0.2f, -transform.up, 0.3f, ~(1 << 8));
          if (hit.collider != null && Vector2.Dot(hit.normal, transform.up) > 0.1)
          {
            jumpChanger += Time.deltaTime;
            if (jumpChanger > 2)
            {
              jumpChanger = 2;
            }
          }
          else
          {
            if (jumpChanger > 0.25f)
            {
              SoundPlayer.PlaySound(SoundPlayer.Sound.JUMP_BIG, 1);
              physics.velocity = ((3 * jumpChanger) + 4) * transform.up;
              jumpCooler = true;
              jumpChanger = 0;
            }
          }
          collider.enabled = true;
        }
      }
      else if (jumpChanger > 0.25f)
      {
        collider.enabled = false;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, (0.8175f * transform.localScale.x) - 0.2f, -transform.up, 0.3f, ~(1 << 8));
        if (hit.collider != null && Vector2.Dot(hit.normal, transform.up) > 0.1)
        {
          SoundPlayer.PlaySound(SoundPlayer.Sound.JUMP_BIG, 1);
          physics.velocity = ((3 * jumpChanger) + 4) * transform.up;
          jumpCooler = true;
        }
        collider.enabled = true;
        jumpChanger = 0;
      }
      else if (jumpCooler)
      {
        jumpCooler = false;
      }
    }
    if (jumpChanger > 0)
    {
      transform.localScale = new Vector3(1, 1 - (jumpChanger * 0.25f), 1);
    }
    else
    {
      transform.localScale = Vector3.one;
    }
  }

  public enum CharacterShape
  {
    SQUARE,
    CIRCLE,
    TRIANGLE
  }

}