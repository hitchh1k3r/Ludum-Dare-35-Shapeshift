using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

  public CharacterShape shape = CharacterShape.SQUARE;
  new public Collider2D collider;

  public float extendSpeed = 5;
  public float retractSpeed = 15;

  public float rotateSpeed = 80;

  public float horzExtend = 0;
  public float vertExtend = 0;

  private int lastRotateDir = 0;
  private Rigidbody2D physics;
  private bool hasControl;
  private bool jumpCooler;

  void Awake()
  {
    physics = GetComponent<Rigidbody2D>();
  }

  void LateUpdate()
  {
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
    hasControl = false;
  }

  public void ControlUpdate()
  {
    hasControl = true;
    if (shape == CharacterShape.SQUARE)
    {
      SquareControlUpdate();
    }
    else if (shape == CharacterShape.TRIANGLE)
    {
      TriangleControlUpdate();
    }
    else if (shape == CharacterShape.CIRCLE)
    {
      CircleControlUpdate();
    }
  }

  private void SquareControlUpdate()
  {
    float xDist = horzExtend;
    float yDist = vertExtend;
    int xDir = 0;
    int yDir = 0;
    if (Input.GetAxisRaw("Horizontal") < -0.1 || Input.GetAxisRaw("Horizontal") > 0.1)
    {
      xDir = Input.GetAxisRaw("Horizontal") < -0.1 ? 1 : -1;
      horzExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
      if (horzExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Horizontal")))
      {
        horzExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        xDir = 0;
      }
      if (vertExtend > 0)
      {
        vertExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
      }
    }
    else if (Input.GetAxisRaw("Vertical") < -0.1)
    {
      horzExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Vertical"));
      if (horzExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Vertical")))
      {
        horzExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Vertical"));
      }
      if (vertExtend > 0)
      {
        vertExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Vertical"));
      }
    }
    else if (Input.GetAxisRaw("Vertical") > 0.1)
    {
      yDir = Input.GetAxisRaw("Vertical") < -0.1 ? 1 : -1;
      vertExtend += Time.deltaTime * extendSpeed * Mathf.Abs(Input.GetAxisRaw("Vertical"));
      if (vertExtend > 2 * Mathf.Abs(Input.GetAxisRaw("Vertical")))
      {
        vertExtend = 2 * Mathf.Abs(Input.GetAxisRaw("Vertical"));
        yDir = 0;
      }
      if (horzExtend > 0)
      {
        horzExtend -= Time.deltaTime * retractSpeed * Mathf.Abs(Input.GetAxisRaw("Vertical"));
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

    // TODO (hitch) based on above check for BoxCollisions and reduce size!

    collider.enabled = false;

    RaycastHit2D up = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x * 0.85f, 0.001f), 0, Vector2.up, 1.5f);
    RaycastHit2D down = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x * 0.85f, 0.001f), 0, Vector2.down, 1.5f);
    if (up.collider != null && down.collider != null && up.collider.gameObject != down.collider.gameObject)
    {
      float vertSpace = down.distance + up.distance;
      if ((1 + vertExtend) / (1 + horzExtend) > vertSpace)
      {
        xDist += horzExtend;
        yDist += vertExtend;
        float newVert = vertSpace * (1 + horzExtend) - 1;
        float newHorz = horzExtend;
        if (newVert < 0)
        {
          newVert = 0;

          newHorz = (1 / vertSpace) - 1;
        }

        horzExtend = newHorz;
        vertExtend = newVert;

        xDist -= horzExtend;
        yDist -= vertExtend;
      }
    }
    RaycastHit2D left = Physics2D.BoxCast(transform.position, new Vector2(0.001f, transform.localScale.y * 0.85f), 0, Vector2.left, 1.5f);
    RaycastHit2D right = Physics2D.BoxCast(transform.position, new Vector2(0.001f, transform.localScale.y * 0.85f), 0, Vector2.right, 1.5f);
    if (left.collider != null && right.collider != null && left.collider.gameObject != right.collider.gameObject)
    {
      float horzSpace = left.distance + right.distance;
      if ((1 + horzExtend) / (1 + vertExtend) > horzSpace)
      {
        xDist += vertExtend;
        yDist += horzExtend;
        float newHorz = horzSpace * (1 + vertExtend) - 1;
        float newVert = vertExtend;
        if (newHorz < 0)
        {
          newHorz = 0;

          newVert = (1 / horzSpace) - 1;
        }

        vertExtend = newVert;
        horzExtend = newHorz;

        xDist -= vertExtend;
        yDist -= horzExtend;
      }
    }

    collider.enabled = true;

    transform.position = transform.position + new Vector3(0.75f * xDir * xDist, 0.75f * yDir * yDist, 0);
    transform.localScale = new Vector3((1 + horzExtend) / (1 + vertExtend), (1 + vertExtend) / (1 + horzExtend), 1);
  }

  private void SquareUpdate()
  {
  }

  private void TriangleControlUpdate()
  {
    if (Input.GetAxisRaw("Horizontal") < -0.1)
    {
      float bonus = 1 + ((transform.rotation.eulerAngles.z % 120) / 120);
      transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotateSpeed * bonus * bonus));
      transform.position = transform.position + new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
      lastRotateDir = -1;
    }
    else if (Input.GetAxisRaw("Horizontal") > 0.1)
    {
      float bonus = 2 - ((transform.rotation.eulerAngles.z % 120) / 120);
      transform.Rotate(new Vector3(0, 0, -Time.deltaTime * rotateSpeed * bonus * bonus));
      transform.position = transform.position + new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
      lastRotateDir = 1;
    }
    else if (Input.GetAxisRaw("Vertical") < -0.1)
    {
      transform.rotation = Quaternion.Euler(0, 0, 180);
    }
    else if (Input.GetAxisRaw("Vertical") > 0.1)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
  }

  private void TriangleUpdate()
  {
    if (!hasControl || (Input.GetAxisRaw("Horizontal") >= -0.1 && Input.GetAxisRaw("Horizontal") <= 0.1))
    {
      float zAngle = transform.rotation.eulerAngles.z;
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
          transform.position = transform.position + new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > deltaAngle)
          {
            amnt = deltaAngle;
          }
          zAngle -= amnt;
        }
        else
        {
          transform.position = transform.position + new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > 60 - deltaAngle)
          {
            amnt = 60 - deltaAngle;
          }
          zAngle += amnt;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, zAngle));
      }
      else if (lastRotateDir < 0)
      {
        float bonus = 1 + ((zAngle % 120) / 120);
        if (deltaAngle > 1)
        {
          transform.position = transform.position + new Vector3(-Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > 60 - deltaAngle)
          {
            amnt = 60 - deltaAngle;
          }
          zAngle += amnt;
        }
        else
        {
          transform.position = transform.position + new Vector3(Time.deltaTime * bonus * bonus * rotateSpeed / 100, 0, 0);
          float amnt = Time.deltaTime * bonus * bonus * rotateSpeed;
          if (amnt > deltaAngle)
          {
            amnt = deltaAngle;
          }
          zAngle -= amnt;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, zAngle));
      }
    }
  }

  private void CircleUpdate()
  {
  }

  private void CircleControlUpdate()
  {
    if (Input.GetAxisRaw("Horizontal") < -0.1 || Input.GetAxisRaw("Horizontal") > 0.1)
    {
      transform.position = transform.position + new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * 5, 0, 0);
    }
    if (Input.GetAxisRaw("Vertical") > 0.1)
    {
      collider.enabled = false;
      if (!jumpCooler)
      {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, (0.5f * transform.localScale.x) - 0.05f, Vector2.down, 0.15f);
        if (!jumpCooler && hit.collider != null)
        {
          physics.velocity = new Vector2(0, 5);
          jumpCooler = true;
        }
      }
      collider.enabled = true;
    }
    else if (jumpCooler)
    {
      jumpCooler = false;
    }
  }

  public enum CharacterShape
  {
    SQUARE,
    CIRCLE,
    TRIANGLE
  }

}
