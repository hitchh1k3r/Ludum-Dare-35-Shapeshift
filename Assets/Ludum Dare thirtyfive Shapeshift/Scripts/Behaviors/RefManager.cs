using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefManager : MonoBehaviour
{

  public static RefManager instance;

  public PlayerController circle;
  public PlayerController square;
  public PlayerController triangle;

  public EyePickupController circleEye;
  public EyePickupController squareEye;
  public EyePickupController triangleEye;

  [System.NonSerialized]
  public List<PlayerController> allCharacters = new List<PlayerController>();

  void Awake()
  {
    instance = this;
    allCharacters.Add(circle);
    allCharacters.Add(square);
    allCharacters.Add(triangle);
  }

}
