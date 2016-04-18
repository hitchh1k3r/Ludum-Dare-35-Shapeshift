using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class WorldNode : MonoBehaviour
{

  public PlayerInputControls playerInputControls;
  public Transform[] visionAnchors;

  public MozaicManager mozaic;
  public PlayerController circle;

  new private Collider2D collider;
  private bool active;
  private Vector2 lastVelocity;
  private float lastAngVelocity;

  void Awake()
  {
    collider = GetComponent<Collider2D>();
  }

  void LateUpdate()
  {
    if (active)
    {
      if (Input.GetButtonDown("Square_Shift") || Input.GetButtonDown("Triangle_Shift"))
      {
        active = false;
        mozaic.phyics.isKinematic = true;
        mozaic.phyics.velocity = Vector2.zero;
        mozaic.phyics.angularVelocity = 0;
        foreach (Transform r in visionAnchors)
        {
          PlayerInputControls.extraFocus.Remove(r);
        }
        PlayerInputControls.levelStep = 5;
        circle.transform.position = transform.position;
        circle.gameObject.SetActive(true);
      }
    }
    else
    {
      if (Input.GetButtonDown("Square_Shift") || Input.GetButtonDown("Triangle_Shift"))
      {
        if (Physics2D.IsTouching(circle.collider, collider))
        {
          active = true;
          mozaic.phyics.isKinematic = false;
          Vector3 lockPos = new Vector3(mozaic.transform.position.x, mozaic.transform.position.y, (mozaic.height * 5) + 5);
          lockPos += new Vector3(mozaic.width * 5, mozaic.height * 5, 0);
          PlayerInputControls.extraFocus.AddRange(visionAnchors);
          PlayerInputControls.levelStep = 0;
          circle.gameObject.SetActive(false);
        }
      }
    }
  }

  void Update()
  {
    if (active)
    {
      float change = (lastVelocity - mozaic.phyics.velocity).sqrMagnitude + Mathf.Abs(lastAngVelocity - mozaic.phyics.angularVelocity);
      if (change > 5)
      {
        if (change > 256)
        {
          change = 256;
        }
        playerInputControls.ScreenShake(Mathf.Sqrt(change) / 16);
      }
      lastVelocity = mozaic.phyics.velocity;
      lastAngVelocity = mozaic.phyics.angularVelocity;
      if (Input.GetAxisRaw("Triangle_Horizontal") < -0.1f || Input.GetAxisRaw("Triangle_Horizontal") > 0.1f)
      {
        mozaic.phyics.AddTorque(((Input.GetAxisRaw("Triangle_Horizontal") < 0) ? 70000 : -70000) * Time.deltaTime);
      }
      if (Input.GetAxisRaw("Square_Horizontal") < -0.1f || Input.GetAxisRaw("Square_Horizontal") > 0.1f)
      {
        mozaic.phyics.AddForce(new Vector3(((Input.GetAxisRaw("Square_Horizontal") < 0) ? -9000 : 9000) * Time.deltaTime, 0, 0));
      }
      if (Input.GetAxisRaw("Square_Vertical") < -0.1f || Input.GetAxisRaw("Square_Vertical") > 0.1f)
      {
        mozaic.phyics.AddForce(new Vector3(0, ((Input.GetAxisRaw("Square_Vertical") < 0) ? -9000 : 9000) * Time.deltaTime, 0));
      }
    }
  }

}
