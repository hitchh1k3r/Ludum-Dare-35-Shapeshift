using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{

  public int characterIndex = 0;
  public CharacterShape[] shapes = new CharacterShape[] { CharacterShape.SQUARE, CharacterShape.CIRCLE, CharacterShape.TRIANGLE };

  public float extendSpeed = 5;
  public float retractSpeed = 15;

  public float horzExtend = 0;
  public float vertExtend = 0;

  private int lastRotateDir = 0;

  void Awake()
  {
  }

  void Update()
  {
    if (shapes[characterIndex] == CharacterShape.SQUARE)
    {
      SquareUpdate();
    }
    else if (shapes[characterIndex] == CharacterShape.TRIANGLE)
    {
      TriangleUpdate();
    }
    if (Input.GetButtonDown("Shapeshift"))
    {
      characterIndex = (characterIndex + 1) % shapes.Length;
      horzExtend = 0;
      vertExtend = 0;
      transform.localScale = new Vector2((1 + horzExtend) / (1 + vertExtend), (1 + vertExtend) / (1 + horzExtend));
    }
  }

  private void SquareUpdate()
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
    transform.position = transform.position + new Vector3(0.75f * xDir * xDist, 0.75f * yDir * yDist, 0);
    transform.localScale = new Vector2((1 + horzExtend) / (1 + vertExtend), (1 + vertExtend) / (1 + horzExtend));
  }

  private void TriangleUpdate()
  {
    transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 120));
    transform.position = transform.position + new Vector3(Time.deltaTime, 0, 0);
    lastRotateDir = 1;

    return;

    if (Input.GetAxisRaw("Horizontal") < -0.1)
    {
      transform.Rotate(new Vector3(0, 0, Time.deltaTime * 120));
      transform.position = transform.position + new Vector3(-Time.deltaTime, 0, 0);
      lastRotateDir = -1;
    }
    else if (Input.GetAxisRaw("Horizontal") > 0.1)
    {
      transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 120));
      transform.position = transform.position + new Vector3(Time.deltaTime, 0, 0);
      lastRotateDir = 1;
    }
    else
    {
      float zAngle = transform.rotation.eulerAngles.z;
      float deltaAngle = zAngle % 60;
      if (deltaAngle < 0.1 || deltaAngle > 59.9)
      {
        lastRotateDir = 0;
      }
      else if (lastRotateDir > 0)
      {
        if (deltaAngle < 59)
        {
          transform.position = transform.position + new Vector3(Time.deltaTime * 0.9f, 0, 0);
          float amnt = Time.deltaTime * 90;
          if (amnt > deltaAngle)
          {
            amnt = deltaAngle;
          }
          zAngle -= amnt;
        }
        else
        {
          transform.position = transform.position + new Vector3(-Time.deltaTime * 0.9f, 0, 0);
          float amnt = Time.deltaTime * 90;
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
        if (deltaAngle > 1)
        {
          transform.position = transform.position + new Vector3(-Time.deltaTime * 0.9f, 0, 0);
          float amnt = Time.deltaTime * 90;
          if (amnt > 60 - deltaAngle)
          {
            amnt = 60 - deltaAngle;
          }
          zAngle += amnt;
        }
        else
        {
          transform.position = transform.position + new Vector3(Time.deltaTime * 0.9f, 0, 0);
          float amnt = Time.deltaTime * 90;
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

  public enum CharacterShape
  {
    SQUARE,
    CIRCLE,
    TRIANGLE
  }

}
